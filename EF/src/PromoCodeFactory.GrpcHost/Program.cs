using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Services.Abstractions;
using PromoCodeFactory.Core.Services.Implementations;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.GrpcHost.Services;

namespace PromoCodeFactory.GrpcHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        builder.Services.AddScoped<ICustomerService, CustomerService>();

        builder.Services.AddDbContext<DataContext>(optionsBuilder =>
        {
            optionsBuilder
                .UseNpgsql(builder.Configuration.GetConnectionString("PostgresDb"))
                .UseSnakeCaseNamingConvention()
                .UseLazyLoadingProxies();
        });

        builder.Services.AddGrpc();
        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            db.Database.Migrate();
            Seed(scope.ServiceProvider);
        }

        app.MapGrpcService<GrpcCustomerService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

        app.Run();
    }

    public static void Seed(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();

            //dbContext.AddRange(FakeDataFactory.Employees);
            //dbContext.AddRange(FakeDataFactory.Customers);
            //dbContext.AddRange(FakeDataFactory.Preferences);
            //dbContext.SaveChanges();
        }
    }
}
