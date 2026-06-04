namespace InventoryManagement.Api.Orders;

public static class PolishHolidays
{
    public static bool IsBankHoliday(DateTimeOffset date)
    {
        if (FixedHolidays.Contains((date.Month, date.Day)))
        {
            return true;
        }

        var easterSunday = CalculateEasterSunday(date.Year);
        return SameCalendarDay(date, easterSunday)
            || SameCalendarDay(date, easterSunday.AddDays(1))
            || SameCalendarDay(date, easterSunday.AddDays(49))
            || SameCalendarDay(date, easterSunday.AddDays(60));
    }

    private static readonly HashSet<(int Month, int Day)> FixedHolidays =
    [
        (1, 1),
        (1, 6),
        (5, 1),
        (5, 3),
        (8, 15),
        (11, 1),
        (11, 11),
        (12, 25),
        (12, 26)
    ];

    private static DateTimeOffset CalculateEasterSunday(int year)
    {
        var a = year % 19;
        var b = year / 100;
        var c = year % 100;
        var d = b / 4;
        var e = b % 4;
        var f = (b + 8) / 25;
        var g = (b - f + 1) / 3;
        var h = (19 * a + b - d - g + 15) % 30;
        var i = c / 4;
        var k = c % 4;
        var l = (32 + 2 * e + 2 * i - h - k) % 7;
        var m = (a + 11 * h + 22 * l) / 451;
        var month = (h + l - 7 * m + 114) / 31;
        var day = (h + l - 7 * m + 114) % 31 + 1;

        return new DateTimeOffset(new DateTime(year, month, day));
    }

    private static bool SameCalendarDay(DateTimeOffset left, DateTimeOffset right)
    {
        return left.Year == right.Year
            && left.Month == right.Month
            && left.Day == right.Day;
    }
}
