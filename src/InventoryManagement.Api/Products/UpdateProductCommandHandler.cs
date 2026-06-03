using InventoryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Products;

public sealed class UpdateProductCommandHandler(InventoryDbContext db)
{
    public async Task<ProductResponse?> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await db.Products
            .SingleOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (product is null)
        {
            return null;
        }

        product.Name = command.Name.Trim();
        product.Description = command.Description.Trim();
        product.Price = command.Price;
        product.Stock = command.Stock;

        await db.SaveChangesAsync(cancellationToken);

        return ProductResponse.FromProduct(product);
    }
}
