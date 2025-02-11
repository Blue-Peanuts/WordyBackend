using System.Text.Json.Serialization;

namespace WordyBackend.Models.Game;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameInstanceState
{
    /// <summary>
    /// The game is in ongoing.
    /// </summary>
    Ongoing,
    /// <summary>
    /// The game has been won by the player.
    /// </summary>
    Won,
    /// <summary>
    /// The game has been lost by the player.
    /// </summary>
    Lost
}