namespace Arras.Common.Player
{
    /// <summary>
    /// The types of players there are to play a match.
    /// </summary>
    public enum PlayerType
    {
        /// <summary>
        /// A normal player who scores by manually throwing.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// A bot that scores determined by its level.
        /// </summary>
        Bot = 1,

        /// <summary>
        /// A bot that scores based on previous legs played by a player.
        /// </summary>
        Yourself = 2,

        /// <summary>
        /// Only used for debugging purposes.
        /// </summary>
        Debug = 3
    }
}