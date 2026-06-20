using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace FixCommons.Tests;

public class SessionIdProviderTests
{
    [Fact]
    public void SetSessionId_ShouldSetSessionId()
    {
        // Arrange
        var sut = new SessionIdProvider();
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");

        // Act
        sut.SetSessionId(sessionId);

        var prop = typeof(SessionIdProvider)
            .GetField("_sessionId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var currentSessionId = prop?.GetValue(sut);

        // Assert
        Assert.Equal(sessionId, currentSessionId);
    }

    [Fact]
    public void SendToTarget_ShouldThrow_WhenSessionIdNotSet()
    {
        // Arrange
        var sut = new SessionIdProvider();
        var message = new NewOrderSingle
        {
            Side = new Side(Side.BUY),
            Symbol = new Symbol("PETR4"),
            OrderQty = new OrderQty(100),
            Price = new Price(10m)
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sut.SendToTarget(message));
    }
}
