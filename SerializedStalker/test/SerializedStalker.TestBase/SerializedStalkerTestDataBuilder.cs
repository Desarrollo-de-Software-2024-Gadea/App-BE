﻿using System;
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
using SerializedStalker.Domain.Notificaciones;
using SerializedStalker.Notificaciones;
using SerializedStalker.EntityFrameworkCore.Notificaciones;

namespace SerializedStalker
{
    public class SerializedStalkerTestDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly IRepository<ListaDeSeguimiento, int> _listaDeSeguimientoRepository;
        private readonly IRepository<MonitoreoApi, int> _monitoreoApiRepository;
        private readonly IRepository<Notificacion, int> _notificacionRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly SerializedStalkerDbContext _context;
        private readonly ILogger<SerializedStalkerTestDataSeedContributor> _logger;


        public SerializedStalkerTestDataSeedContributor(
            IRepository<Serie, int> serieRepository, 
            IRepository<ListaDeSeguimiento, int> listaDeSeguimientoRepository,
            IRepository<MonitoreoApi, int> monitoreoApiRepository,
            IRepository<Notificacion, int> notificacionRepository,
            ICurrentTenant currentTenant, SerializedStalkerDbContext context, 
            ILogger<SerializedStalkerTestDataSeedContributor> logger)
        {
            _serieRepository = serieRepository;
            _listaDeSeguimientoRepository = listaDeSeguimientoRepository;
            _monitoreoApiRepository = monitoreoApiRepository;
            _notificacionRepository = notificacionRepository;
            _currentTenant = currentTenant;
            _context = context;
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
                //await _context.SaveChangesAsync();
                var monitoreoSEED1 = new MonitoreoApi
                {
                    HoraEntrada = DateTime.Now,
                    HoraSalida = DateTime.Now.AddSeconds(20),
                    TiempoDuracion = 20,
                    Errores = new List<string> { "Error 7" }
                };
                await _monitoreoApiRepository.InsertAsync(monitoreoSEED1);

                var monitoreoSEED2 = new MonitoreoApi
                {
                    HoraEntrada = DateTime.Now,
                    HoraSalida = DateTime.Now.AddMinutes(30),
                    TiempoDuracion = 30,
                    Errores = new List<string> { "Error 1" }
                };
                await _monitoreoApiRepository.InsertAsync(monitoreoSEED2);

                var monitoreoSEED3 = new MonitoreoApi
                {
                    HoraEntrada = DateTime.Now,
                    HoraSalida = DateTime.Now.AddMinutes(45),
                    TiempoDuracion = 45,
                    Errores = new List<string> { "Error 2", "Error 3" }
                };
                await _monitoreoApiRepository.InsertAsync(monitoreoSEED3);

                var notificacion1 = new Notificacion
                {
                    UsuarioId = 1,
                    Titulo = "Titulo X0",
                    Mensaje = "Nuevo episodio que no vas a ver",
                    Leida = false
                };
                await _notificacionRepository.InsertAsync(notificacion1);

                //_logger.LogInformation("Serie de prueba creada con ID: {SerieId}", serie.Id);
            }

        }
    }   
}
