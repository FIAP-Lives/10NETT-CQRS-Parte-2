namespace Ecommerce.CrudApi.Data;

public sealed class Order
{
    public Guid Id { get; set; }
    public string? CustomerName { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Status { get; set; } = "Draft";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<OrderItem> Items { get; set; } = new();
}
