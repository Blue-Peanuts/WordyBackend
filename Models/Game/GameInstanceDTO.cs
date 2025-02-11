namespace WordyBackend.Models.Game;

public class GameInstanceDTO(GameInstance gameInstance)
{
    public string Id { get; set; } = gameInstance.Id;
    public int AnswerLength { get; set; } = gameInstance.Answer.Length;
    public IReadOnlyList<GuessFeedbackDTO> GuessFeedbacks => gameInstance.GuessFeedbacks.Select(gf => new GuessFeedbackDTO(gf)).ToList();
}