using Grpc.Net.Client;
using PromoCodeFactory.BlazorClient.Components;
using PromoCodeFactory.BlazorClient.Services;
using Radzen;

namespace PromoCodeFactory.BlazorClient;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddRadzenComponents();
        builder.Services.AddScoped<ICustomerService, CustomerService>();

        builder.Services.AddSingleton(services =>
        {
            var config = services.GetRequiredService<IConfiguration>();
            var grpcServerUrl = config["GrpcServerUrl"];

            //if (string.IsNullOrEmpty(grpcServerUrl))
            //{
            //    var navigationManager = services.GetRequiredService<NavigationManager>();
            //    grpcServerUrl = navigationManager.BaseUri;
            //}

            return GrpcChannel.ForAddress(grpcServerUrl);
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}