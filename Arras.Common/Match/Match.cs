namespace Arras.Common.Match
{
    using System.Collections.Generic;
    using Player;

    /// <summary>
    /// A class containing the basic item of a match.
    /// </summary>
    public class Match
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Match"/> class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="players"></param>
        public Match(MatchType type, List<Player> players)
        {
            this.MatchType = type;
            this.Players = players;
        }

        /// <summary>
        /// The type of match.
        /// </summary>
        public MatchType MatchType { get; set; }

        /// <summary>
        /// The list of players that play in this match.
        /// </summary>
        public List<Player> Players { get; set; }
    }
}