using System.Diagnostics;

namespace WordyBackend.Models.Game;

public class GameInstance(string answer, int maxAttempts)
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public string Answer { get; } = answer;
    public int Attempts { get; private set; } = 0;
    public int MaxAttempts { get; } = maxAttempts;
    
    public IReadOnlyList<GuessFeedback> GuessFeedbacks => _guessFeedbacks;
    private readonly List<GuessFeedback> _guessFeedbacks = [];

    public GameInstanceState State { get; private set; } = GameInstanceState.Ongoing;

    public GuessFeedback Guess(string guess)
    {
        if (State != GameInstanceState.Ongoing)
        {
            throw new GameNotOngoingException();
        }
        if (guess.Length != Answer.Length)
        {
            throw new GuessLengthMismatchException();
        }
        
        Attempts++;

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
        if (guessFeedback.AllCorrect)
        {
            State = GameInstanceState.Won;
        }
        else if (Attempts >= MaxAttempts)
        {
            State = GameInstanceState.Lost;
        }
        else
        {
            State = GameInstanceState.Ongoing;
        }
        return guessFeedback;
    }
}