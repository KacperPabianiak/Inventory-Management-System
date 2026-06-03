using InventoryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Orders;

public sealed class GetOrderByIdQueryHandler(InventoryDbContext db)
{
    public async Task<OrderResponse?> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Lines)
            .SingleOrDefaultAsync(o => o.Id == query.Id, cancellationToken);

        return order is null ? null : OrderResponse.FromOrder(order);
    }
}
