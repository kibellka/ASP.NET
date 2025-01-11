namespace Pcf.Rmq.Consumer
{
    public interface IRmqConsumer<T> where T : class
    {
        Task Subscribe(Func<T, Task> handler, CancellationToken cancellationToken = default);
    }
}