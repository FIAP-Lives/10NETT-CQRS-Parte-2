namespace Ecommerce.CrudApi.Features.Orders.Queries
{
    public class ItemDto
    {
        public required string Name { get; init; }
        public decimal Price { get; init; }
        public int Quantity { get; init; }
    }
}