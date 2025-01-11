namespace Pcf.Rmq.Producer
{
    public interface IRmqProducer<T> where T : class
    {
        Task PublishAsync(string routingKey, T message, CancellationToken cancellationToken = default);
    }
}
