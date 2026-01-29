using Ecommerce.CrudApi.Data;

namespace Ecommerce.CrudApi.Features.Orders.Commands.CreateOrder
{
    public sealed class CreateOrderCommandHandler 
    {
        private readonly CreateOrderValidator validator;
        private readonly CrudDbContext db;

        public CreateOrderCommandHandler(CreateOrderValidator validator, CrudDbContext db)
        {
            this.validator = validator;
            this.db = db;
        }

        public async Task<Result<Guid>> Handle(CreateOrderCommand command)
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

            return Result<Guid>.Success(order.Id);
        }
    }
}
