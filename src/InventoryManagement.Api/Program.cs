using FluentValidation;
using InventoryManagement.Api.Data;
using InventoryManagement.Api.Orders;
using InventoryManagement.Api.Products;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddDbContext<InventoryDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Inventory")
        ?? "Data Source=inventory.db";
    options.UseSqlite(connectionString);
});

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<GetProductsQueryHandler>();
builder.Services.AddScoped<GetProductByIdQueryHandler>();
builder.Services.AddScoped<CreateProductCommandHandler>();
builder.Services.AddScoped<UpdateProductCommandHandler>();
builder.Services.AddScoped<DeleteProductCommandHandler>();
builder.Services.AddScoped<CreateOrderCommandHandler>();
builder.Services.AddScoped<GetOrdersQueryHandler>();
builder.Services.AddScoped<GetOrderByIdQueryHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    db.Database.Migrate();
    await db.SeedCustomers();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var logger = context.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("GlobalExceptionHandler");
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (exception is not null)
        {
            logger.LogError(exception, "Unhandled API exception.");
        }

        await Results.Problem(
            title: "An unexpected server error occurred.",
            statusCode: StatusCodes.Status500InternalServerError)
            .ExecuteAsync(context);
    });
});

app.MapProductEndpoints();
app.MapOrderEndpoints();

app.Run();
