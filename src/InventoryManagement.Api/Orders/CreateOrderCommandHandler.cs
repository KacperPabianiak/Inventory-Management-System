using InventoryManagement.Api.Common;
using InventoryManagement.Api.Data;
using InventoryManagement.Api.Domain;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace InventoryManagement.Api.Orders;

public sealed class CreateOrderCommandHandler(InventoryDbContext db, TimeProvider dateProvider)
{
    private static readonly TimeZoneInfo BusinessTimeZone = ResolveBusinessTimeZone();

    public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var customer = await db.Customers
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == command.CustomerId, cancellationToken);

        if (customer is null)
        {
            return CreateOrderResult.CustomerNotFound();
        }

        var requestedQuantities = command.Products
            .GroupBy(product => product.ProductId)
            .ToDictionary(group => group.Key, group => group.Sum(product => product.Quantity));

        var productIds = requestedQuantities.Keys.ToArray();
        var products = await db.Products
            .Where(product => productIds.Contains(product.Id))
            .ToListAsync(cancellationToken);

        var productsById = products.ToDictionary(product => product.Id);
        var missingProductId = productIds.FirstOrDefault(productId => !productsById.ContainsKey(productId));
        if (missingProductId != Guid.Empty)
        {
            return CreateOrderResult.ProductNotFound(missingProductId);
        }

        var orderDate = GetBusinessDate(dateProvider);
        var orderId = Guid.NewGuid();
        var pricing = OrderPricing.Calculate(
            productsById.Values,
            requestedQuantities,
            customer.Region,
            orderDate);

        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        foreach (var (productId, quantity) in requestedQuantities)
        {
            var updated = await db.Products
                .Where(product => product.Id == productId && product.Stock >= quantity)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(product => product.Stock, product => product.Stock - quantity),
                    cancellationToken);

            if (updated == 0)
            {
                await transaction.RollbackAsync(cancellationToken);

                var currentProduct = await db.Products
                    .AsNoTracking()
                    .SingleOrDefaultAsync(product => product.Id == productId, cancellationToken);

                if (currentProduct is null)
                {
                    return CreateOrderResult.ProductNotFound(productId);
                }

                return CreateOrderResult.InsufficientStock(
                    currentProduct.Name,
                    quantity,
                    currentProduct.Stock);
            }
        }

        var lines = pricing.Lines
            .Select(line => new OrderLine(
                Guid.NewGuid(),
                orderId,
                line.ProductId,
                line.ProductName,
                line.Quantity,
                line.UnitPrice,
                line.AdjustedUnitPrice,
                line.LineTotal))
            .ToList();

        var order = new Order(
            orderId,
            customer.Id,
            customer.Region,
            orderDate,
            pricing.Subtotal,
            pricing.DiscountAmount,
            pricing.DiscountName,
            pricing.Total,
            lines);

        db.Orders.Add(order);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return CreateOrderResult.Created(OrderResponse.FromOrder(order));
    }

    private static DateTimeOffset GetBusinessDate(TimeProvider timeProvider)
    {
        var businessNow = TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), BusinessTimeZone);
        return new DateTimeOffset(
            businessNow.Year,
            businessNow.Month,
            businessNow.Day,
            0,
            0,
            0,
            businessNow.Offset);
    }

    private static TimeZoneInfo ResolveBusinessTimeZone()
    {
        foreach (var id in new[] { "Europe/Warsaw", "Central European Standard Time" })
        {
            if (TimeZoneInfo.TryFindSystemTimeZoneById(id, out var timeZone))
            {
                return timeZone;
            }
        }

        return TimeZoneInfo.Local;
    }
}
