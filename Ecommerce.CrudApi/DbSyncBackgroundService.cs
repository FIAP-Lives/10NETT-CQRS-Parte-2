
using Ecommerce.CrudApi.Data.Read;
using Ecommerce.CrudApi.Data.Write;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.CrudApi
{
    internal sealed class DbSyncBackgroundService : BackgroundService
    {
        private readonly IServiceProvider sp;

        public DbSyncBackgroundService(IServiceProvider sp)
        {
            this.sp = sp;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (stoppingToken.IsCancellationRequested is false)
            {
                using var scope = sp.CreateScope();
                var readDbContext = scope.ServiceProvider.GetRequiredService<ReadDbContext>();
                var writeDbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

                var msg = await writeDbContext.OutboxMessages
                    .Where(x => x.ProcessedAtUtc == null && x.Type == MessageTypes.ORDER_CHANGED)
                    .OrderBy(x => x.OccurredAtUtc)
                    .FirstOrDefaultAsync(stoppingToken);

                if (msg is null)
                    continue;

                var payload = JsonSerializer.Deserialize<Dictionary<string, object>>(msg.Payload);
                var orderId = Guid.Parse(payload["orderId"].ToString()!);
                
                var order = await writeDbContext.Orders
                    .Include(x => x.Items)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == orderId, stoppingToken);

                var products = await writeDbContext.Products
                    .AsNoTracking()
                    .ToDictionaryAsync(x => x.Id, stoppingToken);

                var items = order.Items.Select(i => new
                {
                    ProductId = i.ProductId,
                    ProductName = products.TryGetValue(i.ProductId, out var product) ? product.Name : "Unknown",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList();

                var total = items.Sum(i => i.Quantity * i.UnitPrice);

                var rm = await readDbContext.OrdersRead.FirstOrDefaultAsync(x => x.OrderId == order.Id, stoppingToken);
                if (rm is null)
                {
                    rm = new Data.Read.Models.OrderReadModel { OrderId = order.Id };
                    readDbContext.OrdersRead.Add(rm);
                }

                rm.CustomerName = order.CustomerName;
                rm.ShippingAddress = order.ShippingAddress;
                rm.Status = order.Status;
                rm.Total = total;
                rm.ItemsJson = JsonSerializer.Serialize(items);

                await readDbContext.SaveChangesAsync(stoppingToken);

                msg.ProcessedAtUtc = DateTime.UtcNow;
                await writeDbContext.SaveChangesAsync(stoppingToken);

                Console.WriteLine($"Processed Order Id {msg.Id}");

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}