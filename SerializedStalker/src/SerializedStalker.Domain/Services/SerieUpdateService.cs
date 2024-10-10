using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using SerializedStalker.Series;

namespace SerializedStalker.Series
{
    public class SerieUpdateService : DomainService
    {
        private readonly ISeriesApiService _seriesApiService;
        private readonly IRepository<Serie, int> _serieRepository;

        public SerieUpdateService(
            ISeriesApiService seriesApiService,
            IRepository<Serie, int> serieRepository)
        {
            _seriesApiService = seriesApiService;
            _serieRepository = serieRepository;
        }

        public async Task VerificarYActualizarSeriesAsync()
        {
            var series = await _serieRepository.GetListAsync();

            foreach (var serie in series)
            {
                var apiSeries = await _seriesApiService.BuscarSerieAsync(serie.Titulo, null);

                if (apiSeries != null && apiSeries.Length > 0)
                {
                    var apiSerie = apiSeries.FirstOrDefault();

                    // Si la serie tiene más temporadas, actualizamos el número total de temporadas
                    if (apiSerie.TotalTemporadas > serie.TotalTemporadas)
                    {
                        serie.TotalTemporadas = apiSerie.TotalTemporadas;
                        await _serieRepository.UpdateAsync(serie);
                    }

                    // Obtener la última temporada local
                    var ultimaTemporadaLocal = serie.Temporadas.OrderByDescending(t => t.NumeroTemporada).FirstOrDefault();
                    if (ultimaTemporadaLocal != null)
                    {
                        // Obtener la última temporada desde la API
                        var apiUltimaTemporada = await _seriesApiService.BuscarTemporadaAsync(apiSerie.ImdbID, ultimaTemporadaLocal.NumeroTemporada);

                        if (apiUltimaTemporada != null)
                        {
                            // Comparar la cantidad de episodios
                            if (apiUltimaTemporada.Episodios.Count > ultimaTemporadaLocal.Episodios.Count)
                            {
                                // Detectar episodios nuevos
                                var episodiosLocales = ultimaTemporadaLocal.Episodios.Select(e => e.NumeroEpisodio).ToHashSet();
                                var episodiosNuevos = apiUltimaTemporada.Episodios
                                    .Where(e => !episodiosLocales.Contains(e.NumeroEpisodio))
                                    .ToList();

                                if (episodiosNuevos.Any())
                                {
                                    // Lógica para manejar los episodios nuevos
                                    foreach (var episodioNuevo in episodiosNuevos)
                                    {
                                        var nuevoEpisodio = new Episodio
                                        {
                                            Titulo = episodioNuevo.Titulo,
                                            NumeroEpisodio = episodioNuevo.NumeroEpisodio,
                                            FechaEstreno = episodioNuevo.FechaEstreno,
                                            TemporadaID = ultimaTemporadaLocal.Id
                                        };

                                        // Agregar a la colección de episodios de la temporada local
                                        ultimaTemporadaLocal.Episodios.Add(nuevoEpisodio);
                                    }

                                    // Persistir los cambios en la temporada
                                    await _temporadaRepository.UpdateAsync(ultimaTemporadaLocal);

                                    // Generar y persistir la notificación para la serie (esto dependerá de tu implementación de notificaciones)
                                    // CrearNotificacion(serie, episodiosNuevos);
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
