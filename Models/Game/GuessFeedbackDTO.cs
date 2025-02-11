using System.Collections;

namespace WordyBackend.Models.Game;

public class GuessFeedbackDTO(GuessFeedback guessFeedback) : IReadOnlyList<int>
{
    private readonly List<int> _feedbacks = guessFeedback.Select((feedback) => (int)feedback).ToList();

    public int this[int index] => _feedbacks[index];

    public int Count => _feedbacks.Count;

    public IEnumerator<int> GetEnumerator() => _feedbacks.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}