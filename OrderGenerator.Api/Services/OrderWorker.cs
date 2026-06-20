using FixCommons;
using OrderGenerator.Api.Services.Interfaces;

namespace OrderGenerator.Api.Services;

public class OrderWorker : BackgroundService
{
    private readonly IOrderQueue _orderQueue;
    private readonly ISessionIdProvider _sessionIdProvider;

    public OrderWorker(IOrderQueue orderQueue, ISessionIdProvider sessionIdProvider)
    {
        _orderQueue = orderQueue ?? throw new ArgumentNullException(nameof(orderQueue));
        _sessionIdProvider = sessionIdProvider ?? throw new ArgumentNullException(nameof(sessionIdProvider));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var order in _orderQueue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                _sessionIdProvider.SendToTarget(order);
            }
            catch (Exception ex)
            {
                await _orderQueue.EnqueueAsync(order);
                Console.WriteLine($"Error sending order: {ex.Message}");
            }
        }
    }
}
