using Volo.Abp.Modularity;

namespace SerializedStalker;

/* Inherit from this class for your domain layer tests. */
public abstract class SerializedStalkerDomainTestBase<TStartupModule> : SerializedStalkerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
