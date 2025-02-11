namespace WordyBackend.Models.Game;

public class GameInstanceDTO(GameInstance gameInstance)
{
    public string Id { get; } = gameInstance.Id;
    public int AnswerLength { get; } = gameInstance.Answer.Length;
    public IReadOnlyList<GuessFeedback> GuessFeedbacks { get; } = gameInstance.GuessFeedbacks;
    public GameInstanceState State { get; } = gameInstance.State;
    public int Attempts { get; } = gameInstance.Attempts;
    public int MaxAttempts { get; } = gameInstance.MaxAttempts;
}