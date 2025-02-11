namespace WordyBackend.Models.Game;

public class GuessLengthMismatchException() : GameException("The guess length does not match the word length.");