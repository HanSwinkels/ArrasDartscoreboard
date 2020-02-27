namespace Arras.Common.Match
{
    using System.Collections.Generic;
    using Functional.Maybe;

    /// <summary>
    /// A class containing info about a leg from a certain player
    /// </summary>
    public class LegByPlayer
    {
        /// <summary>
        /// Initializes a new instance of <see cref="LegByPlayer"/>.
        /// </summary>
        /// <param name="id">The id of the player.</param>
        public LegByPlayer(string id)
        {
            this.PlayerId = id;
            this.Scores = new List<int>();
            this.DartsThrown = new List<int>();
        }

        /// <summary>
        /// The player that played this leg.
        /// </summary>
        public string PlayerId { get; set; }

        /// <summary>
        /// The list of scores that are thrown in this leg.
        /// </summary>
        public List<int> Scores { get; set; }

        /// <summary>
        /// The number of darts thrown in this leg.
        /// </summary>
        public List<int> DartsThrown { get; set; }

        /// <summary>
        /// Represents whether the leg was won by the player.
        /// <c>None</c> when the leg hasn't ended yet.
        /// </summary>
        public Maybe<bool> IsWon { get; set; }

        /// <summary>
        /// Represents whether the leg was started by the player.
        /// </summary>
        public bool HasStarted { get; set; }
    }
}