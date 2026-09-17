using EvaluacionTecnicaTheFactoryHKA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EvaluacionTecnicaTheFactoryHKA.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Categoria> Categorias { get; set; } = null!;
    public DbSet<Producto> Productos { get; set; } = null!;
    public DbSet<Factura> Facturas { get; set; } = null!;
    public DbSet<DetalleFactura> DetalleFacturas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TipoDocumento).IsRequired().HasMaxLength(10);
            entity.Property(e => e.NumeroDocumento).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.NumeroDocumento).IsUnique();
            entity.Property(e => e.Nombres).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            
            entity.ToTable(t => t.HasCheckConstraint("CK_Clientes_Email", "Email IS NULL OR Email LIKE '%_@_%._%'"));
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categorias");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(80);
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.Property(e => e.Descripcion).HasMaxLength(255);
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Productos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(30);
            entity.HasIndex(e => e.Codigo).IsUnique();
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Stock).HasDefaultValue(0);
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(e => e.Categoria)
                  .WithMany(c => c.Productos)
                  .HasForeignKey(e => e.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t => 
            {
                t.HasCheckConstraint("CK_Productos_Precio", "PrecioUnitario >= 0");
                t.HasCheckConstraint("CK_Productos_Stock", "Stock >= 0");
            });
            
            entity.HasIndex(e => e.CategoriaId).HasDatabaseName("IX_Productos_Categoria");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.ToTable("Facturas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroFactura).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.NumeroFactura).IsUnique();
            entity.Property(e => e.FechaEmision).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12,2)").HasDefaultValue(0);
            entity.Property(e => e.Impuesto).HasColumnType("decimal(12,2)").HasDefaultValue(0);
            entity.Property(e => e.Descuento).HasColumnType("decimal(12,2)").HasDefaultValue(0);
            entity.Property(e => e.Total).HasColumnType("decimal(12,2)").HasDefaultValue(0);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(20).HasDefaultValue("Pendiente");

            entity.HasOne(e => e.Cliente)
                  .WithMany(c => c.Facturas)
                  .HasForeignKey(e => e.ClienteId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t => 
            {
                t.HasCheckConstraint("CK_Facturas_Estado", "Estado IN ('Pendiente','Pagada','Anulada')");
                t.HasCheckConstraint("CK_Facturas_Total", "Total >= 0");
            });
            
            entity.HasIndex(e => e.ClienteId).HasDatabaseName("IX_Facturas_Cliente");
            entity.HasIndex(e => e.FechaEmision).HasDatabaseName("IX_Facturas_Fecha");
        });

        modelBuilder.Entity<DetalleFactura>(entity =>
        {
            entity.ToTable("DetalleFactura");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Subtotal)
                  .HasColumnType("decimal(12,2)")
                  .HasComputedColumnSql("Cantidad * PrecioUnitario", stored: true);

            entity.HasOne(e => e.Factura)
                  .WithMany(f => f.Detalles)
                  .HasForeignKey(e => e.FacturaId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Producto)
                  .WithMany(p => p.DetallesFactura)
                  .HasForeignKey(e => e.ProductoId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t => t.HasCheckConstraint("CK_Detalle_Cantidad", "Cantidad > 0"));
            
            entity.HasIndex(e => new { e.FacturaId, e.ProductoId }).IsUnique().HasDatabaseName("UQ_Detalle_FacturaProducto");
            entity.HasIndex(e => e.FacturaId).HasDatabaseName("IX_Detalle_Factura");
            entity.HasIndex(e => e.ProductoId).HasDatabaseName("IX_Detalle_Producto");
        });

        // Seed data
        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Electrónica", Descripcion = "Dispositivos y accesorios electrónicos" },
            new Categoria { Id = 2, Nombre = "Oficina", Descripcion = "Insumos y equipos de oficina" }
        );

        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Codigo = "ELE-001", Nombre = "Mouse inalámbrico", CategoriaId = 1, PrecioUnitario = 45000, Stock = 50, Activo = true },
            new Producto { Id = 2, Codigo = "ELE-002", Nombre = "Teclado mecánico", CategoriaId = 1, PrecioUnitario = 120000, Stock = 30, Activo = true },
            new Producto { Id = 3, Codigo = "OFI-001", Nombre = "Resma de papel carta", CategoriaId = 2, PrecioUnitario = 15000, Stock = 100, Activo = true }
        );

        modelBuilder.Entity<Cliente>().HasData(
            new Cliente { Id = 1, TipoDocumento = "CC", NumeroDocumento = "1000000001", Nombres = "Juan", Apellidos = "Pérez", Email = "juan.perez@correo.com", Activo = true }
        );
    }
}
