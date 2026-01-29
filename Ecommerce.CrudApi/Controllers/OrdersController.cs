using Ecommerce.CrudApi.Data;
using Ecommerce.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.CrudApi.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly CrudDbContext _db;
    public OrdersController(CrudDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Create(Order order, CancellationToken ct)
    {
        // Risco de Overposting
        order.Id = Guid.Empty;
        order.Status = "Created";

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    // Read "pesado": inclui join/Include de tudo e retorna mais do que precisa
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        if (order is null) return NotFound();


        var response = new
        {
            order.Id,
            order.CustomerName,
            order.ShippingAddress,
            order.Status,
            order.CreatedAtUtc,
            Items = order.Items.Select(i => new
            {
                i.Id,
                i.ProductId,
                ProductName = GetProductNameById(i.ProductId),
                i.Quantity,
                i.UnitPrice
            }).ToList()
        };

        return Ok(response);
    }

    private string? GetProductNameById(int productId)
        => _db.Products.Find(productId)?.Name;

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
