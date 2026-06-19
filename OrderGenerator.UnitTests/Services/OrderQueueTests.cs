using OrderGenerator.Api.Services;
using OrderGenerator.UnitTests.Attributes;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace OrderGenerator.UnitTests.Services;

public class OrderQueueTests
{
    [Theory, AutoNSubstitute]
    public async Task Enqueue_ShouldAddOrderToQueue(NewOrderSingle order)
    {
        // Arrange
        var sut = new OrderQueue();

        // Act
        await sut.EnqueueAsync(order);

        // Assert
        var readOrder = await sut.Reader.ReadAsync();
        Assert.Equal(order, readOrder);
    }

    [Theory, AutoNSubstitute]
    public async Task EnqueueAsync_SholdReadInQueueSequence(NewOrderSingle order1, NewOrderSingle order2)
    {
        // Arrange
        var queue = new OrderQueue();
        const int qty = 100;
        const decimal price = 10m;
        string symbol = "PETR4";

        order1.ClOrdID = new ClOrdID("123");
        order1.Symbol = new Symbol(symbol);
        order1.Side = new Side(Side.BUY);
        order1.OrderQty = new OrderQty(qty);
        order1.Price = new Price(price);

        order2.ClOrdID = new ClOrdID("321");
        order2.Symbol = new Symbol(symbol);
        order2.Side = new Side(Side.BUY);
        order2.OrderQty = new OrderQty(qty);
        order2.Price = new Price(price);

        // Act
        await queue.EnqueueAsync(order1);
        await queue.EnqueueAsync(order2);

        // Assert
        var read1 = await queue.Reader.ReadAsync();
        var read2 = await queue.Reader.ReadAsync();

        Assert.Equal(order1, read1);
        Assert.Equal(order2, read2);
    }
}
