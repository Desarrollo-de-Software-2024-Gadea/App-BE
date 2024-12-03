using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SerializedStalker.Usuarios;

namespace SerializedStalker.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class SerializedStalkerDbContextFactory : IDesignTimeDbContextFactory<SerializedStalkerDbContext>
{
    public SerializedStalkerDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        SerializedStalkerEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<SerializedStalkerDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new SerializedStalkerDbContext(builder.Options, new FakeCurrentUserService());
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../SerializedStalker.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
