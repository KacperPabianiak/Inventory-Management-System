using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Api.Data;
using InventoryManagement.Api.Orders;
using InventoryManagement.Api.Products;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.IntegrationTests;

public sealed class InventoryEndpointTests
{
    [Fact]
    public async Task CreateProduct_ThenGetProducts_ReturnsCreatedProduct()
    {
        await using var factory = new InventoryApiFactory(new DateTimeOffset(new DateTime(2026, 6, 2)));
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/products", new CreateProductRequest(
            "Keyboard",
            "Mechanical keyboard",
            199.99m,
            12), cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(created);

        var products = await client.GetFromJsonAsync<ProductResponse[]>("/products", cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(products);
        var product = Assert.Single(products);
        Assert.Equal(created!.Id, product.Id);
        Assert.Equal("Keyboard", product.Name);
        Assert.Equal(199.99m, product.Price);
        Assert.Equal(12, product.Stock);
    }

    [Fact]
    public async Task ProductCrud_WhenProductExists_CanReadUpdateAndDeleteIt()
    {
        await using var factory = new InventoryApiFactory(new DateTimeOffset(new DateTime(2026, 6, 2)));
        using var client = factory.CreateClient();

        var product = await CreateProduct(client, "Desk", 250m, 3);

        var read = await client.GetFromJsonAsync<ProductResponse>($"/products/{product.Id}", cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(read);
        Assert.Equal("Desk", read!.Name);

        var updateResponse = await client.PutAsJsonAsync($"/products/{product.Id}", new UpdateProductRequest(
            "Standing desk",
            "Adjustable desk",
            300m,
            4), cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<ProductResponse>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(updated);
        Assert.Equal("Standing desk", updated!.Name);
        Assert.Equal(300m, updated.Price);
        Assert.Equal(4, updated.Stock);

        var deleteResponse = await client.DeleteAsync($"/products/{product.Id}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var missingResponse = await client.GetAsync($"/products/{product.Id}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_WhenStockAvailable_UpdatesStockAndReturnsPricedOrder()
    {
        await using var factory = new InventoryApiFactory(new DateTimeOffset(new DateTime(2026, 6, 2)));
        using var client = factory.CreateClient();

        var product = await CreateProduct(client, "Monitor", 100m, 10);

        var orderResponse = await client.PostAsJsonAsync("/orders", new CreateOrderRequest(
            InventoryDbContextSeed.EuropeCustomerId,
            [new OrderProductRequest(product.Id, 5)]), cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);
        var order = await orderResponse.Content.ReadFromJsonAsync<OrderResponse>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(order);
        Assert.Equal(575m, order!.Subtotal);
        Assert.Equal("Volume", order.DiscountName);
        Assert.Equal(57.50m, order.DiscountAmount);
        Assert.Equal(517.50m, order.Total);

        var products = await client.GetFromJsonAsync<ProductResponse[]>("/products", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(5, Assert.Single(products!).Stock);

        var storedOrder = await client.GetFromJsonAsync<OrderResponse>($"/orders/{order.Id}", cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(storedOrder);
        Assert.Equal(order.Id, storedOrder!.Id);
        Assert.Single(storedOrder.Lines);

        var allOrders = await client.GetFromJsonAsync<OrderResponse[]>("/orders", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Single(allOrders!);
    }

    [Fact]
    public async Task CreateOrder_WhenStockInsufficient_ReturnsConflictAndLeavesStockUnchanged()
    {
        await using var factory = new InventoryApiFactory(new DateTimeOffset(new DateTime(2026, 6, 2)));
        using var client = factory.CreateClient();

        var product = await CreateProduct(client, "Mouse", 50m, 2);

        var orderResponse = await client.PostAsJsonAsync("/orders", new CreateOrderRequest(
            InventoryDbContextSeed.UnitedStatesCustomerId,
            [new OrderProductRequest(product.Id, 3)]), cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, orderResponse.StatusCode);

        var products = await client.GetFromJsonAsync<ProductResponse[]>("/products", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(2, Assert.Single(products!).Stock);
    }

    [Fact]
    public async Task CreateProduct_WhenInvalid_ReturnsValidationProblem()
    {
        await using var factory = new InventoryApiFactory(new DateTimeOffset(new DateTime(2026, 6, 2)));
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/products", new CreateProductRequest(
            "",
            new string('a', 51),
            -1m,
            -1), cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_WhenInvalid_ReturnsValidationProblem()
    {
        await using var factory = new InventoryApiFactory(new DateTimeOffset(new DateTime(2026, 6, 2)));
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/orders", new CreateOrderRequest(
            Guid.Empty,
            [new OrderProductRequest(Guid.Empty, 0)]), cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(problem);
        Assert.Contains("CustomerId", problem!.Errors.Keys);
        Assert.Contains("products[0].productId", problem.Errors.Keys);
        Assert.Contains("products[0].quantity", problem.Errors.Keys);
    }

    [Fact]
    public async Task GetProduct_WhenMissing_ReturnsProblemDetails()
    {
        await using var factory = new InventoryApiFactory(new DateTimeOffset(new DateTime(2026, 6, 2)));
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/products/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("Product was not found.", problem!.Title);
    }

    private static async Task<ProductResponse> CreateProduct(HttpClient client, string name, decimal price, int stock)
    {
        var response = await client.PostAsJsonAsync("/products", new CreateProductRequest(
            name,
            "Test product",
            price,
            stock));

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ProductResponse>())!;
    }
}
