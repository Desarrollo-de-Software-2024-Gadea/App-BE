using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SerializedStalker.Data;
using Volo.Abp.DependencyInjection;

namespace SerializedStalker.EntityFrameworkCore;

public class EntityFrameworkCoreSerializedStalkerDbSchemaMigrator
    : ISerializedStalkerDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreSerializedStalkerDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the SerializedStalkerDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<SerializedStalkerDbContext>()
            .Database
            .MigrateAsync();
    }
}
