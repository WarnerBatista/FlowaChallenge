using NSubstitute;
using OrderAccumulator.Services;
using OrderAccumulator.Services.Interfaces;
using OrderAccumulator.UnitTests.Attributes;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace OrderAccumulator.UnitTests.Services;

public class SellExposureProcessorTests
{
    [Fact]
    public void Constructor_Throws_When_ExposureAccumulator_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new SellExposureProcessor(null));
    }

    [Theory, AutoNSubstitute]
    public void Process_ShouldSetExposure_And_ReturnsNewExecutionReport(
        IExposureAccumulator accumulator, NewOrderSingle order)
    {
        // Arrange
        var sut = new SellExposureProcessor(accumulator);

        const int qty = 100;
        const decimal price = 10m;
        string symbol = "PETR4";

        order.ClOrdID = new ClOrdID("123");
        order.Symbol = new Symbol(symbol);
        order.Side = new Side(Side.SELL);
        order.OrderQty = new OrderQty(qty);
        order.Price = new Price(price);

        var existingExposure = 1000000;
        var expectedExposure = existingExposure - (qty * price);

        accumulator.GetExposure(symbol).Returns(existingExposure);

        // Act
        var report = sut.Process(order);

        // Assert
        accumulator.Received(1).SetExposure(symbol, expectedExposure);
        Assert.NotNull(report);
        Assert.Equal(ExecType.NEW, report.ExecType.Value);
    }
}
