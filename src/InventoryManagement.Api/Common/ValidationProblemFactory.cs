using FluentValidation;

namespace InventoryManagement.Api.Common;

public static class ValidationProblemFactory
{
    public static async Task<Dictionary<string, string[]>?> ValidateAsync<TRequest>(
        TRequest request,
        IValidator<TRequest> validator,
        CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);
        if (result.IsValid)
        {
            return null;
        }

        return result.Errors
            .GroupBy(error => NormalizePropertyName(error.PropertyName))
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());
    }

    private static string NormalizePropertyName(string propertyName)
    {
        return propertyName.StartsWith("Products[", StringComparison.Ordinal)
            ? $"products{propertyName["Products".Length..]}"
            : propertyName;
    }
}
