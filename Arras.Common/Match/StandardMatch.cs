namespace Arras.Common.Match
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Functional.Maybe;
    using Player;

    /// <summary>
    /// A class representing a standard match.
    /// </summary>
    public class StandardMatch : Match
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StandardMatch"/> class.
        /// </summary>
        /// <param name="type">The type of standard match e.g. legs or sets.</param>
        /// <param name="sets">The number of sets that have to be played. <c>None</c> when the <paramref name="type"/> <see cref="StandardMatchType.Legs"/>.</param>
        /// <param name="legs">The number of legs that have to be played.</param>
        /// <param name="format">The format of the match.</param>
        /// <param name="players">A list of players that participates in this match.</param>
        public StandardMatch(StandardMatchType type, Maybe<int> sets, int legs, int format, List<Player> players) 
            : base (MatchType.Standard, players)
        {
            if(type == StandardMatchType.Legs && sets != Maybe<int>.Nothing)
                throw new InvalidOperationException("Cannot create a legs matchtype with also sets.");

            if (type == StandardMatchType.Sets && sets == Maybe<int>.Nothing)
                throw new InvalidOperationException("Cannot create a sets matchtype without a number of sets.");

            this.StandardMatchType = type;
            this.Legs = legs;
            this.Format = format;

            if (type == StandardMatchType.Legs)
                this.LegsPlayed = new List<Leg>().ToMaybe();
            else
                this.SetsPlayed = new List<Set>().ToMaybe();
        }

        /// <summary>
        /// The match type, i.e. sets or legs.
        /// </summary>
        public StandardMatchType StandardMatchType { get; set; }

        /// <summary>
        /// The number of sets that have to be played. <c>None</c> when <see cref="StandardMatchType.Legs"/>
        /// </summary>
        public Maybe<int> Sets { get; set; }

        /// <summary>
        /// The number legs that have to reached for the match to end.
        /// If <see cref="StandardMatchType.Sets"/> this value is the number of legs to win a set.
        /// </summary>
        public int Legs { get; set; }

        /// <summary>
        /// The format that is played, e.g. 501
        /// </summary>
        public int Format { get; set; }

        /// <summary>
        /// A list of legs that are played.
        /// <c>None</c> when <see cref="StandardMatchType.Sets"/>.
        /// </summary>
        public Maybe<List<Leg>> LegsPlayed { get; set; }

        /// <summary>
        /// A list of legs that are played.
        /// <c>None</c> when <see cref="StandardMatchType.Legs"/>.
        /// </summary>
        public Maybe<List<Set>> SetsPlayed { get; set; }
    }
}