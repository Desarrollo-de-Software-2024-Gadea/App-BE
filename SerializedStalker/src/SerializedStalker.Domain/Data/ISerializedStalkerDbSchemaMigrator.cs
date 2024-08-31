using System.Threading.Tasks;

namespace SerializedStalker.Data;

public interface ISerializedStalkerDbSchemaMigrator
{
    Task MigrateAsync();
}
