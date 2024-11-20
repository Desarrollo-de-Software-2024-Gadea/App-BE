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
using SerializedStalker.ListasDeSeguimiento;
using Volo.Abp.Domain.Repositories;

namespace SerializedStalker
{
    public class SerializedStalkerTestDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly IRepository<ListaDeSeguimiento, int> _listaDeSeguimientoRepository;
        private readonly ICurrentTenant _currentTenant;
        //private readonly SerializedStalkerDbContext _context;
        private readonly ILogger<SerializedStalkerTestDataSeedContributor> _logger;


        public SerializedStalkerTestDataSeedContributor(
            IRepository<Serie, int> serieRepository, 
            IRepository<ListaDeSeguimiento, int> listaDeSeguimientoRepository, 
            ICurrentTenant currentTenant, //SerializedStalkerDbContext context, 
            ILogger<SerializedStalkerTestDataSeedContributor> logger)
        {
            _serieRepository = serieRepository;
            _listaDeSeguimientoRepository = listaDeSeguimientoRepository;
            _currentTenant = currentTenant;
          //  _context = context;
            _logger = logger;
            
        }

        public async Task SeedAsync(DataSeedContext context)
        {
           using (_currentTenant.Change(context?.TenantId))
            {
                if (await _serieRepository.GetCountAsync() > 0)
                {
                    return; // Datos ya sembrados
                }


                var serie = new Serie
                {
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

                await _serieRepository.InsertAsync(serie);
                //await _context.SaveChangesAsync();
                var listaDeSeguimientoSEED = new ListaDeSeguimiento
                {
                    FechaModificacion = DateOnly.FromDateTime(DateTime.Now),
                };
                listaDeSeguimientoSEED.Series.Add(serie);
                await _listaDeSeguimientoRepository.InsertAsync(listaDeSeguimientoSEED); //No esta utilizando lo creado por el SEEDer
                //await context. .SaveChangesAsync();

                //_logger.LogInformation("Serie de prueba creada con ID: {SerieId}", serie.Id);
            }
           
        }
    }   
}
