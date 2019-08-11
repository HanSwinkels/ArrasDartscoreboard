namespace Arras.Common.Match
{
    using System.Collections.Generic;

    /// <summary>
    /// A class representing a leg from one player.
    /// </summary>
    public class Leg
    {
        public Leg()
        {
            this.LegByPlayers = new List<LegByPlayer>();
        }
        /// <summary>
        /// A list of legs divided over the players. When there are two players this list will have size two.
        /// Thus size is equal to the number of players.
        /// </summary>
        public List<LegByPlayer> LegByPlayers { get; set; }
    }
}