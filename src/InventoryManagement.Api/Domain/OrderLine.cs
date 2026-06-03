namespace InventoryManagement.Api.Domain;

public class OrderLine
{
    public OrderLine(
        Guid id,
        Guid orderId,
        Guid productId,
        string productName,
        int quantity,
        decimal unitPrice,
        decimal adjustedUnitPrice,
        decimal lineTotal)
    {
        Id = id;
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        AdjustedUnitPrice = adjustedUnitPrice;
        LineTotal = lineTotal;
    }

    private OrderLine()
    {
        ProductName = string.Empty;
    }

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal AdjustedUnitPrice { get; private set; }
    public decimal LineTotal { get; private set; }
}
