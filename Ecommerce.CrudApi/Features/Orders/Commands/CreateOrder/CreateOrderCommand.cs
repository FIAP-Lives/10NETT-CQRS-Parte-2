using Ecommerce.CrudApi.Data.Write.Entities;
using Ecommerce.CrudApi.Shared;
using MediatR;

namespace Ecommerce.CrudApi.Features.Orders.Commands.CreateOrder
{
    public sealed record CreateOrderCommand(string CustomerName, string  ShippingAddress, IReadOnlyList<OrderItem> Items) : IRequest<Result<Guid>>;
}
