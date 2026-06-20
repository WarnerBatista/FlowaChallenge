using FixCommons;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using OrderGenerator.Api.Dtos;
using OrderGenerator.Api.Hubs;
using OrderGenerator.Api.Services;
using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace OrderGenerator.UnitTests.Services;

public class OrderGeneratorApplicationTests
{
    [Fact]
    public void Construtor_Throws_When_HubContext_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(
            () => new OrderGeneratorApplication(null!, Substitute.For<ISessionIdProvider>()));
    }

    [Fact]
    public void Construtor_Throws_When_SessionIdProvider_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(
            () => new OrderGeneratorApplication(Substitute.For<IHubContext<OrderHub>>(), null!));
    }

    [Fact]
    public void OnLogon_Sets_SessionId_In_SessionIdProvider()
    {
        // Arrange
        var hubContext = Substitute.For<IHubContext<OrderHub>>();
        var sessionIdProvider = Substitute.For<ISessionIdProvider>();
        var sut = new OrderGeneratorApplication(hubContext, sessionIdProvider);
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");

        // Act
        sut.OnLogon(sessionId);

        // Assert
        sessionIdProvider.Received(1).SetSessionId(sessionId);
    }

    [Theory]
    [InlineData('0', "Executed", "Order executed successfully")]
    [InlineData('8', "Rejected", "Order rejected by exposure limit")]
    public void FromApp_Sends_OrderResponse_To_Hub_When_ExecutionReport_Is_Received(
        char execType,
        string status,
        string message)
    {
        // Arrange
        var hubContext = Substitute.For<IHubContext<OrderHub>>();
        var sessionIdProvider = Substitute.For<ISessionIdProvider>();
        var sut = new OrderGeneratorApplication(hubContext, sessionIdProvider);
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");
        var orderId = "12345";

        var report = new ExecutionReport
        {
            OrderID = new OrderID(orderId),
            ExecType = new ExecType(execType)
        };

        // Act
        sut.FromApp(report, sessionId);

        // Assert
        hubContext.Clients.Received(1).Group(orderId).SendAsync(
            "OrderStatusUpdated",
            Arg.Is<OrderResponse>(r => r.OrderId == orderId && r.Status == status && r.Message == message),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void FromApp_Throws_ArgumentException_When_ExecType_Is_Invalid()
    {
        // Arrange
        var hubContext = Substitute.For<IHubContext<OrderHub>>();
        var sessionIdProvider = Substitute.For<ISessionIdProvider>();
        var sut = new OrderGeneratorApplication(hubContext, sessionIdProvider);
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");
        var orderId = "12345";
        var execType = 'Z';

        var report = new ExecutionReport
        {
            OrderID = new OrderID(orderId),
            ExecType = new ExecType(execType)
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => sut.FromApp(report, sessionId));
    }
}
