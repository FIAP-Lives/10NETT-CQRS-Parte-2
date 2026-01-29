using Ecommerce.CrudApi.Features.Orders.Commands.CreateOrder;
using Ecommerce.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecommerce.CrudApi.Features.Orders.Queries;
using MediatR;
using Ecommerce.CrudApi.Data.Write;
using Ecommerce.CrudApi.Data.Write.Entities;
using Ecommerce.CrudApi.Shared;

namespace Ecommerce.CrudApi.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly WriteDbContext _db;
    private readonly IMediator mediator;

    public OrdersController(WriteDbContext db, IMediator mediator)
    {
        _db = db;
        this.mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { id = result.Value });

        return result.ToActionResult();
    }

    // Read "pesado": inclui join/Include de tudo e retorna mais do que precisa
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetOrderByIdQuery(id), ct);
        return result.ToActionResult();
    }

    // O que vamos "quebrar" na aula
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, Order updated, CancellationToken ct)
    {
        var existing = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, ct);
        if (existing is null) return NotFound();

        // IF HELL: deduz intenção do usuário...
        if (!string.Equals(existing.Status, updated.Status, StringComparison.OrdinalIgnoreCase))
        {
            if (updated.Status == "Paid")
            {
                if (existing.Status != "Created")
                    throw new DomainException("ORDER_INVALID_STATE", "Only orders in Created tatus can be marked as Paid.");

                existing.Status = "Paid";
                // Enviar notificações para o usuário
                // Iniciar o processo de logística
                //  etc.
            }

            if (updated.Status == "Cancelled")
            {
                if (existing.Status == "Shipped")
                    throw new DomainException("ORDER_INVALID_STATE", "Shipped orders cannot be cancelled.");

                existing.Status = "Cancelled";
                // faz o estorno
                // atualiza estoque
            }
        }

        if (!string.Equals(existing.ShippingAddress, updated.ShippingAddress, StringComparison.OrdinalIgnoreCase))
        {
            if (existing.Status is "Cancelled" or "Shipped")
                throw new DomainException("ORDER_INVALID_STATE", "Cannot change address for Cancelled or Shipped orders.");

            existing.ShippingAddress = updated.ShippingAddress;
            // precisa avisar a transportadora?
            // envia e-mail para o usuário
        }

        await _db.SaveChangesAsync(ct);
        return Ok();
    }
}
