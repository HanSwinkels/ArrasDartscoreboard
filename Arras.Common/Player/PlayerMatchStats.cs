namespace Arras.Common.Player
{
    using System.Collections.Generic;
    using Functional.Maybe;
    using Match;

    /// <summary>
    /// Class containing all the stats of a player.
    /// </summary>
    public class PlayerMatchStats
    {
        /// <summary>
        /// The three dart average.
        /// </summary>
        public double Average { get; set; }

        /// <summary>
        /// The three dart average for the first nine darts of each leg.
        /// </summary>
        public double AverageFirstNine { get; set; }

        /// <summary>
        /// The three dart average of the best leg.
        /// </summary>
        public double AverageBestLeg { get; set; }

        /// <summary>
        /// Number of scores >= 80 and lower than 100.
        /// </summary>
        public int Plus80 { get; set; }

        /// <summary>
        /// Number of scores >= 100 and lower than 140.
        /// </summary>
        public int Plus100 { get; set; }

        /// <summary>
        /// Number of scores >= 140 and lower than 180.
        /// </summary>
        public int Plus140 { get; set; }

        /// <summary>
        /// Number of 180s.
        /// </summary>
        public int OneEighties { get; set; }

        /// <summary>
        /// Number of sets won.
        /// </summary>
        public int Sets { get; set; }

        /// <summary>
        /// Number of legs won.
        /// </summary>
        public int Legs { get; set; }

        /// <summary>
        /// The number of breaks (legs won and not started by the player).
        /// </summary>
        public int Breaks { get; set; }

        /// <summary>
        /// The best leg in number of darts.
        /// </summary>
        public int BestLeg { get; set; }

        /// <summary>
        /// The worst leg in number of darts.
        /// </summary>
        public int WorstLeg { get; set; }

        /// <summary>
        /// The highest score.
        /// </summary>
        public int HighestScore { get; set; }

        /// <summary>
        /// The highest checkout.
        /// </summary>
        public int HighestCheckout { get; set; }
    }
}