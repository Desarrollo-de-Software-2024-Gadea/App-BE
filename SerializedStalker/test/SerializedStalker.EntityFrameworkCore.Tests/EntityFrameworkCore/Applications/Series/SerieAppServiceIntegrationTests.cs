using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SerializedStalker;
using SerializedStalker.EntityFrameworkCore;
using SerializedStalker.Series;
using SerializedStalker.Usuarios;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Xunit;

public class SerieAppServiceIntegrationTests : AbpIntegratedTest<SerializedStalkerApplicationTestModule>
{
    private readonly ISerieAppService _serieAppService;
    private readonly ICurrentUserService _currentUserService;

    public SerieAppServiceIntegrationTests()
    {
        _serieAppService = GetRequiredService<ISerieAppService>();

        // Mockear ICurrentUserService
        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(Guid.NewGuid());
        _currentUserService = currentUserServiceMock.Object;
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

        var serieId = 1;

        // Crear una serie en la base de datos
        await UsingDbContextAsync(async context =>
        {
            var serie = new Serie
            {
                CreatorId = userId.Value,
                Titulo = "Test Serie",
                Calificaciones = new List<Calificacion>()
            };
            await context.Series.AddAsync(serie);
            await context.SaveChangesAsync();
            serieId = serie.Id; // Obtener el Id generado
        });

        var calificacionDto = new CalificacionDto
        {
            SerieID = serieId,
            calificacion = 5,
            comentario = "Last thing you will ever watch."
        };

        // Act
        await _serieAppService.CalificarSerieAsync(calificacionDto);

        // Assert
        var serie = await UsingDbContextAsync(async context => await context.Series.Include(s => s.Calificaciones).FirstOrDefaultAsync(s => s.Id == serieId));
        Assert.Single(serie!.Calificaciones);
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
        // Arrange
        var userId = _currentUserService.GetCurrentUserId();
        if (!userId.HasValue)
        {
            throw new InvalidOperationException("User ID cannot be null");
        }

        var serieId = 1;

        // Crear una serie en la base de datos con una calificación existente
        await UsingDbContextAsync(async context =>
        {
            var serie = new Serie
            {
                CreatorId = userId.Value,
                Titulo = "Test Serie",
                Calificaciones = new List<Calificacion>
                {
                    new Calificacion { UsuarioId = userId.Value, calificacion = 5, comentario = "Great series!" }
                }
            };
            await context.Series.AddAsync(serie);
            await context.SaveChangesAsync();
            serieId = serie.Id; // Obtener el Id generado
        });

        var calificacionDto = new CalificacionDto
        {
            SerieID = serieId,
            calificacion = 5,
            comentario = "Another great series!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _serieAppService.CalificarSerieAsync(calificacionDto));
    }
}


/*
using Moq;
using SerializedStalker.Usuarios;
using Volo.Abp.DependencyInjection;

public class SerieAppServiceIntegrationTests : AbpIntegratedTest<SerializedStalkerApplicationTestModule>
{
    private readonly ISerieAppService _serieAppService;
    private readonly ICurrentUserService _currentUserService;

    public SerieAppServiceIntegrationTests()
    {
        _serieAppService = GetRequiredService<ISerieAppService>();

        // Mockear ICurrentUserService
        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.Setup(s => s.GetCurrentUserId()).Returns(Guid.NewGuid());
        _currentUserService = currentUserServiceMock.Object;
    }

    // Resto del código de prueba...
}

*/