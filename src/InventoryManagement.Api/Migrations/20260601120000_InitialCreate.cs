using InventoryManagement.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.Api.Migrations;

[DbContext(typeof(InventoryDbContext))]
[Migration("20260601120000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Customers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Region = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Customers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Orders",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                Region = table.Column<string>(type: "TEXT", nullable: false),
                OrderDate = table.Column<string>(type: "TEXT", nullable: false),
                Subtotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                DiscountAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                DiscountName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Total = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Orders", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Description = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                Stock = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OrderLines",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                OrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                ProductName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                UnitPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                AdjustedUnitPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                LineTotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderLines", x => x.Id);
                table.ForeignKey(
                    name: "FK_OrderLines_Orders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "Orders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_OrderLines_OrderId",
            table: "OrderLines",
            column: "OrderId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Customers");

        migrationBuilder.DropTable(
            name: "OrderLines");

        migrationBuilder.DropTable(
            name: "Products");

        migrationBuilder.DropTable(
            name: "Orders");
    }
}
