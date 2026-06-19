using OrderGenerator.Api.Services.Interfaces;
using QuickFix;

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
        if (_sessionIdProvider.TryGetSessionID(out var sessionID))
        {
            await foreach (var order in _orderQueue.Reader.ReadAllAsync(stoppingToken))
            {
                Session.SendToTarget(order, sessionID!);
            }
        }
    }
}
