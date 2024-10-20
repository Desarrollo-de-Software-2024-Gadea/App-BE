using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NSubstitute;
using SerializedStalker.Series;
using SerializedStalker.Usuarios;
using Volo.Abp.Domain.Repositories;
using Xunit;

public class SerieAppServiceTests
{
    private readonly Mock<IRepository<Serie, int>> _serieRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly SerieAppService _serieAppService;

    public SerieAppServiceTests()
    {
        _serieRepositoryMock = new Mock<IRepository<Serie, int>>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _serieAppService = new SerieAppService(_serieRepositoryMock.Object, null, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task CalificarSerieAsync_ShouldThrowException_WhenUserAlreadyRated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var serieId = 1;
        var serie = new Serie
        {
            // No se puede establecer el Id directamente, así que lo omitimos
            CreatorId = userId,
            Calificaciones = new List<Calificacion>
            {
                new Calificacion { UsuarioId = userId }
            }
        };

        _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _serieRepositoryMock.Setup(r => r.GetAsync(serieId, true, It.IsAny<CancellationToken>())).ReturnsAsync(serie);

        var calificacionDto = new CalificacionDto
        {
            SerieID = serieId,
            calificacion = 5,
            comentario = "Great series! Needs more Phillip, thou"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _serieAppService.CalificarSerieAsync(calificacionDto));
    }

    [Fact]
    public async Task CalificarSerieAsync_ShouldAddCalificacion_WhenUserHasNotRated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var serieId = 1;
        var serie = new Serie
        {
            // No se puede establecer el Id directamente, así que lo omitimos
            CreatorId = userId,
            Calificaciones = new List<Calificacion>()
        };

        _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _serieRepositoryMock.Setup(r => r.GetAsync(serieId, true, It.IsAny<CancellationToken>())).ReturnsAsync(serie);

        var calificacionDto = new CalificacionDto
        {
            SerieID = serieId,
            calificacion = 5,
            comentario = "Great series! Needs more Phillip, thou"
        };

        // Act
        await _serieAppService.CalificarSerieAsync(calificacionDto);

        // Assert
        _serieRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Serie>(s => s.Calificaciones.Count == 1), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
