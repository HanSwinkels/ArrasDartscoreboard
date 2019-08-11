using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Functional.Maybe;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Arras.Data.Entity
{
    /// <summary>
    /// Entity representing a leg
    /// </summary>
    public class LegEntity
    {
        /// <summary>
        /// Id representing this leg.
        /// </summary>
        [Key]
        public string LegId { get; set; }

        /// <summary>
        /// The date time the leg was started.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Represents the player who played this leg.
        /// </summary>
        public string PlayerId { get; set; }

        /// <summary>
        /// The match in which this leg was played.
        /// </summary>
        public string MatchId { get; set; }

        /// <summary>
        /// The list of scores that are thrown in this leg.
        /// </summary>
        public List<int> Scores { get; set; }

        /// <summary>
        /// The number of darts thrown in this leg.
        /// </summary>
        public int DartsThrown { get; set; }

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
