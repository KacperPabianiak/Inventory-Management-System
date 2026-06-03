namespace InventoryManagement.Api.Domain;

public class Order
{
    public Order(
        Guid id,
        Guid customerId,
        CustomerRegion region,
        DateTimeOffset orderDate,
        decimal subtotal,
        decimal discountAmount,
        string discountName,
        decimal total,
        List<OrderLine> lines)
    {
        Id = id;
        CustomerId = customerId;
        Region = region;
        OrderDate = orderDate;
        Subtotal = subtotal;
        DiscountAmount = discountAmount;
        DiscountName = discountName;
        Total = total;
        Lines = lines;
    }

    private Order()
    {
        DiscountName = string.Empty;
        Lines = [];
    }

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public CustomerRegion Region { get; private set; }
    public DateTimeOffset OrderDate { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public string DiscountName { get; private set; }
    public decimal Total { get; private set; }
    public List<OrderLine> Lines { get; private set; }
}
