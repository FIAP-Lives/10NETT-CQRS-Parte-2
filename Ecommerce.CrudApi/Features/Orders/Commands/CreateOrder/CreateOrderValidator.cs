using FluentValidation;

namespace Ecommerce.CrudApi.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Customer name is required.")
                .MaximumLength(100).WithMessage("Customer name cannot exceed 100 characters.");

            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Shipping address is required.")
                .MaximumLength(200).WithMessage("Shipping address cannot exceed 200 characters.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must contain at least one item.");
        }
    }
}
