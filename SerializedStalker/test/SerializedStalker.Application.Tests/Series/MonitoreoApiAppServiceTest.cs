using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Internal.Mappers;
using Microsoft.EntityFrameworkCore;
using SerializedStalker.EntityFrameworkCore;
using SerializedStalker.Series;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace SerializedStalker.Series
{
    public abstract class MonitoreoApiAppServiceTest<TStartupModule> : SerializedStalkerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IMonitoreoApiAppService _monitoreoApiAppService;
        private readonly SerializedStalkerDbContext _dbContext;

        protected MonitoreoApiAppServiceTest()
        {
            _monitoreoApiAppService = GetRequiredService<IMonitoreoApiAppService>();
            _dbContext = GetRequiredService<SerializedStalkerDbContext>();
        }

        /// <summary>
        /// Prueba unitaria para verificar que el método PersistirMonitoreoAsync de MonitoreoApiAppService
        /// llama al método InsertAsync del repositorio y persiste correctamente un objeto MonitoreoApi en la base de datos.
        /// </summary>
        /// <remarks>
        /// Esta prueba crea una instancia de MonitoreoApiDto con datos de prueba, llama al método PersistirMonitoreoAsync
        /// y luego verifica que el objeto MonitoreoApi correspondiente ha sido guardado en la base de datos.
        /// </remarks>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        [Fact]
        public async Task PersistirMonitoreoAsync_Should_Call_InsertAsync()
        {
            // Arrange
            var monitoreoDto = new MonitoreoApiDto
            {
                HoraEntrada = DateTime.Now,
                HoraSalida = DateTime.Now.AddMinutes(1),
                TiempoDuracion = 60,
                Errores = new List<string> { "Error de prueba" }
            };

            // Act
            await _monitoreoApiAppService.PersistirMonitoreoAsync(monitoreoDto);

            // Assert
            var monitoreoEnDb = await _dbContext.MonitoreosApi
                .FirstOrDefaultAsync(m => m.TiempoDuracion == 60);

            monitoreoEnDb.ShouldNotBeNull(); // Verifica que el monitoreo fue guardado
            monitoreoEnDb.TiempoDuracion.ShouldBe(60); // Verifica que los datos coinciden

        }

        [Fact]
        public async Task MostrarMonitoreosAsync_Should_Show_Monitoreos()
        {
            //Act
            var monitoreoApiDto = await _monitoreoApiAppService.MostrarMonitoreosAsync();

            //Assert
            Assert.NotEmpty(monitoreoApiDto);
            //.FirstOrDefault(s => s.ImdbIdentificator == "tt1234567"); //Falso positivo
        }
    }
}
