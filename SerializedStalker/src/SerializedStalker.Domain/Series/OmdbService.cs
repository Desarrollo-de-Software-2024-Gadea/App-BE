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
        private const string ApiKey = "f7ee0f42"; // Reemplaza con tu clave de OMDB
        private const string BaseUrl = "http://www.omdbapi.com/";

        // Método principal de búsqueda que controla la lógica de validación
        public async Task<SerieDto[]> BuscarSerieAsync(string titulo, string genero = null)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                throw new ArgumentException("El campo título es obligatorio para la búsqueda.", nameof(titulo));
            }

            if (!string.IsNullOrWhiteSpace(titulo) && string.IsNullOrWhiteSpace(genero))
            {
                return await BuscarPorTituloAsync(titulo);
            }

            if (!string.IsNullOrWhiteSpace(titulo) && !string.IsNullOrWhiteSpace(genero))
            {
                return await BuscarPorTituloYGeneroAsync(titulo, genero);
            }

            throw new ArgumentException("No se puede buscar solo por género. El título es obligatorio.");
        }

        private async Task<SerieDto[]> BuscarPorTituloAsync(string titulo)
        {
            var url = $"{BaseUrl}?apikey={ApiKey}&s={titulo}&type=series";
            return await ObtenerSeriesDesdeOmdbAsync(url);
        }

        private async Task<SerieDto[]> BuscarPorTituloYGeneroAsync(string titulo, string genero)
        {
            var url = $"{BaseUrl}?apikey={ApiKey}&s={titulo}&type=series";
            var series = await ObtenerSeriesDesdeOmdbAsync(url);

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

        private async Task<SerieDto[]> ObtenerSeriesDesdeOmdbAsync(string url)
        {
            // Inicializar el monitoreo del tiempo
            var monitoreo = new MonitoreoApi
            {
                HoraEntrada = DateTime.Now // Registrar la hora de entrada
            };

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                monitoreo.HoraSalida = DateTime.Now; // Registrar la hora de salida

                // Aquí podemos agregar el código para almacenar o procesar el monitoreo, si es necesario.

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(jsonResponse);

                if (json["Response"]?.ToString() == "False")
                {
                    return Array.Empty<SerieDto>();
                }

                var seriesJson = json["Search"];
                if (seriesJson == null)
                {
                    return Array.Empty<SerieDto>();
                }

                var seriesList = new List<SerieDto>();
                foreach (var serie in seriesJson)
                {
                    var serieId = serie["imdbID"]?.ToString();
                    var serieDetails = await ObtenerDetallesSerieAsync(serieId);

                    if (serieDetails != null)
                    {
                        seriesList.Add(serieDetails);
                    }
                }

                return seriesList.ToArray();
            }
        }

        private async Task<SerieDto> ObtenerDetallesSerieAsync(string imdbId)
        {
            var url = $"{BaseUrl}?apikey={ApiKey}&i={imdbId}";

            // Inicializar el monitoreo del tiempo
            var monitoreo = new MonitoreoApi
            {
                HoraEntrada = DateTime.Now // Registrar la hora de entrada
            };

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                monitoreo.HoraSalida = DateTime.Now; // Registrar la hora de salida

                // Aquí podemos agregar el código para almacenar o procesar el monitoreo, si es necesario.

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(jsonResponse);

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
}
