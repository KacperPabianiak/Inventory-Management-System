namespace InventoryManagement.Api.Products;

public sealed record CreateProductCommand(string Name, string Description, decimal Price, int Stock);
