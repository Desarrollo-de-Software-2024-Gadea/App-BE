using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using SerializedStalker.ListasDeSeguimiento;
using SerializedStalker.Series;
using Volo.Abp.Domain.Repositories;
using System.Threading;

namespace SerializedStalker.Tests.ListasDeSeguimiento
{
    public class ListaDeSeguimientoAppServiceTests
    {
        private readonly Mock<IRepository<ListaDeSeguimiento, int>> _listaDeSeguimientoRepositoryMock;
        private readonly Mock<IRepository<Serie, int>> _serieRepositoryMock;
        private readonly ListaDeSeguimientoAppService _listaDeSeguimientoAppService;

        public ListaDeSeguimientoAppServiceTests()
        {
            _listaDeSeguimientoRepositoryMock = new Mock<IRepository<ListaDeSeguimiento, int>>();
            _serieRepositoryMock = new Mock<IRepository<Serie, int>>();
            _listaDeSeguimientoAppService = new ListaDeSeguimientoAppService(
                _listaDeSeguimientoRepositoryMock.Object,
                _serieRepositoryMock.Object
            );
        }

        [Fact]
        public async Task AddSerieAsync_Should_Add_Serie_To_Existing_List()
        {
            // Arrange
            var serie = new Serie
            {
                Titulo = "Test Serie" // No asignar Id aquí
            };
            var listaDeSeguimiento = new ListaDeSeguimiento
            {
                Series = new List<Serie>()
            };

            _listaDeSeguimientoRepositoryMock.Setup(r => r.GetListAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ListaDeSeguimiento> { listaDeSeguimiento });
            _serieRepositoryMock.Setup(r => r.GetAsync(1)).ReturnsAsync(serie);

            // Act
            await _listaDeSeguimientoAppService.AddSerieAsync(1);

            // Assert
            Assert.Single(listaDeSeguimiento.Series);
            Assert.Equal(serie, listaDeSeguimiento.Series.First());
            _listaDeSeguimientoRepositoryMock.Verify(r => r.UpdateAsync(listaDeSeguimiento), Times.Once);
        }


        [Fact]
        public async Task AddSerieAsync_Should_Create_New_List_When_No_List_Exists()
        {
            // Arrange
            var serie = new Serie
            {
                Titulo = "Test Serie" // No asignar Id aquí
            };
            _listaDeSeguimientoRepositoryMock.Setup(r => r.GetListAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ListaDeSeguimiento>());
            _serieRepositoryMock.Setup(r => r.GetAsync(1)).ReturnsAsync(serie);

            // Act
            await _listaDeSeguimientoAppService.AddSerieAsync(1);

            // Assert
            _listaDeSeguimientoRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<ListaDeSeguimiento>()), Times.Once);
            _listaDeSeguimientoRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<ListaDeSeguimiento>()), Times.Never);
        }
    }
}
