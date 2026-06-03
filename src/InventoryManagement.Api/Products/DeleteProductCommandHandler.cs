using InventoryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Products;

public sealed class DeleteProductCommandHandler(InventoryDbContext db)
{
    public async Task<bool> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var deleted = await db.Products
            .Where(product => product.Id == command.Id)
            .ExecuteDeleteAsync(cancellationToken);

        return deleted > 0;
    }
}
