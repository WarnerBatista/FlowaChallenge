using FixCommons;
using NSubstitute;
using OrderAccumulator.Services;
using OrderAccumulator.Services.Interfaces;
using OrderAccumulator.UnitTests.Attributes;
using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace OrderAccumulator.UnitTests.Services;

public class OrderAccumulatorApplicationTests
{
    [Fact]
    public void Constructor_Throws_When_ExposureProcessors_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new OrderAccumulatorApplication(null!, Substitute.For<ISessionIdProvider>()));
    }

    [Fact]
    public void Constructor_Throws_When_SessionIdProvider_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new OrderAccumulatorApplication(new Dictionary<char, IExposureProcessor>(), null!));
    }

    [Fact]
    public void FromApp_ShouldProcessNewOrderSingle_And_SendExecutionReport()
    {
        // Arrange
        var exposureProcessors = new Dictionary<char, IExposureProcessor>
        {
            { '1', Substitute.For<IExposureProcessor>() },
            { '2', Substitute.For<IExposureProcessor>() }
        };

        var sessionIdProvider = Substitute.For<ISessionIdProvider>();

        var sut = new OrderAccumulatorApplication(exposureProcessors, sessionIdProvider);

        var order = new NewOrderSingle
        {
            Side = new Side(Side.BUY),
            Symbol = new Symbol("PETR4"),
            OrderQty = new OrderQty(100),
            Price = new Price(10m)
        };

        var sessionID = new SessionID("FIX.4.4", "SENDER", "TARGET");
        var expectedReport = new ExecutionReport();
        exposureProcessors['1'].Process(order).Returns(expectedReport);
        sessionIdProvider.SendToTarget(expectedReport).Returns(true);

        // Act
        sut.FromApp(order, sessionID);

        // Assert
        exposureProcessors['1'].Received(1).Process(order);
    }

    [Theory, AutoNSubstitute]
    public void FromApp_ShouldThrow_When_NoProcessorFound(ISessionIdProvider sessionIdProvider)
    {
        // Arrange
        var exposureProcessors = new Dictionary<char, IExposureProcessor>
        {
            { '1', Substitute.For<IExposureProcessor>() },
            { '2', Substitute.For<IExposureProcessor>() }
        };
        var sut = new OrderAccumulatorApplication(exposureProcessors, sessionIdProvider);

        var order = new NewOrderSingle
        {
            Side = new Side('X'),
            Symbol = new Symbol("PETR4"),
            OrderQty = new OrderQty(100),
            Price = new Price(10m)
        };

        var sessionID = new SessionID("FIX.4.4", "SENDER", "TARGET");

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => sut.FromApp(order, sessionID));
    }
}
