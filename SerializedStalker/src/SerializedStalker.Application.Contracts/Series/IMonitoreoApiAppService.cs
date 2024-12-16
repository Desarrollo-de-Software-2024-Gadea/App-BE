using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using SerializedStalker.Series;

namespace SerializedStalker.Series
{
    public interface IMonitoreoApiAppService : IApplicationService
    {
        Task PersistirMonitoreoAsync(MonitoreoApiDto monitoreoDto);
        Task<MonitoreoApiDto[]> MostrarMonitoreosAsync();
        Task<MonitoreoApiEstadisticasDto> ObtenerEstadisticasAsync();
        Task<MonitoreoApiDto> IniciarMonitoreo();
        Task<MonitoreoApiDto> FinalizarMonitoreo(MonitoreoApiDto monitoreo);
        Task<MonitoreoApiDto> ErrorMonitoreo(MonitoreoApiDto monitoreo, Exception ex);
    }
}