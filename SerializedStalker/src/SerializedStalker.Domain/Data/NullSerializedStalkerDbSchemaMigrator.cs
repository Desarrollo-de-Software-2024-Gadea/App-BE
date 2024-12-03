using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SerializedStalker.Data;

/* This is used if database provider does't define
 * ISerializedStalkerDbSchemaMigrator implementation.
 */
public class NullSerializedStalkerDbSchemaMigrator : ISerializedStalkerDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
