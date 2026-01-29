namespace Ecommerce.CrudApi.Features.Orders.Queries
{
    public sealed record OrderDto
    {
        public Guid Id { get; init; }
        public string? CustomerName { get; init; }
        public string? ShippingAddress { get; init; }
        public string? Status { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public decimal Total { get; init; }
        public List<ItemDto> Items { get; init; }

    }
}
