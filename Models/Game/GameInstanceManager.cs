namespace WordyBackend.Models.Game;

public class GameInstanceManager
{
    private readonly Dictionary<string, GameInstance> _gameInstances = new();
    
    public void AddGameInstance(GameInstance gameInstance)
    {
        _gameInstances.Add(gameInstance.Id, gameInstance);
    }
    
    public void RemoveGameInstance(string gameInstanceId)
    {
        _gameInstances.Remove(gameInstanceId);
    }
    
    public GameInstance? TryGetGameInstance(string gameInstanceId)
    {
        return _gameInstances.GetValueOrDefault(gameInstanceId);
    }
    
    public IReadOnlyList<GameInstance> GetGameInstances()
    {
        return _gameInstances.Values.ToList();
    }
}