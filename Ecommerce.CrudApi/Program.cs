using Ecommerce.CrudApi.Data.Read;
using Ecommerce.CrudApi.Data.Write;
using Ecommerce.CrudApi.Infrastructure;
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

        builder.Services.AddDbContext<WriteDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("WriteDb")));
        builder.Services.AddDbContext<ReadDbContext>(opt =>
        opt.UseNpgsql(builder.Configuration.GetConnectionString("ReadDb"),
            x => x.MigrationsHistoryTable("__EFMigrationsHistory_Read")));

        builder.Services.AddTransient<ExceptionMiddleware>();

        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddMediatR(x =>
        {
            x.LicenseKey = builder.Configuration.GetValue<string>("mediatr-license");
            x.RegisterServicesFromAssemblyContaining<Program>();
        });

        builder.Services.AddHostedService<DbSyncBackgroundService>();

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
