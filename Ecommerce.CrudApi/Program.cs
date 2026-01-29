
using Ecommerce.CrudApi.Data;
using Ecommerce.CrudApi.Features.Orders.Commands.CreateOrder;
using Ecommerce.CrudApi.Features.Orders.Queries;
using Ecommerce.CrudApi.Middlewares;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.CrudApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<CrudDbContext>(opt =>
            opt.UseNpgsql(builder.Configuration.GetConnectionString("WriteDb")));

        builder.Services.AddTransient<ExceptionMiddleware>();

        builder.Services.AddTransient<CreateOrderCommandHandler>();
        builder.Services.AddTransient<GetOrderByIdQueryHandler>();

        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        var app = builder.Build();

        app.UseMiddleware<ExceptionMiddleware>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
