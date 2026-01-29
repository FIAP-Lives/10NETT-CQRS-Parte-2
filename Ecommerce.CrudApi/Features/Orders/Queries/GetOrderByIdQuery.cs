using MediatR;

namespace Ecommerce.CrudApi.Features.Orders.Queries
{
    public sealed record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderDto>>;
}
