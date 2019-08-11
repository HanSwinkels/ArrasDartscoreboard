namespace Arras.Common.Player
{
    using System;

    /// <summary>
    /// A player that is a bot.
    /// </summary>
    public class BotPlayer : Player
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BotPlayer"/> class.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="level">The level at which this bot will score.</param>
        public BotPlayer(string name, BotLevel level) : base(name, PlayerType.Bot)
        {
            this.Level = level;
        }
        /// <summary>
        /// The level of this player.
        /// </summary>
        public BotLevel Level { get; set; }
    }
}