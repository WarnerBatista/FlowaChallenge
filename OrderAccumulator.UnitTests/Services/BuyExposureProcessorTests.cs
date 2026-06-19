using NSubstitute;
using OrderAccumulator.Services;
using OrderAccumulator.Services.Interfaces;
using OrderAccumulator.UnitTests.Attributes;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace OrderAccumulator.UnitTests.Services;

public class BuyExposureProcessorTests
{
    [Fact]
    public void Constructor_Throws_When_ExposureAccumulator_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new BuyExposureProcessor(null));
    }

    [Theory, AutoNSubstitute]
    public void Process_ShouldSetExposure_And_ReturnsNewExecutionReport_When_UnderMaxExposure(
        IExposureAccumulator accumulator, NewOrderSingle order)
    {
        // Arrange
        var sut = new BuyExposureProcessor(accumulator);

        const int qty = 100;
        const decimal price = 10m;
        string symbol = "PETR4";

        order.ClOrdID = new ClOrdID("123");
        order.Symbol = new Symbol(symbol);
        order.Side = new Side(Side.BUY);
        order.OrderQty = new OrderQty(qty);
        order.Price = new Price(price);

        var existingExposure = 0;
        var expectedExposure = qty * price + existingExposure;

        accumulator.GetExposure(symbol).Returns(existingExposure);
        accumulator.IsLessThanMaxExposure(expectedExposure).Returns(true);

        // Act
        var report = sut.Process(order);

        // Assert
        accumulator.Received(1).SetExposure(symbol, expectedExposure);
        Assert.NotNull(report);
        Assert.Equal(ExecType.NEW, report.ExecType.Value);
    }

    [Theory, AutoNSubstitute]
    public void Process_ShouldNotSetExposure_And_ReturnsRejectedExecutionReport_When_AtOrAboveMaxExposure(
        IExposureAccumulator accumulator, NewOrderSingle order)
    {
        // Arrange
        var sut = new BuyExposureProcessor(accumulator);

        const int qty = 100;
        const decimal price = 10m;
        string symbol = "PETR4";

        order.ClOrdID = new ClOrdID("123");
        order.Symbol = new Symbol(symbol);
        order.Side = new Side(Side.BUY);
        order.OrderQty = new OrderQty(qty);
        order.Price = new Price(price);

        var existingExposure = 100000000;
        var expectedExposure = qty * price + existingExposure;

        accumulator.GetExposure(symbol).Returns(existingExposure);
        accumulator.IsLessThanMaxExposure(expectedExposure).Returns(false);

        // Act
        var report = sut.Process(order);

        // Assert
        accumulator.DidNotReceive().SetExposure(Arg.Any<string>(), Arg.Any<decimal>());
        Assert.NotNull(report);
        Assert.Equal(ExecType.REJECTED, report.ExecType.Value);
    }
}
