using QuickFix;

namespace OrderGenerator.Api.Services.Interfaces;

public interface ISessionIdProvider
{
    bool TryGetSessionID(out SessionID? sessionID);
    void SetSessionId(SessionID sessionId);
}
