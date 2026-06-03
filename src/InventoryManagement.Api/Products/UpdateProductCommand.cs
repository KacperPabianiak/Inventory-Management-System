namespace InventoryManagement.Api.Products;

public sealed record UpdateProductCommand(Guid Id, string Name, string Description, decimal Price, int Stock);
