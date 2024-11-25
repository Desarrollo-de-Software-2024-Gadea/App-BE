using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;
using static Volo.Abp.Identity.IdentityPermissions;
using SerializedStalker.Series;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace SerializedStalker.Series
{
    public class MonitoreoApiAppService: ApplicationService, IMonitoreoApiAppService
    {
        private readonly IRepository<MonitoreoApi, int> _monitoreoApiRepository;
        private readonly IObjectMapper _objectMapper;
        public MonitoreoApiAppService(IRepository<MonitoreoApi, int> repository, IObjectMapper objectMapper)
        {
            _monitoreoApiRepository = repository;
            _objectMapper = objectMapper;
        }

        /// <summary>
        /// Persiste un monitoreo en el repositorio.
        /// </summary>
        /// <param name="monitoreoApiDto">El DTO que contiene los datos del monitoreo a persistir.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        public async Task PersistirMonitoreoAsync(MonitoreoApiDto monitoreoApiDto)
        {
            var monitoreo = _objectMapper.Map<MonitoreoApiDto, MonitoreoApi>(monitoreoApiDto);
            await _monitoreoApiRepository.InsertAsync(monitoreo);
        }

        /// <summary>
        /// Obtiene todos los monitoreos almacenados en el repositorio.
        /// </summary>
        /// <returns>Un arreglo de objetos <see cref="MonitoreoApiDto"/> que representan los monitoreos almacenados.</returns>
        /// <exception cref="Exception">Lanzada cuando no hay monitoreos registrados en el repositorio.</exception>
        public async Task<MonitoreoApiDto[]> MostrarMonitoreosAsync()
        {
            //Obtenemos los monitoreos del Repositorio (No estoy 100% seguro de que esto funcione)
            var monitoreos = await _monitoreoApiRepository.GetListAsync();

            // Si no hay monitoreos da una excepción
            if (monitoreos == null)
            {
                throw new Exception("No hay Monitoreos Registrados.");
            }

            return _objectMapper.Map<MonitoreoApi[], MonitoreoApiDto[]>(monitoreos.ToArray());
        }

        /// <summary>
        /// Calcula y obtiene estadísticas de los monitoreos almacenados en el repositorio.
        /// </summary>
        /// <returns>Un objeto <see cref="MonitoreoApiEstadisticasDto"/> que contiene las estadísticas calculadas.</returns>
        /// <exception cref="Exception">Lanzada cuando no hay monitoreos registrados en el repositorio.</exception>
        public async Task<MonitoreoApiEstadisticasDto> ObtenerEstadisticasAsync()
        {
            // Reutilizamos el método MostrarMonitoreosAsync para obtener los monitoreos
            var monitoreosDto = await MostrarMonitoreosAsync();

            // Calculamos las estadísticas directamente sobre los DTOs
            var totalMonitoreos = monitoreosDto.Length;
            var promedioDuracion = monitoreosDto.Average(m => m.TiempoDuracion);
            var totalErrores = monitoreosDto.Sum(m => m.Errores.Count);

            // Creamos el DTO con las estadísticas
            var estadisticasDto = new MonitoreoApiEstadisticasDto
            {
                PromedioDuracion = promedioDuracion,
                TotalErrores = totalErrores,
                TotalMonitoreos = totalMonitoreos
            };

            return estadisticasDto;
        }
    }
}
