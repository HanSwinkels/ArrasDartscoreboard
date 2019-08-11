using System.ComponentModel.DataAnnotations;

namespace Arras.Data.Entity
{
    // Entity representing a player
    public class PlayerEntity
    {
        /// <summary>
        /// Id representing this leg.
        /// </summary>
        [Key]
        public string PlayerId { get; set; }

        /// <summary>
        /// Represents the name of the player.
        /// </summary>
        public string PlayerName { get; set; }
    }
}