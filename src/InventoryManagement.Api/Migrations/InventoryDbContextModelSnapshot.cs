using InventoryManagement.Api.Data;
using InventoryManagement.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace InventoryManagement.Api.Migrations;

[DbContext(typeof(InventoryDbContext))]
partial class InventoryDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "8.0.11");

        modelBuilder.Entity("InventoryManagement.Api.Domain.Customer", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("TEXT");

            b.Property<string>("Name")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("TEXT");

            b.Property<CustomerRegion>("Region")
                .HasConversion<string>()
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.ToTable("Customers");
        });

        modelBuilder.Entity("InventoryManagement.Api.Domain.Order", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("TEXT");

            b.Property<Guid>("CustomerId")
                .HasColumnType("TEXT");

            b.Property<decimal>("DiscountAmount")
                .HasPrecision(18, 2)
                .HasColumnType("TEXT");

            b.Property<string>("DiscountName")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("TEXT");

            b.Property<DateTimeOffset>("OrderDate")
                .HasConversion<string>()
                .HasColumnType("TEXT");

            b.Property<CustomerRegion>("Region")
                .HasConversion<string>()
                .HasColumnType("TEXT");

            b.Property<decimal>("Subtotal")
                .HasPrecision(18, 2)
                .HasColumnType("TEXT");

            b.Property<decimal>("Total")
                .HasPrecision(18, 2)
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.ToTable("Orders");
        });

        modelBuilder.Entity("InventoryManagement.Api.Domain.OrderLine", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("TEXT");

            b.Property<decimal>("AdjustedUnitPrice")
                .HasPrecision(18, 2)
                .HasColumnType("TEXT");

            b.Property<decimal>("LineTotal")
                .HasPrecision(18, 2)
                .HasColumnType("TEXT");

            b.Property<Guid>("OrderId")
                .HasColumnType("TEXT");

            b.Property<Guid>("ProductId")
                .HasColumnType("TEXT");

            b.Property<string>("ProductName")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("TEXT");

            b.Property<int>("Quantity")
                .HasColumnType("INTEGER");

            b.Property<decimal>("UnitPrice")
                .HasPrecision(18, 2)
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.HasIndex("OrderId");

            b.ToTable("OrderLines");
        });

        modelBuilder.Entity("InventoryManagement.Api.Domain.Product", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("TEXT");

            b.Property<string>("Description")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("TEXT");

            b.Property<string>("Name")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("TEXT");

            b.Property<decimal>("Price")
                .HasPrecision(18, 2)
                .HasColumnType("TEXT");

            b.Property<int>("Stock")
                .HasColumnType("INTEGER");

            b.HasKey("Id");

            b.ToTable("Products");
        });

        modelBuilder.Entity("InventoryManagement.Api.Domain.OrderLine", b =>
        {
            b.HasOne("InventoryManagement.Api.Domain.Order", null)
                .WithMany("Lines")
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity("InventoryManagement.Api.Domain.Order", b =>
        {
            b.Navigation("Lines");
        });
#pragma warning restore 612, 618
    }
}
