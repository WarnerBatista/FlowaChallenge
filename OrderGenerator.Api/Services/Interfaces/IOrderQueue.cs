using QuickFix.FIX44;
using System.Threading.Channels;

namespace OrderGenerator.Api.Services.Interfaces;

public interface IOrderQueue
{
    ChannelReader<NewOrderSingle> Reader { get; }
    ValueTask EnqueueAsync(NewOrderSingle order);
}
