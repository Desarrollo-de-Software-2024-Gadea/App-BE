using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Volo.Abp.DependencyInjection;

namespace SerializedStalker.Series
{
    public class OmdbService : ISeriesApiService, ITransientDependency
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "f7ee0f42"; // Reemplaza con tu clave de OMDB
        private const string BaseUrl = "http://www.omdbapi.com/";

        public OmdbService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Método principal de búsqueda que controla la lógica de validación
        public async Task<SerieDto[]> BuscarSerieAsync(string titulo, string genero = null)
        {
            // Validación: El título es obligatorio
            if (string.IsNullOrWhiteSpace(titulo))
            {
                throw new ArgumentException("El campo título es obligatorio para la búsqueda.", nameof(titulo));
            }

            // Si se busca solo por título
            if (!string.IsNullOrWhiteSpace(titulo) && string.IsNullOrWhiteSpace(genero))
            {
                return await BuscarPorTituloAsync(titulo);
            }

            // Si se busca por título y género
            if (!string.IsNullOrWhiteSpace(titulo) && !string.IsNullOrWhiteSpace(genero))
            {
                return await BuscarPorTituloYGeneroAsync(titulo, genero);
            }

            // Si se llega aquí significa que se intentó buscar solo por género, lo cual no está permitido
            throw new ArgumentException("No se puede buscar solo por género. El título es obligatorio.");
        }

        // Método auxiliar para buscar solo por título
        private async Task<SerieDto[]> BuscarPorTituloAsync(string titulo)
        {
            var url = $"{BaseUrl}?apikey={ApiKey}&s={titulo}&type=series";
            return await ObtenerSeriesDesdeOmdbAsync(url);
        }

        // Método auxiliar para buscar por título y género
        private async Task<SerieDto[]> BuscarPorTituloYGeneroAsync(string titulo, string genero)
        {
            var url = $"{BaseUrl}?apikey={ApiKey}&s={titulo}&type=series";
            var series = await ObtenerSeriesDesdeOmdbAsync(url);

            // Filtrar las series por género
            var seriesFiltradas = new List<SerieDto>();
            foreach (var serie in series)
            {
                if (serie.Generos != null && serie.Generos.Contains(genero, StringComparison.OrdinalIgnoreCase))
                {
                    seriesFiltradas.Add(serie);
                }
            }

            return seriesFiltradas.ToArray();
        }

        // Método para obtener series desde OMDb y mapearlas a SerieDto
        private async Task<SerieDto[]> ObtenerSeriesDesdeOmdbAsync(string url)
        {
            // Hacer la petición HTTP
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // Leer el contenido JSON
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(jsonResponse);

            // Verificar si hay resultados
            if (json["Response"]?.ToString() == "False")
            {
                return Array.Empty<SerieDto>();
            }

            // Obtener la lista de series desde el JSON
            var seriesJson = json["Search"];
            if (seriesJson == null)
            {
                return Array.Empty<SerieDto>();
            }

            // Mapear los resultados de OMDB a SerieDto
            var seriesList = new List<SerieDto>();
            foreach (var serie in seriesJson)
            {
                // Hacer otra llamada a la API para obtener detalles adicionales por ID de IMDb
                var serieId = serie["imdbID"]?.ToString();
                var serieDetails = await ObtenerDetallesSerieAsync(serieId);

                if (serieDetails != null)
                {
                    seriesList.Add(serieDetails);
                }
            }

            return seriesList.ToArray();
        }

        // Método para obtener detalles de una serie usando su ID de IMDb
        private async Task<SerieDto> ObtenerDetallesSerieAsync(string imdbId)
        {
            var url = $"{BaseUrl}?apikey={ApiKey}&i={imdbId}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(jsonResponse);

            // Mapear los detalles a SerieDto
            return new SerieDto
            {
                Titulo = json["Title"]?.ToString(),
                Clasificacion = json["Rated"]?.ToString(),
                FechaEstreno = json["Released"]?.ToString(),
                Duracion = json["Runtime"]?.ToString(),
                Generos = json["Genre"]?.ToString(),
                Directores = json["Director"]?.ToString(),
                Escritores = json["Writer"]?.ToString(),
                Actores = json["Actors"]?.ToString(),
                Sinopsis = json["Plot"]?.ToString(),
                Idiomas = json["Language"]?.ToString(),
                Pais = json["Country"]?.ToString(),
                Poster = json["Poster"]?.ToString(),
                ImdbPuntuacion = json["imdbRating"]?.ToString(),
                ImdbVotos = int.TryParse(json["imdbVotes"]?.ToString().Replace(",", ""), out var votes) ? votes : 0,
                Tipo = json["Type"]?.ToString(),
                TotalTemporadas = int.TryParse(json["totalSeasons"]?.ToString(), out var seasons) ? seasons : 0
            };
        }
    }
}
