using System.Collections;

namespace WordyBackend.Models.Game;

public class GuessFeedback(string guess, IEnumerable<LetterGuessFeedback> feedbacks)
{
    /// <summary>
    /// Letter guess feedbacks with index corresponding to the guessed letter index.
    /// </summary>
    public readonly List<LetterGuessFeedback> LetterGuessFeedbacks = feedbacks.ToList();

    public string Guess { get; } = guess;
    
    public bool AllCorrect => LetterGuessFeedbacks.All(f => f == LetterGuessFeedback.Correct);
}