using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NSubstitute;
using SerializedStalker.Series;
using SerializedStalker.Usuarios;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Xunit;

public class SerieAppServiceTests
{
    private readonly Mock<IRepository<Serie, int>> _serieRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<ISeriesApiService> _seriesApiServiceMock;
    private readonly IMapper _objectMapper;
    private readonly TestableSerieAppService _serieAppService;

    public SerieAppServiceTests()
    {
        _serieRepositoryMock = new Mock<IRepository<Serie, int>>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _seriesApiServiceMock = new Mock<ISeriesApiService>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<SerieDto, Serie>();
            cfg.CreateMap<TemporadaDto, Temporada>();
        });

        _objectMapper = config.CreateMapper();

        _serieAppService = new TestableSerieAppService(
            _serieRepositoryMock.Object,
            _seriesApiServiceMock.Object,
            _currentUserServiceMock.Object,
            _objectMapper
        );
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

    [Fact]
    public async Task PersistirSeriesAsync_ShouldThrowException_WhenSerieAlreadyExistsForUser()
    {
        // Arrange
        Guid userId = Guid.NewGuid(); // Asumiendo que el ID del usuario es un entero
        var serieDto = new SerieDto
        {
            ImdbIdentificator = "tt1234567",
            TotalTemporadas = 3
        };

        var serieExistente = new Serie
        {
            ImdbIdentificator = "tt1234567",
            CreatorId = userId,
            TotalTemporadas = 2
        };

        _serieRepositoryMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Serie, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Serie> { serieExistente });

        _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _serieAppService.PersistirSeriesAsync(new[] { serieDto }));
    }
}

// Clase derivada para tests
public class TestableSerieAppService : SerieAppService
{
    public TestableSerieAppService(
        IRepository<Serie, int> serieRepository,
        ISeriesApiService seriesApiService,
        ICurrentUserService currentUserService,
        IMapper objectMapper)
        : base(serieRepository, seriesApiService, currentUserService)
    {
        SetObjectMapper(objectMapper);
    }

    protected void SetObjectMapper(IMapper objectMapper)
    {
        typeof(ApplicationService).GetProperty("ObjectMapper", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(this, objectMapper);
    }
}





