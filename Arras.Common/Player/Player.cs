namespace Arras.Common.Player
{
    using System.Drawing;
    using Functional.Maybe;

    /// <summary>
    /// A basic player
    /// </summary>
    public abstract class Player
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="type">The type of player.</param>
        protected Player(string name, PlayerType type)
        {
            this.Name = name;
            this.PlayerType = type;
        }

        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The identifier of this player.
        /// <c>None</c> when this player has no profile.
        /// </summary>
        public Maybe<string> Id { get; set; }

        /// <summary>
        /// The type of player.
        /// </summary>
        public PlayerType PlayerType { get; set; }
    }
}
