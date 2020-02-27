namespace Arras.Common.Player
{
    public class PlayerScoreItem
    {
        /// <summary>
        /// Initial class PlayerScoreItem.
        /// </summary>
        /// <param name="score">The current score.</param>
        /// <param name="throwout">Throwout advice.</param>
        /// <param name="dartsThrown">Darts thrown in the current leg.</param>
        /// <param name="average">The overall average.</param>
        /// <param name="sets">Number of sets this player has won.</param>
        /// <param name="legs">Number of legs this player has won.</param>
        public PlayerScoreItem(string name, int score, string throwout, int dartsThrown, double average, int sets, int legs)
        {
            this.Name = name;
            this.Score = score;
            this.Throwout = throwout;
            this.DartsThrown = dartsThrown;
            this.Average = average;
            this.Sets = sets;
            this.Legs = legs;
        }

        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current score that this players has left.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// The throwout advice.
        /// </summary>
        public string Throwout { get; set; }

        /// <summary>
        /// The number of darts thrown in the current leg.
        /// </summary>
        public int DartsThrown { get; set; }

        /// <summary>
        /// The three dart average.
        /// </summary>
        public double Average { get; set; }

        /// <summary>
        /// Number of sets won.
        /// </summary>
        public int Sets { get; set; }

        /// <summary>
        /// Number of legs won.
        /// </summary>
        public int Legs { get; set; }

    }
}