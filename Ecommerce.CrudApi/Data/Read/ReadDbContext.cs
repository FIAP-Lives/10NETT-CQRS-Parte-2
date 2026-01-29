using Ecommerce.CrudApi.Data.Read.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.CrudApi.Data.Read;

public sealed class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }

    public DbSet<OrderReadModel> OrdersRead => Set<OrderReadModel>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderReadModel>(b =>
        {
            b.ToTable("orders_read");
            b.HasKey(x => x.OrderId);

            b.Property(x => x.CustomerName).IsRequired().HasMaxLength(200);
            b.Property(x => x.ShippingAddress).IsRequired().HasMaxLength(200);
            b.Property(x => x.Status).IsRequired().HasMaxLength(30);
            b.Property(x => x.Total).HasColumnType("numeric(18,2)");
            b.Property(x => x.ItemsJson).IsRequired();
        });
    }
}