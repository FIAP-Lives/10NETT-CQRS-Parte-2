using Ecommerce.CrudApi.Data.Write;
using Ecommerce.CrudApi.Data.Write.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.CrudApi.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly WriteDbContext _db;
    public ProductsController(WriteDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Create(Product product, CancellationToken ct)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var p = await _db.Products.FindAsync(new object[] { id }, ct);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
        => Ok(await _db.Products.AsNoTracking().OrderBy(x => x.Id).ToListAsync(ct));
}