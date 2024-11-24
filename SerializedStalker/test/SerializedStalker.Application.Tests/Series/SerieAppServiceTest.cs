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
using Volo.Abp.ObjectMapping;
using Autofac.Core;
using System.Linq;
using Volo.Abp.Domain.Entities;


public class SerieAppServiceTests
{
    private readonly Mock<IRepository<Serie, int>> _serieRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<ISeriesApiService> _seriesApiServiceMock;
    private readonly Mock<IObjectMapper> _objectMapper;
    private readonly Mock<IMonitoreoApiAppService> _monitoreoApiAppService;
    private readonly SerieAppService _serieAppService;

    public SerieAppServiceTests()
    {
        _serieRepositoryMock = new Mock<IRepository<Serie, int>>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _seriesApiServiceMock = new Mock<ISeriesApiService>();
        _objectMapper = new Mock<IObjectMapper>();
        _monitoreoApiAppService = new Mock<IMonitoreoApiAppService> { };

        _serieAppService = new SerieAppService(
            _serieRepositoryMock.Object,
            _seriesApiServiceMock.Object,
            _currentUserServiceMock.Object,
            _objectMapper.Object,
            _monitoreoApiAppService.Object
        );
    }

    //Tests para buscar serie

    /// <summary>
    /// Verifica que el método <c>BuscarSerieAsync</c> retorne una lista de series 
    /// cuando existen series que coinciden con el título y género proporcionados.
    /// </summary>
    [Fact]
    public async Task BuscarSerieAsync_ShouldReturnSeries_WhenSeriesExist()
    {
        // Arrange
        var titulo = "Test Title";
        var genero = "Test Genre";
        var series = new SerieDto[] { new SerieDto { Id = 1, Titulo = titulo } };

        _seriesApiServiceMock.Setup(s => s.BuscarSerieAsync(titulo, genero))
            .ReturnsAsync(series);

        // Act
        var result = await _serieAppService.BuscarSerieAsync(titulo, genero);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(titulo, result[0].Titulo);
    }

    /// <summary>
    /// Verifica que el método <c>BuscarTemporadaAsync</c> retorne una temporada 
    /// cuando existe una temporada que coincide con el ID de IMDb y el número de temporada proporcionados.
    /// </summary>
    [Fact]
    public async Task BuscarTemporadaAsync_ShouldReturnTemporada_WhenTemporadaExists()
    {
        // Arrange
        var imdbId = "tt1234567";
        var numeroTemporada = 1;
        var temporada = new TemporadaDto { NumeroTemporada = numeroTemporada };

        _seriesApiServiceMock.Setup(s => s.BuscarTemporadaAsync(imdbId, numeroTemporada))
            .ReturnsAsync(temporada);

        // Act
        var result = await _serieAppService.BuscarTemporadaAsync(imdbId, numeroTemporada);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(numeroTemporada, result.NumeroTemporada);
    }

    //Tests para calificar serie

    /// <summary>
    /// Verifica que el método <c>CalificarSerieAsync</c> lance una excepción <see cref="InvalidOperationException"/> 
    /// cuando el usuario ya ha calificado la serie.
    /// </summary>
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

    /// <summary>
    /// Verifica que el método <c>CalificarSerieAsync</c> agregue una nueva calificación 
    /// cuando el usuario no ha calificado la serie previamente.
    /// </summary>
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

    /// <summary>
    /// Verifica que el método <c>CalificarSerieAsync</c> agregue una nueva calificación 
    /// cuando el usuario no ha calificado la serie previamente (prueba de integración).
    /// </summary>
    [Fact]
    public async Task CalificarSerieAsync_ShouldAddCalificacion_WhenUserHasNotRated_Integration()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var serieId = 1;
        var serie = new Serie
        {
            CreatorId = userId,
            Calificaciones = new List<Calificacion>()
        };

        _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _serieRepositoryMock.Setup(r => r.GetAsync(serieId, true, It.IsAny<CancellationToken>())).ReturnsAsync(serie);

        var calificacionDto = new CalificacionDto
        {
            SerieID = serieId,
            calificacion = 5,
            comentario = "Last thing you will ever watch."
        };

        // Act
        await _serieAppService.CalificarSerieAsync(calificacionDto);

        // Assert
        _serieRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Serie>(s => s.Calificaciones.Count == 1), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifica que el método <c>CalificarSerieAsync</c> lance una excepción <see cref="InvalidOperationException"/> 
    /// cuando el usuario ya ha calificado la serie (prueba de integración).
    /// </summary>
    [Fact]
    public async Task CalificarSerieAsync_ShouldThrowException_WhenUserAlreadyRated_Integration()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var serieId = 1;
        var serie = new Serie
        {
            CreatorId = userId,
            Calificaciones = new List<Calificacion>
        {
            new Calificacion { UsuarioId = userId, calificacion = 5, comentario = "Great series!" }
        }
        };

        _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _serieRepositoryMock.Setup(r => r.GetAsync(serieId, true, It.IsAny<CancellationToken>())).ReturnsAsync(serie);

        var calificacionDto = new CalificacionDto
        {
            SerieID = serieId,
            calificacion = 5,
            comentario = "Lorem ipsum!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _serieAppService.CalificarSerieAsync(calificacionDto));
    }

    //Tests para persistir serie

    /// <summary>
    /// Verifica que el método <c>PersistirSeriesAsync</c> inserte una nueva serie cuando la serie no existe en el repositorio.
    /// </summary>
    [Fact]
    public async Task PersistirSeriesAsync_ShouldInsertNewSerie_WhenSerieDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var serieDto = new SerieDto
        {
            ImdbIdentificator = "tt1234567",
            TotalTemporadas = 3,
            Temporadas = new List<TemporadaDto>
        {
            new TemporadaDto { NumeroTemporada = 1, Titulo = "Temporada 1" }
        }
        };

        _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _serieRepositoryMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Serie, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Serie>());
        _objectMapper.Setup(m => m.Map<SerieDto, Serie>(It.IsAny<SerieDto>())).Returns((SerieDto dto) => new Serie
        {
            ImdbIdentificator = dto.ImdbIdentificator,
            TotalTemporadas = dto.TotalTemporadas,
            CreatorId = userId,
            Temporadas = dto.Temporadas.Select(t => new Temporada
            {
                NumeroTemporada = t.NumeroTemporada,
                Titulo = t.Titulo
            }).ToList()
        });

        // Act
        await _serieAppService.PersistirSeriesAsync(new[] { serieDto });

        // Assert
        _serieRepositoryMock.Verify(r => r.InsertAsync(It.Is<Serie>(s => s.ImdbIdentificator == "tt1234567" && s.TotalTemporadas == 3 && s.CreatorId == userId), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifica que el método <c>PersistirSeriesAsync</c> lance una excepción <see cref="InvalidOperationException"/> 
    /// cuando el método <c>GetListAsync</c> del repositorio retorna null.
    /// </summary>
    [Fact]
    public async Task PersistirSeriesAsync_ShouldThrowException_WhenGetListAsyncReturnsNull()
    {
        // Arrange
        var serieDto = new SerieDto
        {
            ImdbIdentificator = "tt1234567",
            TotalTemporadas = 3
        };

        _serieRepositoryMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Serie, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync((List<Serie>)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _serieAppService.PersistirSeriesAsync(new[] { serieDto }));
    }

    //Tests para modificar calificación

    /// <summary>
    /// Verifica que el método <c>ModificarCalificacionAsync</c> lance una excepción <see cref="EntityNotFoundException"/> 
    /// cuando la serie especificada no se encuentra en el repositorio.
    /// </summary>
    [Fact]
    public async Task ModificarCalificacionAsync_ShouldThrowException_WhenSerieNotFound()
    {
        // Arrange
        var calificacionDto = new CalificacionDto
        {
            SerieID = 1,
            calificacion = 5,
            comentario = "Great series!"
        };

        _serieRepositoryMock.Setup(r => r.GetAsync(calificacionDto.SerieID, true, It.IsAny<CancellationToken>())).ReturnsAsync((Serie)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _serieAppService.ModificarCalificacionAsync(calificacionDto));
    }

    /// <summary>
    /// Verifica que el método <c>ModificarCalificacionAsync</c> lance una excepción <see cref="InvalidOperationException"/> 
    /// cuando el usuario actual no puede ser encontrado.
    /// </summary>
    [Fact]
    public async Task ModificarCalificacionAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var calificacionDto = new CalificacionDto
        {
            SerieID = 1,
            calificacion = 5,
            comentario = "Great series!"
        };

        var serie = new Serie
        {
            CreatorId = Guid.NewGuid(),
            Calificaciones = new List<Calificacion>()
        };

        _serieRepositoryMock.Setup(r => r.GetAsync(calificacionDto.SerieID, true, It.IsAny<CancellationToken>())).ReturnsAsync(serie);
        _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns((Guid?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _serieAppService.ModificarCalificacionAsync(calificacionDto));
    }

    /// <summary>
    /// Verifica que el método <c>ModificarCalificacionAsync</c> lance una excepción <see cref="UnauthorizedAccessException"/> 
    /// cuando el usuario actual no está autorizado para modificar la calificación de la serie.
    /// </summary>
    [Fact]
    public async Task ModificarCalificacionAsync_ShouldThrowException_WhenUserNotAuthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var calificacionDto = new CalificacionDto
        {
            SerieID = 1,
            calificacion = 5,
            comentario = "Great series!"
        };

        var serie = new Serie
        {
            CreatorId = Guid.NewGuid(), // Different user
            Calificaciones = new List<Calificacion>()
        };

        _serieRepositoryMock.Setup(r => r.GetAsync(calificacionDto.SerieID, true, It.IsAny<CancellationToken>())).ReturnsAsync(serie);
        _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _serieAppService.ModificarCalificacionAsync(calificacionDto));
    }

    /// <summary>
    /// Verifica que el método <c>ModificarCalificacionAsync</c> lance una excepción <see cref="InvalidOperationException"/> 
    /// cuando no se encuentra una calificación existente para el usuario actual en la serie especificada.
    /// </summary>
    [Fact]
    public async Task ModificarCalificacionAsync_ShouldThrowException_WhenCalificacionNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var calificacionDto = new CalificacionDto
        {
            SerieID = 1,
            calificacion = 5,
            comentario = "Great series!"
        };

        var serie = new Serie
        {
            CreatorId = userId,
            Calificaciones = new List<Calificacion>()
        };

        _serieRepositoryMock.Setup(r => r.GetAsync(calificacionDto.SerieID, true, It.IsAny<CancellationToken>())).ReturnsAsync(serie);
        _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _serieAppService.ModificarCalificacionAsync(calificacionDto));
    }

    /// <summary>
    /// Verifica que el método <c>ModificarCalificacionAsync</c> actualice correctamente la calificación y el comentario de una serie 
    /// cuando se proporcionan datos válidos. Además, verifica que el método <c>UpdateAsync</c> del repositorio sea llamado una vez.
    /// </summary>
    [Fact]
    public async Task ModificarCalificacionAsync_ShouldUpdateCalificacion_WhenValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var calificacionDto = new CalificacionDto
        {
            SerieID = 1,
            calificacion = 5,
            comentario = "Great series!"
        };

        var calificacionExistente = new Calificacion
        {
            UsuarioId = userId,
            calificacion = 3,
            comentario = "Good series"
        };

        var serie = new Serie
        {
            CreatorId = userId,
            Calificaciones = new List<Calificacion> { calificacionExistente }
        };

        _serieRepositoryMock.Setup(r => r.GetAsync(calificacionDto.SerieID, true, It.IsAny<CancellationToken>())).ReturnsAsync(serie);
        _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);

        // Act
        await _serieAppService.ModificarCalificacionAsync(calificacionDto);

        // Assert
        Assert.Equal(calificacionDto.calificacion, calificacionExistente.calificacion);
        Assert.Equal(calificacionDto.comentario, calificacionExistente.comentario);
        _serieRepositoryMock.Verify(r => r.UpdateAsync(serie, It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task ObtenerSeriesAsync_Should_Show_Series()
    {
        // Arrange
        var serieId = 1;
        var serie = new Serie
        {
            Titulo = "Neo Test Serie",
            Clasificacion = "PG-99",
            FechaEstreno = "2000-01-01",
            Duracion = "1h",
            Generos = "Drama",
            Directores = "Director Test",
            Escritores = "Writer Test",
            Actores = "Actor Test",
            Sinopsis = "Neo Test Sinopsis",
            Idiomas = "Español",
            Pais = "Finlandia",
            Poster = "URL del poster",
            ImdbPuntuacion = "8.5",
            ImdbVotos = 1000,
            ImdbIdentificator = "tt1234567",
            Tipo = "Serie",
        };
        _serieRepositoryMock.Setup(r => r.GetAsync(serieId, true, It.IsAny<CancellationToken>())).ReturnsAsync(serie);


        //Act
        var seriesDto = await _serieAppService.ObtenerSeriesAsync();

        //Assert
        Assert.NotEmpty(seriesDto);
    }

}


