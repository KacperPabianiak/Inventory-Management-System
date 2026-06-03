using InventoryManagement.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Data;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Name).IsRequired().HasMaxLength(50);
            entity.Property(product => product.Description).IsRequired().HasMaxLength(50);
            entity.Property(product => product.Price).HasPrecision(18, 2);
            entity.Property(product => product.Stock).IsRequired();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(customer => customer.Id);
            entity.Property(customer => customer.Name).IsRequired().HasMaxLength(50);
            entity.Property(customer => customer.Region).HasConversion<string>().IsRequired();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(order => order.Id);
            entity.Property(order => order.Region).HasConversion<string>().IsRequired();
            entity.Property(order => order.Subtotal).HasPrecision(18, 2);
            entity.Property(order => order.DiscountAmount).HasPrecision(18, 2);
            entity.Property(order => order.Total).HasPrecision(18, 2);
            entity.Property(order => order.DiscountName).IsRequired().HasMaxLength(50);
            entity.Property(order => order.OrderDate).HasConversion<string>().HasColumnType("TEXT");
            entity.HasMany(order => order.Lines).WithOne().HasForeignKey(line => line.OrderId);
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.HasKey(line => line.Id);
            entity.Property(line => line.ProductName).IsRequired().HasMaxLength(50);
            entity.Property(line => line.UnitPrice).HasPrecision(18, 2);
            entity.Property(line => line.AdjustedUnitPrice).HasPrecision(18, 2);
            entity.Property(line => line.LineTotal).HasPrecision(18, 2);
        });
    }
}
