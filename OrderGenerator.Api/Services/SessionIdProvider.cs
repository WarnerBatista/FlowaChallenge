using OrderGenerator.Api.Services.Interfaces;
using QuickFix;

namespace OrderGenerator.Api.Services;

public class SessionIdProvider : ISessionIdProvider
{
    private SessionID? _sessionId;
    public void SetSessionId(SessionID sessionId) => _sessionId = sessionId;

    public bool TryGetSessionID(out SessionID? sessionID)
    {
        sessionID = _sessionId;

        if (_sessionId == null)
            return false;
        else
            return true;
    }
}
