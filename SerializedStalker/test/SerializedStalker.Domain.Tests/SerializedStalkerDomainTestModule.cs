using Volo.Abp.Modularity;

namespace SerializedStalker;

[DependsOn(
    typeof(SerializedStalkerDomainModule),
    typeof(SerializedStalkerTestBaseModule)
)]
public class SerializedStalkerDomainTestModule : AbpModule
{

}
