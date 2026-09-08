using Microsoft.EntityFrameworkCore;
using Transportes_Orellana.Models;

namespace Transportes_Orellana.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Motorista> Motoristas { get; set; }
    public DbSet<UnidadTransporte> UnidadesTransporte { get; set; }
    public DbSet<Flete> Fletes { get; set; }
    public DbSet<GastoFlete> GastosFletes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CLIENTE
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("cliente");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .HasColumnName("cliente_id");
            entity.Property(e => e.Nombre)
                  .HasColumnName("nombre")
                  .HasMaxLength(100);
            entity.Property(e => e.Apellido)
                  .HasColumnName("apellido")
                  .HasMaxLength(100);
            entity.Property(e => e.Correo)
                  .HasColumnName("correo")
                  .HasMaxLength(254);
            entity.Property(e => e.Telefono)
                  .HasColumnName("telefono")
                  .HasMaxLength(15);
            entity.Property(e => e.Direccion)
                  .HasColumnName("direccion")
                  .HasMaxLength(500);
            entity.Property(e => e.Estado)
                  .HasColumnName("estado")
                  .HasMaxLength(20);         
            entity.Property(e => e.FechaRegistro)
                  .HasColumnName("fecha_registro");
        });

        // MOTORISTA
        modelBuilder.Entity<Motorista>(entity =>
        {
            entity.ToTable("motorista");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .HasColumnName("motorista_id");
            entity.Property(e => e.Nombre)
                  .HasColumnName("nombre")
                  .HasMaxLength(100);
            entity.Property(e => e.Apellido)
                  .HasColumnName("apellido")
                  .HasMaxLength(100);
            entity.Property(e => e.Direccion)
                  .HasColumnName("direccion")
                  .HasMaxLength(500);
            entity.Property(e => e.Correo)
                  .HasColumnName("correo")
                  .HasMaxLength(254);
            entity.Property(e => e.Telefono)
                  .HasColumnName("telefono")
                  .HasMaxLength(15);
            entity.Property(e => e.PerfilSocial)
                  .HasColumnName("perfil_social")
                  .HasMaxLength(2048);
            entity.Property(e => e.Curriculum)
                  .HasColumnName("curriculum")
                  .HasMaxLength(2048);
            entity.Property(e => e.Estado)
                  .HasColumnName("estado")
                  .HasMaxLength(20);        
            entity.Property(e => e.FechaRegistro)
                  .HasColumnName("fecha_registro");
        });

        // UNIDAD TRANSPORTE
        modelBuilder.Entity<UnidadTransporte>(entity =>
        {
            entity.ToTable("unidad_transporte");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .HasColumnName("unidad_id");
            entity.Property(e => e.Placa)
                  .HasColumnName("placa")
                  .HasMaxLength(8);
            entity.Property(e => e.Marca)
                  .HasColumnName("marca")
                  .HasMaxLength(25);
            entity.Property(e => e.CapacidadToneladas)
                  .HasColumnName("capacidad_toneladas")
                  .HasColumnType("numeric(6,2)");
            entity.Property(e => e.AnioFabricacion)
                  .HasColumnName("anio_fabricacion");
            entity.Property(e => e.Estado)
                  .HasColumnName("estado")
                  .HasMaxLength(20);         
            entity.Property(e => e.FechaRegistro)
                  .HasColumnName("fecha_registro");
        });

        // FLETE
        modelBuilder.Entity<Flete>(entity =>
        {
            entity.ToTable("flete");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .HasColumnName("flete_id");
            entity.Property(e => e.ClienteId)
                  .HasColumnName("cliente_id");
            entity.Property(e => e.UnidadId)
                  .HasColumnName("unidad_id");
            entity.Property(e => e.MotoristaId)
                  .HasColumnName("motorista_id");
            entity.Property(e => e.Descripcion)
                  .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                  .HasColumnName("estado")
                  .HasMaxLength(20);         
            entity.Property(e => e.MontoCobro)
                  .HasColumnName("monto_cobro")
                  .HasColumnType("numeric(10,2)");
            entity.Property(e => e.LugarRecolecta)
                  .HasColumnName("lugar_recolecta")
                  .HasMaxLength(200);
            entity.Property(e => e.LugarEntrega)
                  .HasColumnName("lugar_entrega")
                  .HasMaxLength(200);
            entity.Property(e => e.HoraSalida)
                  .HasColumnName("hora_salida");
            entity.Property(e => e.HoraDestino)
                  .HasColumnName("hora_destino");
            entity.Property(e => e.Consideraciones)
                  .HasColumnName("consideraciones");
            entity.Property(e => e.FechaRegistro)
                  .HasColumnName("fecha_registro");

            // Relaciones
            entity.HasOne(e => e.Cliente)
                  .WithMany(c => c.Fletes)
                  .HasForeignKey(e => e.ClienteId);
            entity.HasOne(e => e.Unidad)
                  .WithMany(u => u.Fletes)
                  .HasForeignKey(e => e.UnidadId);
            entity.HasOne(e => e.Motorista)
                  .WithMany(m => m.Fletes)
                  .HasForeignKey(e => e.MotoristaId);
        });

        // GASTO FLETE 
        modelBuilder.Entity<GastoFlete>(entity =>
        {
            entity.ToTable("gasto_flete");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .HasColumnName("gasto_id");
            entity.Property(e => e.FleteId)
                  .HasColumnName("flete_id");
            entity.Property(e => e.TipoGasto)
                  .HasColumnName("tipo_gasto")
                  .HasMaxLength(20);         
            entity.Property(e => e.Concepto)
                  .HasColumnName("concepto")
                  .HasMaxLength(100);
            entity.Property(e => e.Monto)
                  .HasColumnName("monto")
                  .HasColumnType("numeric(10,2)");
            entity.Property(e => e.FechaRegistro)
                  .HasColumnName("fecha_registro");

            // Relacion
            entity.HasOne(e => e.Flete)
                  .WithMany(f => f.Gastos)
                  .HasForeignKey(e => e.FleteId);
        });
    }
}