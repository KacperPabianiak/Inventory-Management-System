namespace InventoryManagement.Api.Orders;

public sealed record CreateOrderRequest(Guid CustomerId, IReadOnlyList<OrderProductRequest> Products);

public sealed record OrderProductRequest(Guid ProductId, int Quantity);
