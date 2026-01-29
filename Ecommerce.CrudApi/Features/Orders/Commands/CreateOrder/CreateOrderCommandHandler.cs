using Ecommerce.CrudApi.Data.Write;
using Ecommerce.CrudApi.Data.Write.Entities;
using Ecommerce.CrudApi.Shared;
using MediatR;
using System.Text.Json;

namespace Ecommerce.CrudApi.Features.Orders.Commands.CreateOrder
{
    public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        private readonly CreateOrderValidator validator;
        private readonly WriteDbContext db;

        public CreateOrderCommandHandler(CreateOrderValidator validator, WriteDbContext db)
        {
            this.validator = validator;
            this.db = db;
        }

        public async Task<Result<Guid>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Result<Guid>.Failure(ErrorCodes.VALIDATION, errors);
            }

            var order = new Order()
            {
                CustomerName = command.CustomerName,
                ShippingAddress = command.ShippingAddress,
                CreatedAtUtc = DateTime.UtcNow,
                Items = command.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };


            db.Orders.Add(order);
            await db.SaveChangesAsync();

            var evt = new { orderId = order.Id };
            db.OutboxMessages.Add(new Data.Write.Entities.OutboxMessage
            {
                Type = MessageTypes.ORDER_CHANGED,
                Payload = JsonSerializer.Serialize(evt)
            });
            await db.SaveChangesAsync();

            return Result<Guid>.Success(order.Id);
        }
    }
}
