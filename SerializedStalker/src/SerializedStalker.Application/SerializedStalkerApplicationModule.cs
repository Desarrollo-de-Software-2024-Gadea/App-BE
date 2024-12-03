using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using SerializedStalker.Series;

namespace SerializedStalker
{
    [DependsOn(
        typeof(SerializedStalkerDomainModule)
    )]
    public class SerializedStalkerApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<SerializedStalkerApplicationModule>();
            });

            // Registrar el servicio de dominio
            context.Services.AddTransient<SerieUpdateService>();

            // Registrar el worker como un IHostedService
            context.Services.AddHostedService<SerieUpdateChecker>();

            context.Services.AddTransient<ISeriesApiService, OmdbService>();

            // Registrar el servicio de MonitoreoApiAppService
            context.Services.AddTransient<IMonitoreoApiAppService, MonitoreoApiAppService>();
        }
    }
}
