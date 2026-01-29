using Ecommerce.CrudApi.Data.Write;
using Ecommerce.CrudApi.Data.Write.Entities;
using Ecommerce.CrudApi.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.CrudApi.Features.Orders.Queries
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
    {
        private readonly WriteDbContext db;

        public GetOrderByIdQueryHandler(WriteDbContext db)
        {
            this.db = db;
        }

        public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery query, CancellationToken ct)
        {
            var existing = db.Orders.Include(x => x.Items).FirstOrDefault(x => x.Id == query.Id);

            if (existing is null)
                return Result<OrderDto>.Failure(ErrorCodes.NOT_FOUND, "Order not found.");

            return Result<OrderDto>.Success(new OrderDto
            {
                Id = existing.Id,
                CustomerName = existing.CustomerName,
                ShippingAddress = existing.ShippingAddress,
                Status = existing.Status,
                Items = GetItems(existing.Items)
            });
        }

        private List<ItemDto> GetItems(IReadOnlyList<OrderItem> items)
        {
            return items.Select(x => new ItemDto
            {
                Name = GetProductNameById(x.ProductId),
                Price = x.UnitPrice,
                Quantity = x.Quantity
            }).ToList();
        }

        private string GetProductNameById(int productId)
        {
            return db.Products.Find(productId)?.Name ?? "Unknown Product";
        }
    }
}
