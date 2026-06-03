using FluentValidation;
using InventoryManagement.Api.Common;

namespace InventoryManagement.Api.Orders;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/orders", async (GetOrdersQueryHandler handler, CancellationToken cancellationToken) =>
        {
            var orders = await handler.Handle(new GetOrdersQuery(), cancellationToken);
            return Results.Ok(orders);
        })
        .WithName("GetOrders");

        app.MapGet("/orders/{id:guid}", async (
            Guid id,
            GetOrderByIdQueryHandler handler,
            CancellationToken cancellationToken) =>
        {
            var order = await handler.Handle(new GetOrderByIdQuery(id), cancellationToken);
            return order is null
                ? Results.NotFound(EndpointResults.ProblemDetails("Order was not found.", StatusCodes.Status404NotFound))
                : Results.Ok(order);
        })
        .WithName("GetOrderById");

        app.MapPost("/orders", async (
            CreateOrderRequest request,
            IValidator<CreateOrderRequest> validator,
            CreateOrderCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var validationErrors = await ValidationProblemFactory.ValidateAsync(request, validator, cancellationToken);
            if (validationErrors is not null)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var result = await handler.Handle(new CreateOrderCommand(request.CustomerId, request.Products), cancellationToken);

            return result.Status switch
            {
                CreateOrderStatus.Created => Results.Created($"/orders/{result.Order!.Id}", result.Order),
                CreateOrderStatus.CustomerNotFound => Results.NotFound(EndpointResults.ProblemDetails("Customer was not found.", StatusCodes.Status404NotFound)),
                CreateOrderStatus.ProductNotFound => Results.NotFound(EndpointResults.ProblemDetails(result.ErrorMessage!, StatusCodes.Status404NotFound)),
                CreateOrderStatus.InsufficientStock => Results.Conflict(EndpointResults.ProblemDetails(result.ErrorMessage!, StatusCodes.Status409Conflict)),
                _ => Results.BadRequest(EndpointResults.ProblemDetails(result.ErrorMessage ?? "Order could not be created.", StatusCodes.Status400BadRequest))
            };
        })
        .WithName("CreateOrder");

        return app;
    }
}
