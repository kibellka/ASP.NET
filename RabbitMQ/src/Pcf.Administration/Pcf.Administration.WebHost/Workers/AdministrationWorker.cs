using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pcf.Administration.Core.Abstractions.Services;
using Pcf.Rmq.Consumer;

namespace Pcf.Administration.WebHost.Workers
{
    public class AdministrationWorker(IRmqConsumer<NotifyAdminAboutPartnerManagerPromoCodeDto> consumer, IServiceScopeFactory serviceScopeFactory) : BackgroundService
    {
        private readonly IRmqConsumer<NotifyAdminAboutPartnerManagerPromoCodeDto> _consumer = consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _consumer.Subscribe(async message =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var employeeService = scope.ServiceProvider.GetRequiredService<IEmployeeService>();

                await employeeService.AppliedPromocodesAsync(message.PartnerManagerId);

            }, cancellationToken);
        }
    }
}
