using FluentValidation;

namespace InventoryManagement.Api.Orders;

public sealed class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(request => request.CustomerId)
            .NotEmpty()
            .WithMessage("CustomerId is required.")
            .OverridePropertyName(nameof(CreateOrderRequest.CustomerId));

        RuleFor(request => request.Products)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("At least one product is required.")
            .NotEmpty()
            .WithMessage("At least one product is required.")
            .OverridePropertyName(nameof(CreateOrderRequest.Products));

        RuleForEach(request => request.Products)
            .SetValidator(new OrderProductRequestValidator())
            .When(request => request.Products is not null);
    }
}

public sealed class OrderProductRequestValidator : AbstractValidator<OrderProductRequest>
{
    public OrderProductRequestValidator()
    {
        RuleFor(product => product.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required.")
            .OverridePropertyName("productId");

        RuleFor(product => product.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.")
            .OverridePropertyName("quantity");
    }
}
