using OrderAccumulator.Extensions;
using OrderAccumulator.UnitTests.Attributes;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace OrderAccumulator.UnitTests.Extensions;

public class NewOrderSingleExtensionsTests
{
    [Theory, AutoNSubstitute]
    public void GenerateNewExecutionReport_ShouldReturnExecutionReportWithNewStatus(NewOrderSingle order)
    {
        // Arrange
        order.ClOrdID = new ClOrdID("123");
        order.Symbol = new Symbol("PETR4");
        order.Side = new Side(Side.BUY);
        order.OrderQty = new OrderQty(100);
        order.Price = new Price(10);

        // Act
        var report = order.GenerateNewExecutionReport();

        // Assert
        Assert.Equal(order.ClOrdID.Value, report.OrderID.Value);
        Assert.Equal(ExecType.NEW, report.ExecType.Value);
        Assert.Equal(OrdStatus.NEW, report.OrdStatus.Value);
        Assert.Equal(order.Symbol.Value, report.Symbol.Value);
        Assert.Equal(order.Side.Value, report.Side.Value);
        Assert.Equal(order.OrderQty.Value, report.CumQty.Value);
        Assert.Equal(order.Price.Value, report.AvgPx.Value);
    }

    [Theory, AutoNSubstitute]
    public void GenerateRejectedExecutionReport_ShouldReturnExecutionReportWithExpectedValues(
    NewOrderSingle order)
    {
        // Arrange
        order.ClOrdID = new ClOrdID("456");
        order.Symbol = new Symbol("VALE3");
        order.Side = new Side(Side.SELL);
        order.OrderQty = new OrderQty(50);
        order.Price = new Price(20);

        // Act
        var report = order.GenerateRejectedExecutionReport();

        // Assert
        Assert.Equal(order.ClOrdID.Value, report.OrderID.Value);
        Assert.Equal(ExecType.REJECTED, report.ExecType.Value);
        Assert.Equal(OrdStatus.REJECTED, report.OrdStatus.Value);
        Assert.Equal(order.Symbol.Value, report.Symbol.Value);
        Assert.Equal(order.Side.Value, report.Side.Value);
        Assert.Equal(order.OrderQty.Value, report.CumQty.Value);
        Assert.Equal(order.Price.Value, report.AvgPx.Value);
    }
}
