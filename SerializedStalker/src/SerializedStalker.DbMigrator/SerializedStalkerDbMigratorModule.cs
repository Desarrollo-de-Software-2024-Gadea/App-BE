using SerializedStalker.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SerializedStalker.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(SerializedStalkerEntityFrameworkCoreModule),
    typeof(SerializedStalkerApplicationContractsModule)
)]
public class SerializedStalkerDbMigratorModule : AbpModule
{
}
