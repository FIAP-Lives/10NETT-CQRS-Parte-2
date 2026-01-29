using Ecommerce.CrudApi.Data;

namespace Ecommerce.CrudApi.Features.Orders.Commands.CreateOrder
{
    public sealed record CreateOrderCommand(string CustomerName, string  ShippingAddress, IReadOnlyList<OrderItem> Items);
}
