using NSubstitute;
using OrderGenerator.Api.Services;
using OrderGenerator.Api.Services.Interfaces;

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
}
