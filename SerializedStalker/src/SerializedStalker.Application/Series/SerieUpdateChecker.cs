using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Notification;
using Volo.Abp.Threading;

namespace SerializedStalker.Series
{
    public class SerieUpdateChecker : AbpBackgroundWorkerBase
    {
        private readonly ISeriesApiService _seriesApiService;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly INotificationPublisher _notificationPublisher;

        public SerieUpdateChecker(
            ISeriesApiService seriesApiService,
            IRepository<Serie, int> serieRepository,
            INotificationPublisher notificationPublisher)
        {
            _seriesApiService = seriesApiService;
            _serieRepository = serieRepository;
            _notificationPublisher = notificationPublisher;
        }

        // Sobrescribir el método 'StartAsync' no es necesario en AbpBackgroundWorkerBase

        public override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            // Obtener todas las series almacenadas
            var series = await _serieRepository.GetListAsync();

            foreach (var serie in series)
            {
                // Consultar la API para obtener los detalles más recientes
                var apiSeries = await _seriesApiService.BuscarPorTituloAsync(serie.Titulo);

                if (apiSeries != null && apiSeries.Length > 0)
                {
                    var apiSerie = apiSeries.FirstOrDefault();

                    // Verificar si hay cambios (nueva temporada, nuevo episodio, etc.)
                    if (apiSerie.TotalTemporadas > serie.TotalTemporadas)
                    {
                        // Actualizar la serie en la base de datos
                        serie.TotalTemporadas = apiSerie.TotalTemporadas;
                        await _serieRepository.UpdateAsync(serie);

                        // Generar una notificación
                        await _notificationPublisher.PublishAsync(
                            "SerieActualizada",
                            new NotificationData
                            {
                                { "Message", $"La serie {serie.Titulo} tiene una nueva temporada disponible." }
                            }
                        );
                    }
                }
            }
        }
    }
}