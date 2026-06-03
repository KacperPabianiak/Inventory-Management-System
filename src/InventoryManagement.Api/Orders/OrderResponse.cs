using InventoryManagement.Api.Domain;

namespace InventoryManagement.Api.Orders;

public record OrderResponse(
    Guid Id,
    Guid CustomerId,
    CustomerRegion Region,
    DateTimeOffset OrderDate,
    decimal Subtotal,
    decimal DiscountAmount,
    string DiscountName,
    decimal Total,
    IReadOnlyList<OrderLineResponse> Lines)
{
    public static OrderResponse FromOrder(Order order)
    {
        return new OrderResponse(
            order.Id,
            order.CustomerId,
            order.Region,
            order.OrderDate,
            order.Subtotal,
            order.DiscountAmount,
            order.DiscountName,
            order.Total,
            order.Lines.Select(OrderLineResponse.FromOrderLine).ToArray());
    }
}

public sealed record OrderLineResponse(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal AdjustedUnitPrice,
    decimal LineTotal)
{
    public static OrderLineResponse FromOrderLine(OrderLine line)
    {
        return new OrderLineResponse(
            line.ProductId,
            line.ProductName,
            line.Quantity,
            line.UnitPrice,
            line.AdjustedUnitPrice,
            line.LineTotal);
    }
}
