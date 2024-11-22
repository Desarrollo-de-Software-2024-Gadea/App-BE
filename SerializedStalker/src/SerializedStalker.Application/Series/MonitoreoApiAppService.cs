using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace SerializedStalker.Series
{
    public class MonitoreoApiAppService: IMonitoreoApiAppService
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
    }
}
