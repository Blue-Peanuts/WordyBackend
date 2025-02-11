namespace WordyBackend.Models.Game;

public abstract class GameException : Exception
{
    protected GameException()
    {
    }

    protected GameException(string message) : base(message)
    {
    }
}