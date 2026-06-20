using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using OrderGenerator.Api.Dtos;
using OrderGenerator.Api.Hubs;
using OrderGenerator.Api.Services;
using OrderGenerator.Api.Services.Interfaces;
using OrderGenerator.UnitTests.Attributes;
using QuickFix.Fields;
using QuickFix.FIX44;
using System.Reflection;

namespace OrderGenerator.UnitTests.Services;

public class FixInitiatorServiceTests
{
    private readonly string _validConfigPath;

    public FixInitiatorServiceTests()
    {
        _validConfigPath = Path.Combine(AppContext.BaseDirectory, "Config", "Initiator.cfg"); ;
    }

    [Fact]
    public void Constructor_Throws_When_OrderGeneratorApplication_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(
            () => new FixInitiatorService(
                "Config.cfg",
                null!,
                Substitute.For<IOrderQueue>()));
    }

    [Fact]
    public void Constructor_Throws_When_OrderQueue_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(
            () => new FixInitiatorService(
                "Config.cfg",
                new OrderGeneratorApplication(Substitute.For<IHubContext<OrderHub>>(), Substitute.For<ISessionIdProvider>()),
                null!));
    }

    [Theory, AutoNSubstitute]
    public void Start_ShouldInitializeAndStartInitiator(IOrderQueue queue)
    {
        // Arrange
        var application = new OrderGeneratorApplication(Substitute.For<IHubContext<OrderHub>>(), Substitute.For<ISessionIdProvider>());
        var sut = new FixInitiatorService(_validConfigPath, application, queue);

        // Act
        sut.Start();

        var initiatorField = typeof(FixInitiatorService)
            .GetField("_initiator", BindingFlags.NonPublic | BindingFlags.Instance);

        var initiator = initiatorField!.GetValue(sut);

        var prop = initiator!.GetType()
            .GetProperty("IsStopped", BindingFlags.Public | BindingFlags.Instance);

        var isStopped = (bool)prop!.GetValue(initiator)!;

        // Assert
        Assert.NotNull(initiatorField);
        Assert.False(isStopped);

        sut.Stop();
    }

    [Theory, AutoNSubstitute]
    public void Stop_ShouldStopInitiator(IOrderQueue queue)
    {
        // Arrange
        var application = new OrderGeneratorApplication(Substitute.For<IHubContext<OrderHub>>(), Substitute.For<ISessionIdProvider>());
        var sut = new FixInitiatorService("config/Initiator.cfg", application, queue);
        sut.Start();

        // Act
        sut.Stop();

        var initiatorField = typeof(FixInitiatorService)
            .GetField("_initiator", BindingFlags.NonPublic | BindingFlags.Instance);

        var initiator = initiatorField!.GetValue(sut);

        var prop = initiator!.GetType()
            .GetProperty("IsStopped", BindingFlags.Public | BindingFlags.Instance);

        var isStopped = (bool)prop!.GetValue(initiator)!;

        // Assert
        Assert.NotNull(initiatorField);
        Assert.True(isStopped);
    }

    [Theory, AutoNSubstitute]
    public void SendOrder_ShouldEnqueueOrderAndReturnResponse(IOrderQueue queue)
    {
        // Arrange
        var application = new OrderGeneratorApplication(Substitute.For<IHubContext<OrderHub>>(), Substitute.For<ISessionIdProvider>());
        var sut = new FixInitiatorService("config/Initiator.cfg", application, queue);
        var orderRequest = new OrderRequest
        (
            "AAPL",
            "BUY",
            10,
            150.00m
        );

        // Act
        var response = sut.SendOrder(orderRequest);

        // Assert
        queue.Received(1).EnqueueAsync(Arg.Any<NewOrderSingle>());
        Assert.NotNull(response);
        Assert.Equal("Order received and is being processed.", response.Message);
        Assert.Equal("Pending", response.Status);
    }

    [Fact]
    public void SendOrder_ShouldGenerateUniqueOrderIds()
    {
        // Arrange
        var application = new OrderGeneratorApplication(Substitute.For<IHubContext<OrderHub>>(), Substitute.For<ISessionIdProvider>());
        var queue = Substitute.For<IOrderQueue>();
        var sut = new FixInitiatorService("config/Initiator.cfg", application, queue);
        var orderRequest = new OrderRequest
        (
            "AAPL",
            "BUY",
            10,
            150.00m
        );

        // Act
        var response1 = sut.SendOrder(orderRequest);
        var response2 = sut.SendOrder(orderRequest);

        // Assert
        Assert.NotEqual(response1.OrderId, response2.OrderId);
    }

    [Theory]
    [InlineData("BUY", '1')]
    [InlineData("SELL", '2')]
    public void GetSide_ShouldReturnCorrectSide(string sideInput, char expectedSide)
    {
        // Arrange
        var application = new OrderGeneratorApplication(Substitute.For<IHubContext<OrderHub>>(), Substitute.For<ISessionIdProvider>());
        var queue = Substitute.For<IOrderQueue>();
        var sut = new FixInitiatorService("config/Initiator.cfg", application, queue);

        // Act
        var side = typeof(FixInitiatorService)
            .GetMethod("GetSide", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, new object[] { sideInput }) as Side;

        // Assert
        Assert.NotNull(side);
        Assert.Equal(expectedSide, side.Value);
    }

    [Fact]
    public void GetSide_ShouldThrow_WhenSideIsInvalid()
    {
        // Arrange
        var application = new OrderGeneratorApplication(Substitute.For<IHubContext<OrderHub>>(), Substitute.For<ISessionIdProvider>());
        var queue = Substitute.For<IOrderQueue>();
        var sut = new FixInitiatorService("config/Initiator.cfg", application, queue);

        // Act & Assert
        Assert.Throws<TargetInvocationException>(() =>
            typeof(FixInitiatorService)
                .GetMethod("GetSide", BindingFlags.NonPublic | BindingFlags.Static)!
                .Invoke(null, new object[] { "INVALID" }));
    }
}
