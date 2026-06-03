namespace InventoryManagement.Api.Orders;

public sealed record CreateOrderResult(CreateOrderStatus Status, OrderResponse? Order = null, string? ErrorMessage = null)
{
    public static CreateOrderResult Created(OrderResponse order)
    {
        return new CreateOrderResult(CreateOrderStatus.Created, order);
    }

    public static CreateOrderResult CustomerNotFound()
    {
        return new CreateOrderResult(CreateOrderStatus.CustomerNotFound);
    }

    public static CreateOrderResult ProductNotFound(Guid productId)
    {
        return new CreateOrderResult(
            CreateOrderStatus.ProductNotFound,
            ErrorMessage: $"Product '{productId}' was not found.");
    }

    public static CreateOrderResult InsufficientStock(string productName, int requestedQuantity, int availableStock)
    {
        return new CreateOrderResult(
            CreateOrderStatus.InsufficientStock,
            ErrorMessage: $"Product '{productName}' has insufficient stock. Requested {requestedQuantity}, available {availableStock}.");
    }
}

public enum CreateOrderStatus
{
    Created,
    CustomerNotFound,
    ProductNotFound,
    InsufficientStock
}
