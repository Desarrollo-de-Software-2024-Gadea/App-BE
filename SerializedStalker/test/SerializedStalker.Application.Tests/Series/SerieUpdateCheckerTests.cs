using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using SerializedStalker.Series;
using Xunit;

namespace SerializedStalker.Tests.Series
{
    public class SerieUpdateCheckerTests
    {
        private readonly Mock<ILogger<SerieUpdateChecker>> _loggerMock;
        private readonly Mock<ISerieUpdateService> _serieUpdateServiceMock; // Usar la interfaz
        private readonly SerieUpdateChecker _serieUpdateChecker;

        public SerieUpdateCheckerTests()
        {
            _loggerMock = new Mock<ILogger<SerieUpdateChecker>>();
            _serieUpdateServiceMock = new Mock<ISerieUpdateService>();

            // Configura el mock para que no haga nada al invocar el método
            _serieUpdateServiceMock
                .Setup(s => s.VerificarYActualizarSeriesAsync())
                .Returns(Task.CompletedTask); // Simular que el método completa correctamente

            _serieUpdateChecker = new SerieUpdateChecker(
                _loggerMock.Object,
                _serieUpdateServiceMock.Object);
        }

        [Fact]
        public async Task StartAsync_Should_Start_Timer()
        {
            // Act
            await _serieUpdateChecker.StartAsync(CancellationToken.None);

            // Assert
            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString() == "SerieUpdateChecker starting."),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task StopAsync_Should_Stop_Timer()
        {
            // Arrange
            await _serieUpdateChecker.StartAsync(CancellationToken.None);

            // Act
            await _serieUpdateChecker.StopAsync(CancellationToken.None);

            // Assert
            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString() == "SerieUpdateChecker stopping."),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task DoWork_Should_Call_UpdateService()
        {
            // Arrange
            await _serieUpdateChecker.StartAsync(CancellationToken.None); // Asegúrate de que el checker esté activo

            // Act
            var method = typeof(SerieUpdateChecker)
                .GetMethod("DoWork", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            try
            {
                await Task.Run(() => method.Invoke(_serieUpdateChecker, new object[] { null }));
            }
            catch (Exception ex)
            {
                // Captura cualquier excepción para identificar el problema
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Assert
            _serieUpdateServiceMock.Verify(s => s.VerificarYActualizarSeriesAsync(), Times.Once);

            // Limpiar
            await _serieUpdateChecker.StopAsync(CancellationToken.None);
        }


        [Fact]
        public void Dispose_Should_Dispose_Timer()
        {
            // Act
            _serieUpdateChecker.Dispose();

            // Assert
            // Asegúrate de que no lanza una excepción
            _serieUpdateChecker.Dispose();
        }
    }
}
