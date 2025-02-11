using System.Collections;

namespace WordyBackend.Models.Game;

public class GuessFeedback(string guess, IEnumerable<LetterGuessFeedback> feedbacks) : IReadOnlyList<LetterGuessFeedback>
{
    public string Guess { get; } = guess;
    
    private readonly List<LetterGuessFeedback> _feedbacks = feedbacks.ToList();

    public LetterGuessFeedback this[int index] => _feedbacks[index];

    public int Count => _feedbacks.Count;

    public IEnumerator<LetterGuessFeedback> GetEnumerator() => _feedbacks.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public bool AllCorrect => _feedbacks.All(f => f == LetterGuessFeedback.Correct);
}