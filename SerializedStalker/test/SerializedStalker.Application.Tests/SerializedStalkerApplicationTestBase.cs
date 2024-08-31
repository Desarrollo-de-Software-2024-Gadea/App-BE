using Volo.Abp.Modularity;

namespace SerializedStalker;

public abstract class SerializedStalkerApplicationTestBase<TStartupModule> : SerializedStalkerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
