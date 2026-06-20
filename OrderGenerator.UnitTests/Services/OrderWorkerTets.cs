using FixCommons;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using OrderGenerator.Api.Services;
using OrderGenerator.Api.Services.Interfaces;
using OrderGenerator.UnitTests.Attributes;
using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace OrderGenerator.UnitTests.Services;

public class OrderWorkerTets
{
    [Fact]
    public void Constructor_Throws_When_OrderQueue_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(
            () => new OrderWorker(null!, Substitute.For<ISessionIdProvider>()));
    }

    [Fact]
    public void Constructor_Throws_When_SessionIdProvider_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(
            () => new OrderWorker(Substitute.For<IOrderQueue>(), null!));
    }

    [Theory, AutoNSubstitute]
    public async Task ExecuteAsync_Sends_Orders_From_Queue(
        ISessionIdProvider sessionIdProvider)
    {
        // Arrange
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");

        var orderQueue = new OrderQueue();
        var sut = new OrderWorker(orderQueue, sessionIdProvider);

        var order = new NewOrderSingle
        {
            Side = new Side(Side.BUY),
            Symbol = new Symbol("PETR4"),
            OrderQty = new OrderQty(100),
            Price = new Price(10m)
        };

        await orderQueue.EnqueueAsync(order);
        sessionIdProvider.SetSessionId(sessionId);
        sessionIdProvider.SendToTarget(order).Returns(true);

        // Act
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        await sut.StartAsync(cts.Token);
        await Task.Delay(100);

        // Assert
        sessionIdProvider.Received(1).SendToTarget(order);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldEnqueue_WhenSendToTarget_ThrowsAnException()
    {
        // Arrange
        var orderQueue = new OrderQueue();
        var sessionIdProvider = Substitute.For<ISessionIdProvider>();
        var sut = new OrderWorker(orderQueue, sessionIdProvider);

        var order = new NewOrderSingle
        {
            Side = new Side(Side.BUY),
            Symbol = new Symbol("PETR4"),
            OrderQty = new OrderQty(100),
            Price = new Price(10m)
        };

        await orderQueue.EnqueueAsync(order);

        sessionIdProvider
            .SendToTarget(Arg.Any<NewOrderSingle>())
            .Throws(new Exception("Falha no envio"));

        // Act
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(500);
        await sut.StartAsync(cts.Token);
        await Task.Delay(200);

        // Assert
        var reEnqueuedOrder = await orderQueue.Reader.ReadAsync(cts.Token);
        Assert.Equal(order.Symbol.Value, reEnqueuedOrder.Symbol.Value);
        Assert.Equal(order.OrderQty.Value, reEnqueuedOrder.OrderQty.Value);
        Assert.Equal(order.Price.Value, reEnqueuedOrder.Price.Value);
    }
}
