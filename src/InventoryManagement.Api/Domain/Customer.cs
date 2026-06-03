namespace InventoryManagement.Api.Domain;

public class Customer
{
    public Customer(Guid id, string name, CustomerRegion region)
    {
        Id = id;
        Name = name;
        Region = region;
    }

    private Customer()
    {
        Name = string.Empty;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public CustomerRegion Region { get; private set; }
}
