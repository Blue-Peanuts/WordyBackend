using WordyBackend.Models.Game;

namespace WordyBackend.Models;

public class UserSessionDTO(UserSession userSession)
{
    public string Id { get; set; } = userSession.Id;
    public GameInstanceManagerDTO GameInstanceManager => new(userSession.GameInstanceManager);
}