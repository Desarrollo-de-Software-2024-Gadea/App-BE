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

        public async Task PersistirMonitoreoAsync(MonitoreoApiDto monitoreoApiDto)
        {
            var monitoreo = _objectMapper.Map<MonitoreoApiDto, MonitoreoApi>(monitoreoApiDto);
            await _monitoreoApiRepository.InsertAsync(monitoreo);
        }
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
    }
}
