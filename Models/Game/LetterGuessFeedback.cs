using Swashbuckle.AspNetCore.Annotations;

namespace WordyBackend.Models.Game;


/// <summary>
/// Feedback for a guessed letter. 0 = Correct, 1 = Misplaced, 2 = Absent.
/// </summary>
public enum LetterGuessFeedback
{
    Correct = 0,
    Misplaced = 1,
    Absent = 2
}