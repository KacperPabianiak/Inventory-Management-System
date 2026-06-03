using InventoryManagement.Api.Data;
using InventoryManagement.Api.Domain;

namespace InventoryManagement.Api.Products;

public sealed class CreateProductCommandHandler(InventoryDbContext db)
{
    public async Task<ProductResponse> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = new Product(
            Guid.NewGuid(),
            command.Name.Trim(),
            command.Description.Trim(),
            command.Price,
            command.Stock);

        db.Products.Add(product);
        await db.SaveChangesAsync(cancellationToken);

        return ProductResponse.FromProduct(product);
    }
}
