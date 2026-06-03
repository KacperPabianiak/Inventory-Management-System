using InventoryManagement.Api.Domain;
using InventoryManagement.Api.Orders;

namespace InventoryManagement.UnitTests;

public sealed class OrderPricingTests
{
    [Fact]
    public void Calculate_AppliesLocationIncreaseBeforeVolumeDiscount()
    {
        var product = Product("Widget", 10m);

        var result = OrderPricing.Calculate(
            [product],
            new Dictionary<Guid, int> { [product.Id] = 10 },
            CustomerRegion.Europe,
            new DateTimeOffset(new DateTime(2026, 6, 2)));

        Assert.Equal(115m, result.Subtotal);
        Assert.Equal("Volume", result.DiscountName);
        Assert.Equal(23m, result.DiscountAmount);
        Assert.Equal(92m, result.Total);
    }

    [Fact]
    public void Calculate_AppliesBlackFridayWhenItIsBestForCustomer()
    {
        var product = Product("Widget", 100m);

        var result = OrderPricing.Calculate(
            [product],
            new Dictionary<Guid, int> { [product.Id] = 5 },
            CustomerRegion.UnitedStates,
            new DateTimeOffset(new DateTime(2026, 11, 27)));

        Assert.Equal("Black Friday", result.DiscountName);
        Assert.Equal(125m, result.DiscountAmount);
        Assert.Equal(375m, result.Total);
    }

    [Fact]
    public void Calculate_AppliesHolidayDiscountToOneMostExpensiveUnit()
    {
        var cheap = Product("Cheap", 100m);
        var expensive = Product("Expensive", 200m);

        var result = OrderPricing.Calculate(
            [cheap, expensive],
            new Dictionary<Guid, int>
            {
                [cheap.Id] = 1,
                [expensive.Id] = 1
            },
            CustomerRegion.UnitedStates,
            new DateTimeOffset(new DateTime(2026, 1, 1)));

        Assert.Equal("Holiday", result.DiscountName);
        Assert.Equal(30m, result.DiscountAmount);
        Assert.Equal(270m, result.Total);
    }

    [Fact]
    public void Calculate_AppliesHolidayDiscountOnMovablePolishHoliday()
    {
        var product = Product("Widget", 100m);

        var result = OrderPricing.Calculate(
            [product],
            new Dictionary<Guid, int> { [product.Id] = 1 },
            CustomerRegion.UnitedStates,
            new DateTimeOffset(new DateTime(2026, 5, 24)));

        Assert.Equal("Holiday", result.DiscountName);
        Assert.Equal(15m, result.DiscountAmount);
        Assert.Equal(85m, result.Total);
    }

    [Fact]
    public void Calculate_WhenMultipleDiscountsApply_ChoosesHighestDiscountAmount()
    {
        var product = Product("Widget", 10m);

        var result = OrderPricing.Calculate(
            [product],
            new Dictionary<Guid, int> { [product.Id] = 50 },
            CustomerRegion.UnitedStates,
            new DateTimeOffset(new DateTime(2026, 11, 27)));

        Assert.Equal("Volume", result.DiscountName);
        Assert.Equal(150m, result.DiscountAmount);
        Assert.Equal(350m, result.Total);
    }

    private static Product Product(string name, decimal price)
    {
        return new Product(Guid.NewGuid(), name, "Description", price, 100);
    }
}
