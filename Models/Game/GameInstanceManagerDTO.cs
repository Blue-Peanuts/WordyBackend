namespace WordyBackend.Models.Game;

public class GameInstanceManagerDTO(GameInstanceManager gameInstanceManager)
{
    public IReadOnlyList<GameInstanceDTO> GameInstances =>
        gameInstanceManager.GetGameInstances().Select((instance) => new GameInstanceDTO(instance)).ToList();
}