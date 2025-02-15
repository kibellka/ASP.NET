using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Pcf.Rmq.Consumer
{
    public class RmqConsumer<T>(IConnection connection, IOptions<RmqConsumerOptions> options) : IRmqConsumer<T> where T : class
    {
        private readonly IConnection _connection = connection;
        private readonly RmqConsumerOptions _options = options.Value;
        private IChannel _channel;

        public async Task Subscribe(Func<T, Task> handler, CancellationToken cancellationToken = default)
        {
            if (_channel is null)
            {
                _channel = await _connection.CreateChannelAsync();
            }

            await _channel.QueueDeclareAsync(_options.QueueName, true, false, false, cancellationToken: cancellationToken);
            await _channel.QueueBindAsync(_options.QueueName, _options.ExchangeName, _options.RoutingKey, cancellationToken: cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));

                    if (message is not null)
                    {
                        await handler(message);
                        await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
                    }
                }
                catch(Exception ex)
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true, cancellationToken);
                }
            };

            await _channel.BasicConsumeAsync(_options.QueueName, false, consumer, cancellationToken);
        }
    }
}
