using FluentValidation;
using System.Linq.Expressions;

namespace InventoryManagement.Api.Products;

public sealed class CreateProductRequestValidator : ProductRequestValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
        : base(
            request => request.Name,
            request => request.Description,
            request => request.Price,
            request => request.Stock)
    {
    }
}

public sealed class UpdateProductRequestValidator : ProductRequestValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
        : base(
            request => request.Name,
            request => request.Description,
            request => request.Price,
            request => request.Stock)
    {
    }
}

public abstract class ProductRequestValidator<TRequest> : AbstractValidator<TRequest>
{
    protected ProductRequestValidator(
        Expression<Func<TRequest, string?>> name,
        Expression<Func<TRequest, string?>> description,
        Expression<Func<TRequest, decimal>> price,
        Expression<Func<TRequest, int>> stock)
    {
        RuleFor(name)
            .Cascade(CascadeMode.Stop)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("Name is required.")
            .Must(value => value!.Trim().Length <= 50)
            .WithMessage("Name must be 50 characters or fewer.")
            .OverridePropertyName("name");

        RuleFor(description)
            .Cascade(CascadeMode.Stop)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("Description is required.")
            .Must(value => value!.Trim().Length <= 50)
            .WithMessage("Description must be 50 characters or fewer.")
            .OverridePropertyName("description");

        RuleFor(price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero.")
            .OverridePropertyName("price");

        RuleFor(stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock cannot be negative.")
            .OverridePropertyName("stock");
    }
}
