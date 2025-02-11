using WordyBackend.Models.Game;

namespace WordyBackend.Models;

public class UserSession
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public GameInstanceManager GameInstanceManager { get; } = new();
}