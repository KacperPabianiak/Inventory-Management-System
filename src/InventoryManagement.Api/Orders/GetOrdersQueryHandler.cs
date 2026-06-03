using InventoryManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Orders;

public sealed class GetOrdersQueryHandler(InventoryDbContext db)
{
    public async Task<IReadOnlyList<OrderResponse>> Handle(GetOrdersQuery _, CancellationToken cancellationToken)
    {
        var orders = await db.Orders
            .AsNoTracking()
            .Include(order => order.Lines)
            .OrderByDescending(order => order.OrderDate)
            .ThenBy(order => order.Id)
            .ToListAsync(cancellationToken);

        return orders.Select(OrderResponse.FromOrder).ToArray();
    }
}
