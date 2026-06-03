using InventoryManagement.Api.Domain;

namespace InventoryManagement.Api.Orders;

public static class OrderPricing
{
    public static PricingResult Calculate(
        IEnumerable<Product> products,
        IReadOnlyDictionary<Guid, int> quantities,
        CustomerRegion region,
        DateTimeOffset orderDate)
    {
        var multiplier = region switch
        {
            CustomerRegion.Europe => 1.15m,
            CustomerRegion.Asia => 1.05m,
            _ => 1.00m
        };

        var lines = products
            .OrderBy(product => product.Name)
            .Select(product =>
            {
                var adjustedUnitPrice = Money(product.Price * multiplier);
                var quantity = quantities[product.Id];
                return new PricingLine(
                    product.Id,
                    product.Name,
                    quantity,
                    Money(product.Price),
                    adjustedUnitPrice,
                    Money(adjustedUnitPrice * quantity));
            })
            .ToArray();

        var subtotal = Money(lines.Sum(line => line.LineTotal));
        var totalQuantity = lines.Sum(line => line.Quantity);
        var discounts = new List<DiscountCandidate>
        {
            VolumeDiscount(subtotal, totalQuantity),
            BlackFridayDiscount(subtotal, orderDate),
            HolidayDiscount(lines, orderDate)
        };

        var bestDiscount = discounts
            .Where(discount => discount.Amount > 0)
            .OrderByDescending(discount => discount.Amount)
            .ThenBy(discount => discount.Priority)
            .FirstOrDefault();

        var discountAmount = bestDiscount?.Amount ?? 0m;
        var discountName = bestDiscount?.Name ?? "None";

        return new PricingResult(lines, subtotal, discountAmount, discountName, Money(subtotal - discountAmount));
    }

    private static DiscountCandidate VolumeDiscount(decimal subtotal, int totalQuantity)
    {
        var percentage = totalQuantity switch
        {
            >= 50 => 0.30m,
            >= 10 => 0.20m,
            >= 5 => 0.10m,
            _ => 0m
        };

        return new DiscountCandidate("Volume", Money(subtotal * percentage), 3);
    }

    private static DiscountCandidate BlackFridayDiscount(decimal subtotal, DateTimeOffset orderDate)
    {
        return IsBlackFriday(orderDate)
            ? new DiscountCandidate("Black Friday", Money(subtotal * 0.25m), 1)
            : new DiscountCandidate("Black Friday", 0m, 1);
    }

    private static DiscountCandidate HolidayDiscount(IReadOnlyList<PricingLine> lines, DateTimeOffset orderDate)
    {
        if (!PolishHolidays.IsBankHoliday(orderDate) || lines.Count == 0)
        {
            return new DiscountCandidate("Holiday", 0m, 2);
        }

        var mostExpensiveUnit = lines.Max(line => line.AdjustedUnitPrice);
        return new DiscountCandidate("Holiday", Money(mostExpensiveUnit * 0.15m), 2);
    }

    private static bool IsBlackFriday(DateTimeOffset date)
    {
        if (date.Month != 11 || date.DayOfWeek != DayOfWeek.Friday)
        {
            return false;
        }

        var fourthThursday = Enumerable.Range(1, DateTime.DaysInMonth(date.Year, 11))
            .Select(day => new DateTimeOffset(new DateTime(date.Year, 11, day)))
            .Where(d => d.DayOfWeek == DayOfWeek.Thursday)
            .Skip(3)
            .First();

        return SameCalendarDay(date, fourthThursday.AddDays(1));
    }

    private static bool SameCalendarDay(DateTimeOffset left, DateTimeOffset right)
    {
        return left.Year == right.Year
            && left.Month == right.Month
            && left.Day == right.Day;
    }

    private static decimal Money(decimal amount)
    {
        return decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    private sealed record DiscountCandidate(string Name, decimal Amount, int Priority);
}

public sealed record PricingResult(
    IReadOnlyList<PricingLine> Lines,
    decimal Subtotal,
    decimal DiscountAmount,
    string DiscountName,
    decimal Total);

public sealed record PricingLine(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal AdjustedUnitPrice,
    decimal LineTotal);
