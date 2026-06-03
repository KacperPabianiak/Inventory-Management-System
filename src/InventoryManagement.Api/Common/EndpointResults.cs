using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Common;

public static class EndpointResults
{
    public static ProblemDetails ProblemDetails(string title, int statusCode)
    {
        return new ProblemDetails
        {
            Title = title,
            Status = statusCode
        };
    }
}
