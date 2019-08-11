namespace Arras.Data.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    /// <summary>
    /// An entity that represents a standard match.
    /// </summary>
    public class StandardMatchEntity
    {
        /// <summary>
        /// Id represeting the match.
        /// </summary>
        [Key]
        public string MatchId { get; set; }

        /// <summary>
        /// The list of player ids that played in this match.
        /// </summary>
        public List<string> PlayerIds { get; set; }

        /// <summary>
        /// The list of ids of the legs played in this match.
        /// </summary>
        public List<string> LegIds { get; set; }
    }
}