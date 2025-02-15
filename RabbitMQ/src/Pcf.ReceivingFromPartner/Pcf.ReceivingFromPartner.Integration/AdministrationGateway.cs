using System;
using System.Threading.Tasks;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.Rmq.Producer;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class AdministrationGateway(IRmqProducer<NotifyAdminAboutPartnerManagerPromoCodeDto> producer)
        : IAdministrationGateway
    {
        private const string ROUTING_KEY = "administration.notification";

        private readonly IRmqProducer<NotifyAdminAboutPartnerManagerPromoCodeDto> _producer = producer;

        public async Task NotifyAdminAboutPartnerManagerPromoCode(Guid partnerManagerId)
        {
            var data = new NotifyAdminAboutPartnerManagerPromoCodeDto { PartnerManagerId = partnerManagerId };
            await _producer.PublishAsync(ROUTING_KEY, data);
        }
    }
}