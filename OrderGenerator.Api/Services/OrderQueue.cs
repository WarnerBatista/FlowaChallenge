using OrderGenerator.Api.Services.Interfaces;
using QuickFix.FIX44;
using System.Threading.Channels;

namespace OrderGenerator.Api.Services;

public class OrderQueue : IOrderQueue
{
    private readonly Channel<NewOrderSingle> _channel;
    public ChannelReader<NewOrderSingle> Reader => _channel.Reader;

    public OrderQueue()
    {
        _channel = Channel.CreateUnbounded<NewOrderSingle>();
    }

    public ValueTask EnqueueAsync(NewOrderSingle order)
    {
        return _channel.Writer.WriteAsync(order);
    }
}
