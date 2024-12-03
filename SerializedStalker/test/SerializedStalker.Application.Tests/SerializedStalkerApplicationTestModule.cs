using Volo.Abp.Modularity;

namespace SerializedStalker;

[DependsOn(
    typeof(SerializedStalkerApplicationModule),
    typeof(SerializedStalkerDomainTestModule)
)]
public class SerializedStalkerApplicationTestModule : AbpModule
{

}
