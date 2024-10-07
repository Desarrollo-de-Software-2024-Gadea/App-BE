using Microsoft.Extensions.Logging;
using Moq;
using SerializedStalker.Series;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SerializedStalker.Tests.Series
{
    public class SerieUpdateCheckerTests
    {
        private readonly Mock<ILogger<SerieUpdateChecker>> _loggerMock;
        private readonly Mock<SerieUpdateService> _serieUpdateServiceMock;
        private readonly SerieUpdateChecker _serieUpdateChecker;

        public SerieUpdateCheckerTests()
        {
            _loggerMock = new Mock<ILogger<SerieUpdateChecker>>();
            _serieUpdateServiceMock = new Mock<SerieUpdateService>();
            _serieUpdateChecker = new SerieUpdateChecker(_loggerMock.Object, _serieUpdateServiceMock.Object);
        }

        [Fact]
        public async Task StartAsync_LlamaVerificarYActualizarSeriesAsync()
        {
            // Act
            await _serieUpdateChecker.StartAsync(CancellationToken.None);

            // Assert
            _serieUpdateServiceMock.Verify(service => service.VerificarYActualizarSeriesAsync(), Times.Once);
        }

        [Fact]
        public async Task StopAsync_NoLlamaVerificarYActualizarSeriesAsync()
        {
            // Act
            await _serieUpdateChecker.StopAsync(CancellationToken.None);

            // Assert
            _serieUpdateServiceMock.Verify(service => service.VerificarYActualizarSeriesAsync(), Times.Never);
        }
    }
}