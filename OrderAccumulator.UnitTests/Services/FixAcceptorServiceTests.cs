using OrderAccumulator.Services;
using OrderAccumulator.UnitTests.Attributes;
using System.Reflection;

namespace OrderAccumulator.UnitTests.Services;

public class FixAcceptorServiceTests
{
    [Fact]
    public void Constructor_Throws_When_Application_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new FixAcceptorService("config.cfg", null!));
    }

    [Theory, AutoNSubstitute]
    public void Start_ShouldInitializeAndStartAcceptor(OrderAccumulatorApplication application)
    {
        // Arrange
        var sut = new FixAcceptorService("Config/Acceptor.cfg", application);

        // Act
        sut.Start();

        var acceptorField = typeof(FixAcceptorService)
            .GetField("_acceptor", BindingFlags.NonPublic | BindingFlags.Instance);

        var acceptor = acceptorField!.GetValue(sut);

        var prop = acceptor!.GetType()
            .GetField("_isStarted", BindingFlags.NonPublic | BindingFlags.Instance);

        var isStarted = (bool)prop!.GetValue(acceptor)!;

        // Assert
        Assert.NotNull(acceptorField);
        Assert.True(isStarted);

        sut.Stop();
    }

    [Theory, AutoNSubstitute]
    public void Stop_ShouldStopAcceptor(OrderAccumulatorApplication application)
    {
        // Arrange
        var sut = new FixAcceptorService("Config/Acceptor.cfg", application);

        sut.Start();

        // Act
        sut.Stop();

        var acceptorField = typeof(FixAcceptorService)
            .GetField("_acceptor", BindingFlags.NonPublic | BindingFlags.Instance);

        var acceptor = acceptorField!.GetValue(sut);

        var prop = acceptor!.GetType()
            .GetField("_isStarted", BindingFlags.NonPublic | BindingFlags.Instance);

        var isStarted = (bool)prop!.GetValue(acceptor)!;

        // Assert
        Assert.NotNull(acceptorField);
        Assert.False(isStarted);
    }
}
