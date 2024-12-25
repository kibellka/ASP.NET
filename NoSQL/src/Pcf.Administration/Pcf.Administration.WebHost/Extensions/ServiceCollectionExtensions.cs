using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.DataAccess;
using Pcf.Administration.DataAccess.Repositories;

namespace Pcf.Administration.WebHost.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IMongoClient>(sp =>
            {
                return new MongoClient(connectionString);
            });

            services.AddScoped<IMongoDatabase>(sp =>
            {
                var url = new MongoUrl(connectionString);
                var client = sp.GetRequiredService<IMongoClient>();
                var database = client.GetDatabase(url.DatabaseName);
                return database;
            });

            services.AddScoped<DataContext>();
            services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));

            return services;
        }
    }
}
