using InventoryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Products;

public sealed class GetProductByIdQueryHandler(InventoryDbContext db)
{
    public async Task<ProductResponse?> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await db.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == query.Id, cancellationToken);

        return product is null ? null : ProductResponse.FromProduct(product);
    }
}
