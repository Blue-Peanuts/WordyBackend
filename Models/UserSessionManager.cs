namespace WordyBackend.Models;

public class UserSessionManager
{
    private readonly Dictionary<string, UserSession> _sessions = new();

    public UserSession CreateSession(string userId)
    {
        var session = new UserSession();
        _sessions.Add(session.Id, session);

        return session;
    }

    public void RemoveSession(string sessionId)
    {
        _sessions.Remove(sessionId);
    }
    
    public UserSession? TryGetSession(string sessionId)
    {
        return _sessions.GetValueOrDefault(sessionId);
    }
}