using FluentValidation;
using InventoryManagement.Api.Common;

namespace InventoryManagement.Api.Products;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/products", async (GetProductsQueryHandler handler, CancellationToken cancellationToken) =>
        {
            var products = await handler.Handle(new GetProductsQuery(), cancellationToken);
            return Results.Ok(products);
        })
        .WithName("GetProducts");

        app.MapGet("/products/{id:guid}", async (
            Guid id,
            GetProductByIdQueryHandler handler,
            CancellationToken cancellationToken) =>
        {
            var product = await handler.Handle(new GetProductByIdQuery(id), cancellationToken);
            return product is null
                ? Results.NotFound(EndpointResults.ProblemDetails("Product was not found.", StatusCodes.Status404NotFound))
                : Results.Ok(product);
        })
        .WithName("GetProductById");

        app.MapPost("/products", async (
            CreateProductRequest request,
            IValidator<CreateProductRequest> validator,
            CreateProductCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var validationErrors = await ValidationProblemFactory.ValidateAsync(request, validator, cancellationToken);
            if (validationErrors is not null)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var product = await handler.Handle(new CreateProductCommand(
                request.Name!,
                request.Description!,
                request.Price,
                request.Stock), cancellationToken);

            return Results.Created($"/products/{product.Id}", product);
        })
        .WithName("CreateProduct");

        app.MapPut("/products/{id:guid}", async (
            Guid id,
            UpdateProductRequest request,
            IValidator<UpdateProductRequest> validator,
            UpdateProductCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var validationErrors = await ValidationProblemFactory.ValidateAsync(request, validator, cancellationToken);
            if (validationErrors is not null)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var product = await handler.Handle(new UpdateProductCommand(
                id,
                request.Name!,
                request.Description!,
                request.Price,
                request.Stock), cancellationToken);

            return product is null
                ? Results.NotFound(EndpointResults.ProblemDetails("Product was not found.", StatusCodes.Status404NotFound))
                : Results.Ok(product);
        })
        .WithName("UpdateProduct");

        app.MapDelete("/products/{id:guid}", async (
            Guid id,
            DeleteProductCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var deleted = await handler.Handle(new DeleteProductCommand(id), cancellationToken);
            return deleted
                ? Results.NoContent()
                : Results.NotFound(EndpointResults.ProblemDetails("Product was not found.", StatusCodes.Status404NotFound));
        })
        .WithName("DeleteProduct");

        return app;
    }
}
