using DAW_Procesos_Fabricacion_KevinRodriguez.Models;
using Microsoft.EntityFrameworkCore;

namespace DAW_Procesos_Fabricacion_KevinRodriguez.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<OrdenProduccion> OrdenesProduccion => Set<OrdenProduccion>();

    public DbSet<ProcesoFabricacion> ProcesosFabricacion => Set<ProcesoFabricacion>();

    public DbSet<OrdenProceso> OrdenesProcesos => Set<OrdenProceso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrdenProduccion>(entity =>
        {
            entity.ToTable("OrdenesProduccion");
            entity.Property(orden => orden.NumeroOrden).UseCollation("NOCASE");
            entity.HasIndex(orden => orden.NumeroOrden).IsUnique();
        });

        modelBuilder.Entity<ProcesoFabricacion>(entity =>
        {
            entity.ToTable("ProcesosFabricacion");
            entity.Property(proceso => proceso.Nombre).UseCollation("NOCASE");
            entity.HasIndex(proceso => proceso.Nombre).IsUnique();
        });

        modelBuilder.Entity<OrdenProceso>(entity =>
        {
            entity.ToTable("OrdenesProcesos");
            entity.HasKey(relacion => new
            {
                relacion.OrdenProduccionId,
                relacion.ProcesoFabricacionId
            });

            entity.Property(relacion => relacion.Estado)
                .HasConversion<string>()
                .HasDefaultValue(EstadoProceso.Pendiente);

            entity.HasOne(relacion => relacion.OrdenProduccion)
                .WithMany(orden => orden.Procesos)
                .HasForeignKey(relacion => relacion.OrdenProduccionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(relacion => relacion.ProcesoFabricacion)
                .WithMany(proceso => proceso.Ordenes)
                .HasForeignKey(relacion => relacion.ProcesoFabricacionId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
