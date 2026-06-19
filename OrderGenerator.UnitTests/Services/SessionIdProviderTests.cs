using OrderGenerator.Api.Services;
using QuickFix;

namespace OrderGenerator.UnitTests.Services;

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

        sut.TryGetSessionID(out var retrievedSessionId);

        // Assert
        Assert.Equal(sessionId, retrievedSessionId);
    }

    [Fact]
    public void TryGetSessionID_ShouldReturnFalse_WhenSessionIdNotSet()
    {
        // Arrange
        var sut = new SessionIdProvider();

        // Act
        var result = sut.TryGetSessionID(out var sessionId);

        // Assert
        Assert.False(result);
        Assert.Null(sessionId);
    }

    [Fact]
    public void TryGetSessionID_ShouldReturnTrue_WhenSessionIdSet()
    {
        // Arrange
        var sut = new SessionIdProvider();
        var sessionId = new SessionID("FIX.4.4", "SENDER", "TARGET");
        sut.SetSessionId(sessionId);

        // Act
        var result = sut.TryGetSessionID(out var retrievedSessionId);

        // Assert
        Assert.True(result);
        Assert.Equal(sessionId, retrievedSessionId);
    }
}
