namespace Pcf.Rmq.Consumer
{
    public class RmqConsumerOptions
    {
        public required string HostName { get; set; }

        public required int Port { get; set; }

        public required string UserName { get; set; }

        public required string Password { get; set; }

        public required string VirtualHost { get; set; }

        public required string ExchangeName { get; set; }

        public required string ExchangeType { get; set; }

        public required string QueueName { get; set; }

        public required string RoutingKey { get; set; }
    }
}
