using InventoryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Products;

public sealed class GetProductsQueryHandler(InventoryDbContext db)
{
    public async Task<IReadOnlyList<ProductResponse>> Handle(GetProductsQuery _, CancellationToken cancellationToken)
    {
        return await db.Products
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .Select(product => ProductResponse.FromProduct(product))
            .ToListAsync(cancellationToken);
    }
}
