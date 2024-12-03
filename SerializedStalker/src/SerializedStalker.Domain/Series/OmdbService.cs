using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Volo.Abp.DependencyInjection;

namespace SerializedStalker.Series
{
    public class OmdbService : ISeriesApiService, ITransientDependency
    {
        private const string ApiKey = "f7ee0f42"; // Reemplaza con tu clave de OMDB
        private const string BaseUrl = "http://www.omdbapi.com/";
        private readonly ILogger<OmdbService> _logger;

        public OmdbService(ILogger<OmdbService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Busca series en la API de OMDB por título y opcionalmente por género.
        /// </summary>
        /// <param name="titulo">El título de la serie a buscar. Este campo es obligatorio.</param>
        /// <param name="genero">El género de la serie a buscar. Este campo es opcional.</param>
        /// <returns>Un array de objetos <see cref="SerieDto"/> que coinciden con los criterios de búsqueda.</returns>
        /// <exception cref="ArgumentException">Se lanza si el campo título está vacío o solo se proporciona el género.</exception>
        public async Task<SerieDto[]> BuscarSerieAsync(string titulo, string genero = null)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                _logger.LogError("El campo título es obligatorio para la búsqueda.");
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

            _logger.LogError("No se puede buscar solo por género. El título es obligatorio.");
            throw new ArgumentException("No se puede buscar solo por género. El título es obligatorio.");
        }

        /// <summary>
        /// Busca series en la API de OMDB por título.
        /// </summary>
        /// <param name="titulo">El título de la serie a buscar.</param>
        /// <returns>Un array de objetos <see cref="SerieDto"/> que coinciden con el título proporcionado.</returns>
        private async Task<SerieDto[]> BuscarPorTituloAsync(string titulo)
        {
            var url = $"{BaseUrl}?apikey={ApiKey}&s={titulo}&type=series";
            _logger.LogInformation("Buscando series por título: {Titulo}", titulo);
            return await ObtenerSeriesDesdeOmdbAsync(url);
        }

        /// <summary>
        /// Busca series en la API de OMDB por título y filtra los resultados por género.
        /// </summary>
        /// <param name="titulo">El título de la serie a buscar.</param>
        /// <param name="genero">El género de la serie a buscar.</param>
        /// <returns>Un array de objetos <see cref="SerieDto"/> que coinciden con el título y género proporcionados.</returns>
        private async Task<SerieDto[]> BuscarPorTituloYGeneroAsync(string titulo, string genero)
        {
            var url = $"{BaseUrl}?apikey={ApiKey}&s={titulo}&type=series";
            _logger.LogInformation("Buscando series por título: {Titulo} y género: {Genero}", titulo, genero);
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

        /// <summary>
        /// Realiza una solicitud a la API de OMDB para obtener series basadas en la URL proporcionada.
        /// </summary>
        /// <param name="url">La URL de la solicitud a la API de OMDB.</param>
        /// <returns>Un array de objetos <see cref="SerieDto"/> obtenidos de la API de OMDB.</returns>
        /// <exception cref="Exception">Se lanza si hay un error en la respuesta de la API o si no se encuentran series.</exception>
        private async Task<SerieDto[]> ObtenerSeriesDesdeOmdbAsync(string url)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    _logger.LogInformation("Realizando solicitud a OMDB API: {Url}", url);
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(jsonResponse);

                    if (json["Response"]?.ToString() == "False")
                    {
                        var error = json["Error"]?.ToString();
                        _logger.LogError("Error en la respuesta de la API: {Error}", error);
                        throw new Exception("Error en la respuesta de la API: " + error);
                    }

                    var seriesJson = json["Search"];
                    if (seriesJson == null)
                    {
                        _logger.LogWarning("No se encontraron series en la respuesta de la API.");
                        throw new Exception("No se encontraron series en la respuesta de la API.");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción en ObtenerSeriesDesdeOmdbAsync");
                throw new Exception("Excepción en ObtenerSeriesDesdeOmdbAsync: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Busca una temporada específica de una serie en la API de OMDB por IMDb ID y número de temporada.
        /// </summary>
        /// <param name="imdbId">El identificador IMDb de la serie.</param>
        /// <param name="numeroTemporada">El número de la temporada a buscar.</param>
        /// <returns>Un objeto <see cref="TemporadaDto"/> que representa la temporada buscada, o null si no se encuentra.</returns>
        /// <exception cref="ArgumentException">Se lanza si el identificador IMDb está vacío.</exception>
        /// <exception cref="Exception">Se lanza si hay un error en la solicitud a la API.</exception>
        public async Task<TemporadaDto> BuscarTemporadaAsync(string imdbId, int numeroTemporada)
        {
            if (string.IsNullOrWhiteSpace(imdbId))
            {
                _logger.LogError("El identificador IMDb es obligatorio para buscar una temporada.");
                throw new ArgumentException("El identificador IMDb es obligatorio para buscar una temporada.", nameof(imdbId));
            }

            var url = $"{BaseUrl}?apikey={ApiKey}&i={imdbId}&season={numeroTemporada}";
            _logger.LogInformation("Buscando temporada {NumeroTemporada} para la serie con IMDb ID: {ImdbId}", numeroTemporada, imdbId);

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(jsonResponse);

                    if (json["Response"]?.ToString() == "False")
                    {
                        _logger.LogWarning("No se encontró la temporada {NumeroTemporada} para la serie con IMDb ID: {ImdbId}", numeroTemporada, imdbId);
                        return null;
                    }

                    var episodiosJson = json["Episodes"];
                    if (episodiosJson == null)
                    {
                        _logger.LogWarning("No se encontraron episodios para la temporada {NumeroTemporada} de la serie con IMDb ID: {ImdbId}", numeroTemporada, imdbId);
                        return null;
                    }

                    var episodiosList = new List<EpisodioDto>();
                    foreach (var episodio in episodiosJson)
                    {
                        episodiosList.Add(new EpisodioDto
                        {
                            Titulo = episodio["Title"]?.ToString(),
                            NumeroEpisodio = int.TryParse(episodio["Episode"]?.ToString(), out var episodioNum) ? episodioNum : 0,
                            FechaEstreno = DateOnly.TryParse(episodio["Released"]?.ToString(), out var fecha) ? fecha : DateOnly.MinValue
                        });
                    }

                    return new TemporadaDto
                    {
                        Titulo = json["Title"]?.ToString(),
                        NumeroTemporada = int.TryParse(json["Season"]?.ToString(), out var seasonNumber) ? seasonNumber : 0,
                        Episodios = episodiosList
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción en BuscarTemporadaAsync");
                throw new Exception("Excepción en BuscarTemporadaAsync: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene los detalles de una serie específica en la API de OMDB por IMDb ID.
        /// </summary>
        /// <param name="imdbId">El identificador IMDb de la serie.</param>
        /// <returns>Un objeto <see cref="SerieDto"/> que representa los detalles de la serie.</returns>
        /// <exception cref="Exception">Se lanza si hay un error en la solicitud a la API.</exception>
        private async Task<SerieDto> ObtenerDetallesSerieAsync(string imdbId)
        {
            var url = $"{BaseUrl}?apikey={ApiKey}&i={imdbId}";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    _logger.LogInformation("Obteniendo detalles de la serie con IMDb ID: {ImdbId}", imdbId);
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

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
                        ImdbIdentificator = imdbId,
                        ImdbVotos = int.TryParse(json["imdbVotes"]?.ToString().Replace(",", ""), out var votes) ? votes : 0,
                        Tipo = json["Type"]?.ToString(),
                        TotalTemporadas = int.TryParse(json["totalSeasons"]?.ToString(), out var seasons) ? seasons : 0
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción en ObtenerDetallesSerieAsync");
                throw new Exception("Excepción en ObtenerDetallesSerieAsync: " + ex.Message, ex);
            }
        }
    }
}