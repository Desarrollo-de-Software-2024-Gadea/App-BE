using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SerializedStalker.Series
{
    public class SerieUpdateChecker : IHostedService, IDisposable
    {
        private readonly ILogger<SerieUpdateChecker> _logger;
        private readonly SerieUpdateService _serieUpdateService;
        private Timer _timer;

        public SerieUpdateChecker(
            ILogger<SerieUpdateChecker> logger,
            SerieUpdateService serieUpdateService)
        {
            _logger = logger;
            _serieUpdateService = serieUpdateService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SerieUpdateChecker starting.");

            // Configurar el worker para que ejecute la verificación cada 24 horas (86400000 ms = 24 horas)
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(86400000));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("SerieUpdateChecker running verification.");

            // Llamar al servicio de dominio para verificar y actualizar las series
            await _serieUpdateService.VerificarYActualizarSeriesAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SerieUpdateChecker stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}