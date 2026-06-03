using InventoryManagement.Api.Domain;

namespace InventoryManagement.Api.Data;

public static class InventoryDbContextSeed
{
    public static readonly Guid UnitedStatesCustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid EuropeCustomerId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid AsiaCustomerId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static async Task SeedCustomers(this InventoryDbContext db)
    {
        if (db.Customers.Any())
        {
            return;
        }

        await db.Customers.AddRangeAsync(
            new Customer(UnitedStatesCustomerId, "US Customer", CustomerRegion.UnitedStates),
            new Customer(EuropeCustomerId, "Europe Customer", CustomerRegion.Europe),
            new Customer(AsiaCustomerId, "Asia Customer", CustomerRegion.Asia));

        await db.SaveChangesAsync();
    }
}
