using System.Diagnostics;

namespace WordyBackend.Models.Game;

public class GameInstance(string answer, int maxAttempts)
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string Answer { get; set; } = answer;
    public int Attempts { get; set; } = 0;
    public int MaxAttempts { get; set; } = maxAttempts;
    
    public IReadOnlyList<GuessFeedback> GuessFeedbacks => _guessFeedbacks;
    private readonly List<GuessFeedback> _guessFeedbacks = [];

    public GuessFeedback Guess(string guess)
    {
        Attempts++;
        if (guess.Length != Answer.Length)
        {
            throw new ArgumentException("Guess must be the same length as the answer.");
        }

        List<LetterGuessFeedback> feedback = [];
        for (int i = 0; i < Answer.Length; i++)
        {
            feedback.Add(
                Answer[i] == guess[i] ? LetterGuessFeedback.Correct :
                Answer.Contains(guess[i]) ? LetterGuessFeedback.Misplaced :
                LetterGuessFeedback.Absent
            );
        }
        
        var guessFeedback = new GuessFeedback(guess, feedback);
        _guessFeedbacks.Add(guessFeedback);
        return guessFeedback;
    }
}