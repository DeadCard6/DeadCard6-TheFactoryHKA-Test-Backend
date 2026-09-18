using EvaluacionTecnicaTheFactoryHKA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EvaluacionTecnicaTheFactoryHKA.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasSequence<int>("InvoiceNumbers")
            .StartsAt(1)
            .IncrementsBy(1);

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Clients");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DocumentType).IsRequired().HasMaxLength(10);
            entity.Property(e => e.DocumentNumber).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.DocumentNumber).IsUnique();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.RegistrationDate).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            
            entity.ToTable(t => t.HasCheckConstraint("CK_Clients_Email", "Email IS NULL OR Email LIKE '%_@_%._%'"));
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(80);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(30);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Stock).HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t => 
            {
                t.HasCheckConstraint("CK_Products_Price", "UnitPrice >= 0");
                t.HasCheckConstraint("CK_Products_Stock", "Stock >= 0");
            });
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Invoices");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
            entity.Property(e => e.IssueDate).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            entity.Property(e => e.Tax).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            entity.Property(e => e.Discount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            
            var navigation = entity.Metadata.FindNavigation(nameof(Invoice.Details));
            navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);

            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Pending");

            entity.HasOne(e => e.Client)
                  .WithMany(c => c.Invoices)
                  .HasForeignKey(e => e.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t => 
            {
                t.HasCheckConstraint("CK_Invoices_Status", "Status IN ('Pending','Paid','Voided')");
                t.HasCheckConstraint("CK_Invoices_Total", "Total >= 0");
            });
        });

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.ToTable("InvoiceDetails");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Subtotal)
                  .HasColumnType("decimal(12,2)")
                  .HasComputedColumnSql("Quantity * UnitPrice", stored: true);

            entity.HasOne(e => e.Invoice)
                  .WithMany(f => f.Details)
                  .HasForeignKey(e => e.InvoiceId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                  .WithMany(p => p.InvoiceDetails)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t => t.HasCheckConstraint("CK_InvoiceDetails_Quantity", "Quantity > 0"));
            
            entity.HasIndex(e => new { e.InvoiceId, e.ProductId }).IsUnique().HasDatabaseName("UQ_InvoiceDetails_InvoiceProduct");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired();
        });

        // Seed data
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electrónica", Description = "Dispositivos electrónicos y accesorios" },
            new Category { Id = 2, Name = "Oficina", Description = "Suministros y equipos de oficina" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Code = "ELE-001", Name = "Ratón Inalámbrico", CategoryId = 1, UnitPrice = 45000, Stock = 50, IsActive = true },
            new Product { Id = 2, Code = "ELE-002", Name = "Teclado Mecánico", CategoryId = 1, UnitPrice = 120000, Stock = 30, IsActive = true },
            new Product { Id = 3, Code = "OFI-001", Name = "Resma de Papel Carta", CategoryId = 2, UnitPrice = 15000, Stock = 100, IsActive = true }
        );

        modelBuilder.Entity<Client>().HasData(
            new Client { Id = 1, DocumentType = "CC", DocumentNumber = "1000000001", FirstName = "Juan", LastName = "Pérez", Email = "juan.perez@example.com", IsActive = true }
        );
    }
}

