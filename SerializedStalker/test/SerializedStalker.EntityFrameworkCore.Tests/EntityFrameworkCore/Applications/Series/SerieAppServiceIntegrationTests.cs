using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SerializedStalker;
using SerializedStalker.EntityFrameworkCore;
using SerializedStalker.Series;
using SerializedStalker.Usuarios;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Xunit;

public abstract class SerieAppServiceIntegrationTests<TStartupModule> : SerializedStalkerApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly ISerieAppService _serieAppService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SerieAppServiceIntegrationTests<TStartupModule>> _logger;
    private readonly IDataSeedContributor _testDataSeedContributor;

    public SerieAppServiceIntegrationTests()
    {
        _serieAppService = GetRequiredService<ISerieAppService>();

        // Mockear ICurrentUserService
        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(Guid.NewGuid());
        _currentUserService = currentUserServiceMock.Object;

        _logger = GetRequiredService<ILogger<SerieAppServiceIntegrationTests<TStartupModule>>>();
        _testDataSeedContributor = GetRequiredService<IDataSeedContributor>();
    }

    [Fact]
    public async Task CalificarSerieAsync_ShouldAddCalificacion_WhenUserHasNotRated()
    {
        // Arrange
        var userId = _currentUserService.GetCurrentUserId();
        if (!userId.HasValue)
        {
            throw new InvalidOperationException("User ID cannot be null");
        }

        // Obtener la serie de prueba creada en SerializedStalkerTestDataSeedContributor
        var serie = await UsingDbContextAsync(async context =>
        {
            var serie = await context.Series.FirstOrDefaultAsync(s => s.Titulo == "Test Serie");
            _logger.LogInformation("Serie obtenida: {SerieId}", serie?.Id);
            return serie;
        });

        Assert.NotNull(serie);

        var calificacionDto = new CalificacionDto
        {
            SerieID = serie.Id,
            calificacion = 5,
            comentario = "Last thing you will ever watch."
        };

        // Act
        await _serieAppService.CalificarSerieAsync(calificacionDto);

        // Assert
        var serieConCalificaciones = await UsingDbContextAsync(async context => await context.Series.Include(s => s.Calificaciones).FirstOrDefaultAsync(s => s.Id == serie.Id));
        Assert.Single(serieConCalificaciones!.Calificaciones);
    }

    private async Task UsingDbContextAsync(Func<SerializedStalkerDbContext, Task> action)
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<SerializedStalkerDbContext>();
            await action(context);
        }
    }

    private async Task<T> UsingDbContextAsync<T>(Func<SerializedStalkerDbContext, Task<T>> action)
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<SerializedStalkerDbContext>();
            return await action(context);
        }
    }

    [Fact]
    public async Task CalificarSerieAsync_ShouldThrowException_WhenUserAlreadyRated()
    {
        // Seed data
        await _testDataSeedContributor.SeedAsync(new DataSeedContext());

        // Arrange
        var userId = _currentUserService.GetCurrentUserId();
        if (!userId.HasValue)
        {
            throw new InvalidOperationException("User ID cannot be null");
        }

        // Obtener la serie de prueba creada en SerializedStalkerTestDataSeedContributor
        var serie = await UsingDbContextAsync(async context =>
        {
            var serie = await context.Series.FirstOrDefaultAsync(s => s.Titulo == "Test Serie");
            _logger.LogInformation("Serie obtenida: {SerieId}", serie?.Id);
            return serie;
        });

        Assert.NotNull(serie);

        // Agregar una calificación existente
        await UsingDbContextAsync(async context =>
        {
            serie.Calificaciones.Add(new Calificacion { UsuarioId = userId.Value, calificacion = 5, comentario = "Great series!" });
            await context.SaveChangesAsync();
        });

        var calificacionDto = new CalificacionDto
        {
            SerieID = serie.Id,
            calificacion = 5,
            comentario = "Lorem ipsum!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _serieAppService.CalificarSerieAsync(calificacionDto));
    }
}
