namespace InventoryManagement.Api.Orders;

public sealed record CreateOrderCommand(Guid CustomerId, IReadOnlyList<OrderProductRequest> Products);
