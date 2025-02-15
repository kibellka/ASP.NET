using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pcf.Administration.DataAccess;

namespace Pcf.Administration.WebHost.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static Task InitMongoDb(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            return dbContext.InitializeAsync();
        }
    }
}
