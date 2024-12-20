﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SerializedStalker.EntityFrameworkCore;
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
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using SerializedStalker;
using Microsoft.Extensions.Logging;



public class SerieAppServiceTests
{
    private readonly Mock<IRepository<Serie, int>> _serieRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<ISeriesApiService> _seriesApiServiceMock;
    private readonly Mock<IObjectMapper> _objectMapper;
    private readonly Mock<IMonitoreoApiAppService> _monitoreoApiAppService;
    private readonly SerieAppService _serieAppService;
    private readonly Mock<ILogger<SerieAppService>> _loggerMock;

    public SerieAppServiceTests()
    {
        _serieRepositoryMock = new Mock<IRepository<Serie, int>>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _seriesApiServiceMock = new Mock<ISeriesApiService>();
        _objectMapper = new Mock<IObjectMapper>();
        _monitoreoApiAppService = new Mock<IMonitoreoApiAppService> { };
        _loggerMock = new Mock<ILogger<SerieAppService>>();
        _serieAppService = new SerieAppService(
            _serieRepositoryMock.Object,
            _seriesApiServiceMock.Object,
            _currentUserServiceMock.Object,
            _objectMapper.Object,
            _monitoreoApiAppService.Object,
            _loggerMock.Object
        );
    }

    //Tests de obtener series

    /// <summary>
    /// Verifica que el método <c>ObtenerSeriesAsync</c> retorne una lista de series 
    /// cuando existen series en el repositorio.
    /// </summary>
    [Fact]
    public async Task ObtenerSeriesAsync_ShouldReturnSeries_WhenSeriesExist()
    {
        // Arrange
        var series = new List<Serie>
    {
        new Serie { Titulo = "Test Serie", Generos = "Drama", Sinopsis = "Una serie de prueba" }
    };
        var seriesDto = new List<SerieDto>
    {
        new SerieDto { Titulo = "Test Serie", Generos = "Drama", Sinopsis = "Una serie de prueba" }
    };

        _serieRepositoryMock.Setup(repo => repo.GetListAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(series);

        _objectMapper.Setup(mapper => mapper.Map<Serie[], SerieDto[]>(It.IsAny<Serie[]>()))
            .Returns(seriesDto.ToArray());

        // Act
        var result = await _serieAppService.ObtenerSeriesAsync();

        // Assert
        result.ShouldNotBeEmpty();
        result.Length.ShouldBe(1);
        result[0].Titulo.ShouldBe("Test Serie");
        result[0].Generos.ShouldBe("Drama");
        result[0].Sinopsis.ShouldBe("Una serie de prueba");
    }

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
            NroCalificacion = 5,
            Comentario = "Great series! Needs more Phillip, thou"
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
            NroCalificacion = 5,
            Comentario = "Great series! Needs more Phillip, thou"
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
            NroCalificacion = 5,
            Comentario = "Last thing you will ever watch."
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
            new Calificacion { UsuarioId = userId, NroCalificacion = 5, Comentario = "Great series!" }
        }
        };

        _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _serieRepositoryMock.Setup(r => r.GetAsync(serieId, true, It.IsAny<CancellationToken>())).ReturnsAsync(serie);

        var calificacionDto = new CalificacionDto
        {
            SerieID = serieId,
            NroCalificacion = 5,
            Comentario = "Lorem ipsum!"
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
            Titulo = "Test 2",
            Clasificacion = "PG-13",
            FechaEstreno = "2023-01-09",
            Duracion = "2h",
            Generos = "Drama",
            Directores = "Director Test",
            Escritores = "Writer Test",
            Actores = "Actor Test",
            Sinopsis = "Test Sinopsis",
            Idiomas = "Español",
            Pais = "España",
            Poster = "URL del poster",
            ImdbPuntuacion = "8.7",
            ImdbVotos = 1000,
            ImdbIdentificator = "tt1234567",
            Tipo = "Serie",
            TotalTemporadas = 3,
            Temporadas = new List<TemporadaDto>
        {
            new TemporadaDto { NumeroTemporada = 1, Titulo = "Temporada 1" },
            new TemporadaDto { NumeroTemporada = 2, Titulo = "Temporada 2" },
            new TemporadaDto { NumeroTemporada = 3, Titulo = "Temporada 3" },
        }
        };

        _currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(userId);
        _serieRepositoryMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Serie, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Serie>());
        _objectMapper.Setup(m => m.Map<SerieDto, Serie>(It.IsAny<SerieDto>())).Returns((SerieDto dto) => new Serie
        {
            Titulo = dto.Titulo,
            Clasificacion = dto.Clasificacion,
            FechaEstreno = dto.FechaEstreno,
            Duracion = dto.Duracion,
            Generos = dto.Generos,
            Directores = dto.Directores,
            Escritores = dto.Escritores,
            Actores = dto.Actores,
            Sinopsis = dto.Sinopsis,
            Idiomas = dto.Idiomas,
            Pais = dto.Pais,
            Poster = dto.Poster,
            ImdbPuntuacion = dto.ImdbPuntuacion,
            ImdbVotos = dto.ImdbVotos,
            ImdbIdentificator = dto.ImdbIdentificator,
            Tipo = dto.Tipo,
            TotalTemporadas = dto.TotalTemporadas,
            CreatorId = userId,
        });
        _objectMapper.Setup(m => m.Map<TemporadaDto, Temporada>(It.IsAny<TemporadaDto>())).Returns((TemporadaDto dto) => new Temporada
        {
            Titulo = dto.Titulo,
            FechaLanzamiento = dto.FechaLanzamiento,
            NumeroTemporada = dto.NumeroTemporada,
            SerieID = dto.SerieID
        });
        // Act
        await _serieAppService.PersistirSeriesAsync(new[] { serieDto });

        // Assert
        _serieRepositoryMock.Verify(r => r.InsertAsync(It.Is<Serie>(s => s.ImdbIdentificator == "tt1234567" && s.TotalTemporadas == 3 && s.CreatorId == userId), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifica que se lance una InvalidOperationException cuando se intenta persistir una serie que ya está persistida.
    /// </summary>
    [Fact]
    public async Task PersistirSeriesAsync_ShouldThrowInvalidOperationException_WhenSerieAlreadyPersisted()
    {
        // Arrange
        var serieDto = new SerieDto { ImdbIdentificator = "tt1234567", TotalTemporadas = 3 };
        var seriesDto = new[] { serieDto };

        var serieExistente = new Serie { ImdbIdentificator = "tt1234567", TotalTemporadas = 3 };
        _serieRepositoryMock.Setup(repo => repo.GetListAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Serie> { serieExistente });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _serieAppService.PersistirSeriesAsync(seriesDto));
        Assert.Equal("Serie ya esta persistida", exception.Message);
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
            NroCalificacion = 5,
            Comentario = "Great series!"
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
            NroCalificacion = 5,
            Comentario = "Great series!"
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
            NroCalificacion = 5,
            Comentario = "Great series!"
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
            NroCalificacion = 5,
            Comentario = "Great series!"
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
            NroCalificacion = 5,
            Comentario = "Great series!"
        };

        var calificacionExistente = new Calificacion
        {
            UsuarioId = userId,
            NroCalificacion = 3,
            Comentario = "Good series"
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
        Assert.Equal(calificacionDto.NroCalificacion, calificacionExistente.NroCalificacion);
        Assert.Equal(calificacionDto.Comentario, calificacionExistente.Comentario);
        _serieRepositoryMock.Verify(r => r.UpdateAsync(serie, It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
