using InventoryManagement.Api.Domain;

namespace InventoryManagement.Api.Products;

public sealed record ProductResponse(Guid Id, string Name, string Description, decimal Price, int Stock)
{
    public static ProductResponse FromProduct(Product product)
    {
        return new ProductResponse(product.Id, product.Name, product.Description, product.Price, product.Stock);
    }
}
