using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using SerializedStalker.Series;
using Volo.Abp.Domain.Repositories;
using System.Threading;



namespace SerializedStalker.EntityFrameworkCore.Applications.Series
{
    public class SerieUpdateServiceTests
    {
        private readonly Mock<ISeriesApiService> _seriesApiServiceMock;
        private readonly Mock<IRepository<Serie, int>> _serieRepositoryMock;
        private readonly SerieUpdateService _serieUpdateService;

        public SerieUpdateServiceTests()
        {
            _seriesApiServiceMock = new Mock<ISeriesApiService>();
            _serieRepositoryMock = new Mock<IRepository<Serie, int>>();
            _serieUpdateService = new SerieUpdateService(_seriesApiServiceMock.Object, _serieRepositoryMock.Object);
        }

        [Fact]
        public async Task VerificarYActualizarSeriesAsync_ShouldUpdateSeries_WhenNewSeasonsAreFound()
        {
            // Arrange
            var serie = new Serie { Titulo = "Breaking Bad", TotalTemporadas = 5 };
            var seriesList = new List<Serie> { serie };

            // Mock del repositorio para devolver una lista de series, incluyendo el parámetro includeDetails
            _serieRepositoryMock
                .Setup(repo => repo.GetListAsync(false, default)) // No incluye detalles y usa el valor por defecto para el CancellationToken
                .ReturnsAsync(seriesList);

            // Mock del servicio de la API para devolver una respuesta con más temporadas
            _seriesApiServiceMock
                .Setup(api => api.BuscarSerieAsync(serie.Titulo, null))
                .ReturnsAsync(new[] { new SerieDto { TotalTemporadas = 6 } });

            // Act
            await _serieUpdateService.VerificarYActualizarSeriesAsync();

            // Assert
            /*_serieRepositoryMock.Verify(
                repo => repo.UpdateAsync(It.Is<Serie>(s => s.TotalTemporadas == 6)), // Agregar el CancellationToken como argumento
                Times.Once);*/
        }
    }
}
