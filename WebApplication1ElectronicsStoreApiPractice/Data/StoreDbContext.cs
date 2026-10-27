using ElectronicsStoreApiPractice.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsStoreApiPractice.Data;

public class StoreDbContext : DbContext
{
    public DbSet<AppUser> AppUsers => Set<AppUser>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    public StoreDbContext(
        DbContextOptions<StoreDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Конфигурация категорий.
        var category = modelBuilder.Entity<Category>();

        category.ToTable("categories");

        category.HasKey(c => c.Id);

        category.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        category.Property(c => c.Name)
            .HasColumnName("name")
            .IsRequired();

        category.HasIndex(c => c.Name)
            .IsUnique();

        // Конфигурация товаров.
        var product = modelBuilder.Entity<Product>();

        product.ToTable("products", table =>
        {
            table.HasCheckConstraint(
                "CK_products_price_positive",
                "price > 0");

            table.HasCheckConstraint(
                "CK_products_quantity_nonnegative",
                "quantity >= 0");
        });

        product.HasKey(p => p.Id);

        product.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        product.Property(p => p.Name)
            .HasColumnName("name")
            .IsRequired();

        product.Property(p => p.Price)
            .HasColumnName("price")
            .HasPrecision(10, 2);

        product.HasIndex(p => p.Price);

        product.Property(p => p.Quantity)
            .HasColumnName("quantity");

        product.Property(p => p.CategoryId)
            .HasColumnName("category_id");

        product.Property(p => p.Description)
            .HasColumnName("description");

        product.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);

        // Конфигурация пользователей.
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Username)
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(u => u.NormalizedUsername)
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(u => u.PasswordHash)
                .HasMaxLength(512)
                .IsRequired();

            entity.HasIndex(u => u.NormalizedUsername)
                .IsUnique();

            entity.Property(u => u.Role)
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue("User");
        });
    }
}