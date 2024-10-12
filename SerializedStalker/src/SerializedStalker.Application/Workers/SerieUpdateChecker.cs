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
        private readonly ISerieUpdateService _serieUpdateService; // Usando la interfaz
        private Timer _timer;
        private bool _isRunning; // Añadido para controlar si el timer está activo

        public SerieUpdateChecker(
            ILogger<SerieUpdateChecker> logger,
            ISerieUpdateService serieUpdateService)
        {
            _logger = logger;
            _serieUpdateService = serieUpdateService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_isRunning) // Verificar si ya está en ejecución
            {
                return Task.CompletedTask; // No iniciar de nuevo
            }

            _logger.LogInformation("SerieUpdateChecker starting.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(86400000));
            _isRunning = true; // Marcar como en ejecución
            return Task.CompletedTask;
        }

        public void DoWork(object state)
        {
            _ = DoWorkAsync(state); // Llamada asincrónica
        }

        private async Task DoWorkAsync(object state)
        {
            if (_serieUpdateService == null)
            {
                _logger.LogWarning("SerieUpdateService is not initialized.");
                return; // Manejo de la situación donde el servicio es null
            }

            _logger.LogInformation("SerieUpdateChecker running verification.");
            await _serieUpdateService.VerificarYActualizarSeriesAsync();
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SerieUpdateChecker stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            _isRunning = false; // Marcar como no en ejecución
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
