using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Pcf.Rmq.Producer
{
    public class RmqProducer<T>(IConnection connection, IOptions<RmqProducerOptions> options) : IRmqProducer<T> where T : class
    {
        private readonly IConnection _connection = connection;
        private readonly RmqProducerOptions _options = options.Value;
        private IChannel _channel;

        public async Task PublishAsync(string routingKey, T message, CancellationToken cancellationToken = default)
        {
            if (_channel is null)
            {
                _channel = await _connection.CreateChannelAsync();
            }

            var jsonString = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonString);

            await _channel.ExchangeDeclareAsync(_options.ExchangeName, _options.ExchangeType, true, false, cancellationToken: cancellationToken);
            await _channel.BasicPublishAsync(_options.ExchangeName, routingKey, body, cancellationToken);
        }
    }
}
