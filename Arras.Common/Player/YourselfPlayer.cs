namespace Arras.Common.Player
{
    using System;
    using System.Collections.Specialized;

    /// <summary>
    /// A player that represents yourself. Its level is based upon your previous thrown legs.
    /// </summary>
    public class YourselfPlayer : Player
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YourselfPlayer"/> class.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="level">The level at which this bot will score.</param>
        public YourselfPlayer(string name, YourselfLevel level) : base(name, PlayerType.Yourself)
        {
            this.Level = level;
        }
        /// <summary>
        /// The level of this player.
        /// </summary>
        public YourselfLevel Level { get; set; }
    }
}