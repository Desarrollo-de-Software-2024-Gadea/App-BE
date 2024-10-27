using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using SerializedStalker.Series;
using SerializedStalker.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace SerializedStalker
{
    public class SerializedStalkerTestDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly SerializedStalkerDbContext _context;
        private readonly ILogger<SerializedStalkerTestDataSeedContributor> _logger;

        public SerializedStalkerTestDataSeedContributor(ICurrentTenant currentTenant, SerializedStalkerDbContext context, ILogger<SerializedStalkerTestDataSeedContributor> logger)
        {
            _currentTenant = currentTenant;
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            using (_currentTenant.Change(context?.TenantId))
            {
                if (await _context.Series.AnyAsync())
                {
                    return; // Datos ya sembrados
                }

                var userId = Guid.NewGuid(); // Simular un usuario

                var serie = new Serie
                {
                    CreatorId = userId,
                    Titulo = "Test Serie",
                    Clasificacion = "PG-13",
                    FechaEstreno = "2023-01-01",
                    Duracion = "1h",
                    Generos = "Drama",
                    Directores = "Director Test",
                    Escritores = "Writer Test",
                    Actores = "Actor Test",
                    Sinopsis = "Test Sinopsis",
                    Idiomas = "Español",
                    Pais = "España",
                    Poster = "URL del poster",
                    ImdbPuntuacion = "8.5",
                    ImdbVotos = 1000,
                    ImdbIdentificator = "tt1234567",
                    Tipo = "Serie",
                    Calificaciones = new List<Calificacion>()
                };

                await _context.Series.AddAsync(serie);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Serie de prueba creada con ID: {SerieId}", serie.Id);
            }
        }
    }
}
