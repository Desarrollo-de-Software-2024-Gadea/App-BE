using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using SerializedStalker.Series;
using SerializedStalker.ListasDeSeguimiento;
using SerializedStalker.Domain.Notificaciones;
using System.Reflection.Emit;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Users;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using System;
using Volo.Abp.Auditing;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SerializedStalker.Usuarios;

namespace SerializedStalker.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class SerializedStalkerDbContext :
    AbpDbContext<SerializedStalkerDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    //Serie
    public DbSet<Serie> Series { get; set; }

    //Temporada
    public DbSet<Temporada> Temporadas { get; set; }

    //Episodio
    public DbSet<Episodio> Episodios { get; set; }

    //Lista de seguimiento
    public DbSet<ListaDeSeguimiento> ListasDeSeguimientos { get; set; }

    //Notificación
    public DbSet<Notificacion> Notificaciones { get; set; }

    //Calificacion
    public DbSet<Calificacion> Calificaciones { get; set; }


    //Manejo de Usuarios
    private readonly ICurrentUserService _currentUserService;

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext 
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext .
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion
   
    public SerializedStalkerDbContext(DbContextOptions<SerializedStalkerDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        //_currentUserService = this.GetService<ICurrentUserService>();
        _currentUserService = currentUserService;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //// Configuración del filtro global para CreatorId basado en el usuario actual
   //builder.Entity<Serie>().HasQueryFilter(serie => serie.CreatorId == _currentUserService.GetCurrentUserId());

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();

        /* Configure your own tables/entities inside here */
        //// Configuración del filtro global para CreatorId basado en el usuario actual
        //builder.Entity<Serie>().HasQueryFilter(serie => serie.CreatorId == _currentUserService.GetCurrentUserId());
        //Serie
        builder.Entity<Serie>(b =>
        {
            b.ToTable(SerializedStalkerConsts.DbTablePrefix + "Series",
                SerializedStalkerConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.Titulo).IsRequired().HasMaxLength(128);
            b.Property(x => x.Clasificacion).IsRequired().HasMaxLength(128);
            b.Property(x => x.FechaEstreno).IsRequired().HasMaxLength(128);
            b.Property(x => x.Duracion).IsRequired().HasMaxLength(128);
            b.Property(x => x.Generos).IsRequired().HasMaxLength(128);
            b.Property(x => x.Directores).IsRequired().HasMaxLength(128);
            b.Property(x => x.Escritores).IsRequired().HasMaxLength(128);
            b.Property(x => x.Actores).IsRequired().HasMaxLength(128);
            b.Property(x => x.Sinopsis).IsRequired().HasMaxLength(300);
            b.Property(x => x.Idiomas).IsRequired().HasMaxLength(128);
            b.Property(x => x.Pais).IsRequired().HasMaxLength(128);
            b.Property(x => x.Poster).IsRequired().HasMaxLength(128);
            b.Property(x => x.ImdbPuntuacion).IsRequired().HasMaxLength(128);
            b.Property(x => x.ImdbVotos).IsRequired(); // No aplica HasMaxLength porque es un int
            b.Property(x => x.ImdbIdentificator).IsRequired().HasMaxLength(128);
            b.Property(x => x.Tipo).IsRequired().HasMaxLength(128);
            b.Property(x => x.TotalTemporadas).IsRequired(); // No aplica HasMaxLength porque es un int
            // Relación con Temporadas
            b.HasMany(s => s.Temporadas)
             .WithOne(t => t.Serie)
             .HasForeignKey(t => t.SerieID)
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired();
            // Relación con Calificaciones
            b.HasMany(s => s.Calificaciones)
             .WithOne(c => c.Serie)
             .HasForeignKey(c => c.SerieID)
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired();
        });
        //Temporada
        builder.Entity<Temporada>(b =>
        {
            b.ToTable(SerializedStalkerConsts.DbTablePrefix + "Temporadas",
                SerializedStalkerConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.Titulo).IsRequired().HasMaxLength(128);
            b.Property(x => x.FechaLanzamiento).IsRequired().HasMaxLength(128);
            b.Property(x => x.NumeroTemporada).IsRequired();

           // Relación con Serie
            b.HasOne(t => t.Serie)
             .WithMany(s => s.Temporadas)
             .HasForeignKey(t => t.SerieID)
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired();
            // Relación con Episodios
            b.HasMany(t => t.Episodios)
             .WithOne(e => e.Temporada)
             .HasForeignKey(e => e.TemporadaID)
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired();
        });
        //Episodio
        builder.Entity<Episodio>(b =>
        {
            b.ToTable(SerializedStalkerConsts.DbTablePrefix + "Episodios",
                SerializedStalkerConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.Titulo).IsRequired().HasMaxLength(128);
            b.Property(x => x.Directores).IsRequired().HasMaxLength(128);
            b.Property(x => x.Escritores).IsRequired().HasMaxLength(128);
            b.Property(x => x.Resumen).IsRequired().HasMaxLength(128);
            b.Property(x => x.Duracion).IsRequired().HasMaxLength(128);
            b.Property(x => x.Directores).IsRequired().HasMaxLength(128);
            b.Property(x => x.FechaEstreno).IsRequired();
            b.Property(x => x.NumeroEpisodio).IsRequired();
            b.HasOne(e => e.Temporada)
             .WithMany(t => t.Episodios)
             .HasForeignKey(e => e.TemporadaID)
             .OnDelete(DeleteBehavior.Cascade);
        });

        //Lista de seguimiento
        builder.Entity<ListaDeSeguimiento>(b =>
        {
            b.ToTable(SerializedStalkerConsts.DbTablePrefix + "ListasDeSeguimiento",
                SerializedStalkerConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.FechaModificacion).IsRequired();
            b.HasMany(ls => ls.Series)
                 .WithOne();
        });

        //Notificación
        builder.Entity<Notificacion>(b =>
        {
            b.ToTable(SerializedStalkerConsts.DbTablePrefix + "Notificacion",
                SerializedStalkerConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.UsuarioId).IsRequired();
            b.Property(x => x.Titulo).IsRequired();
            b.Property(x => x.Mensaje).IsRequired();
            b.Property(x => x.Leida).IsRequired();
            b.Property(x => x.Tipo).IsRequired();
            b.Property(x => x.FechaCreacion).IsRequired();

        });

        //Califiacacion
        builder.Entity<Calificacion>(b =>
        {
            b.ToTable(SerializedStalkerConsts.DbTablePrefix + "Calificacion",
                SerializedStalkerConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.calificacion).IsRequired();
            b.Property(x => x.comentario);
            b.Property(x => x.FechaCreacion).IsRequired();
            b.Property(x => x.SerieID).IsRequired();
            b.Property(x => x.UsuarioId).IsRequired(); // Configura la propiedad UsuarioId como requerida
        });
        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(SerializedStalkerConsts.DbTablePrefix + "YourEntities", SerializedStalkerConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
    }
}
