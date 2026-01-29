using Microsoft.EntityFrameworkCore;

namespace Ecommerce.CrudApi.Data;

public sealed class CrudDbContext : DbContext
{
    public CrudDbContext(DbContextOptions<CrudDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(b =>
        {
            b.ToTable("products");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Price).HasColumnType("numeric(18,2)");
        });

        modelBuilder.Entity<Order>(b =>
        {
            b.ToTable("orders");
            b.HasKey(x => x.Id);
            b.Property(x => x.Status).IsRequired().HasMaxLength(30);
            b.Property(x => x.CustomerName).IsRequired().HasMaxLength(200);
            b.Property(x => x.ShippingAddress).IsRequired().HasMaxLength(500);
            b.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.OrderId);
        });

        modelBuilder.Entity<OrderItem>(b =>
        {
            b.ToTable("order_items");
            b.HasKey(x => x.Id);
            b.Property(x => x.UnitPrice).HasColumnType("numeric(18,2)");
            b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId);
        });
    }
}