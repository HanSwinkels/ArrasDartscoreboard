namespace Arras.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Common.Match;
    using Common.Player;
    using Functional.Maybe;

    /// <summary>
    /// A class that provides methods regarding players.
    /// </summary>
    public class PlayerService
    {
        /// <summary>
        /// The player for this service.
        /// </summary>
        public readonly Player Player;

        /// <summary>
        /// The list of legs that this player has played.
        /// </summary>
        private List<LegByPlayer> LegsByPlayer = new List<LegByPlayer>();

        public PlayerService(Player player)
        {
            this.Player = player;
        }

        /// <summary>
        /// Gets all the legs played during a match. Necessary since there are two types (i.e. sets and legs).
        /// </summary>
        /// <param name="match">The match from which to retrieve all the legs.</param>
        /// <returns>The legs played in the match</returns>
        private List<Leg> GetLegs(StandardMatch match)
        {
            return match.StandardMatchType == StandardMatchType.Legs ?
                match.LegsPlayed.Value : match.SetsPlayed.Value.SelectMany(y => y.Legs).ToList();                
        }

        /// <summary>
        /// Gets the data that is displayed in the <see cref="Player"/>.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public PlayerScoreItem GetPlayerScoreItem(StandardMatch match)
        {
            var legs = this.GetLegs(match);
            this.LegsByPlayer = this.GetLegsByPlayer(legs).ToList();

            var score = match.Format - this.LegsByPlayer.Last().Scores.Sum();

            var legsWon = 0;
            if (match.StandardMatchType == StandardMatchType.Sets)
            {
                legsWon = this.GetNumberWonLegsInSet(match);
            }
            else
            {
                legsWon = this.GetNumberWonLegs();
;            }

            return new PlayerScoreItem(
                this.Player.Name, 
                score, 
                GetThrowoutAdvice(score), 
                this.LegsByPlayer.Last().DartsThrown.Sum(), 
                GetAverage(),
                GetSetsWon(match),
                legsWon);
        }

        /// <summary>
        /// Gets the latest match stats for the player, except for a running leg.
        /// </summary>
        /// <param name="match">The match for which the stats have to be retrieved.</param>
        /// <param name="player">The player for which to retrieve the stats.</param>
        /// <remarks>Does not take running legs into account.</remarks>
        /// <returns>The latest match stats for the player.</returns>
        public PlayerMatchStats GetAllStats(StandardMatch match)
        {
            var legs = this.GetLegs(match);
            this.LegsByPlayer = this.GetLegsByPlayer(legs).Where(x => x.IsWon.HasValue).ToList();

            return new PlayerMatchStats
            {
                Average = this.GetAverage(),
                AverageBestLeg = this.GetAverageBestLeg(),
                AverageFirstNine = this.GetAverageFirstNine(),
                BestLeg = this.GetBestLeg(),
                Breaks = this.GetBreaks(),
                HighestCheckout = this.GetHighestCheckout(),
                HighestScore = this.GetHighestScore(),
                Legs = this.GetNumberWonLegs(),
                Sets = this.GetSetsWon(match),
                OneEighties = this.GetScoresAbove(180),
                Plus100 = this.GetScoresAbove(100),
                Plus140 = this.GetScoresAbove(140),
                Plus80 = this.GetScoresAbove(80),
                WorstLeg = this.GetWorstLeg()
            };
        }

        /// <summary>
        /// Gets the latest match stats for the player, except for a running leg.
        /// </summary>
        /// <param name="matches">The matches for which the stats have to be retrieved</param>
        /// <param name="player">The player for which to retrieve the stats</param>
        /// <remarks>Does not take running legs into account.</remarks>
        /// <returns>The latest match stats for the player.</returns>
        public PlayerMatchStats GetAllStats(List<StandardMatch> matches)
        {
            this.LegsByPlayer = matches.SelectMany(x => this.GetLegsByPlayer(this.GetLegs(x))).Where(x => x.IsWon.HasValue).ToList();

            return new PlayerMatchStats
            {
                Average = this.GetAverage(),
                AverageBestLeg = this.GetAverageBestLeg(),
                AverageFirstNine = this.GetAverageFirstNine(),
                BestLeg = this.GetBestLeg(),
                Breaks = this.GetBreaks(),
                HighestCheckout = this.GetHighestCheckout(),
                HighestScore = this.GetHighestScore(),
                Legs = this.GetNumberWonLegs(),
                Sets = this.GetSetsWon(matches),
                OneEighties = this.GetScoresAbove(180),
                Plus100 = this.GetScoresAbove(100),
                Plus140 = this.GetScoresAbove(140),
                Plus80 = this.GetScoresAbove(80),
                WorstLeg = this.GetWorstLeg()
            };
        }

        /// <summary>
        /// Gets the latest basic match stats for the player. Will not get any stats regarding leg information, such as best leg etc.
        /// </summary>
        /// <param name="match">The match from which to retrieve the basic stats.</param>
        /// <returns>The latest match stats for the player.</returns>
        public PlayerMatchStats GetBasicStats(StandardMatch match)
        {
            this.LegsByPlayer = this.GetLegsByPlayer(this.GetLegs(match));
            return new PlayerMatchStats
            {
                Average = this.GetAverage(),
                AverageFirstNine = this.GetAverageFirstNine(),
                HighestScore = this.GetHighestScore(),
                OneEighties = this.GetScoresAbove(180),
                Plus100 = this.GetScoresAbove(100),
                Plus140 = this.GetScoresAbove(140),
                Plus80 = this.GetScoresAbove(80),
            };
        }

        /// <summary>
        /// Gets a list of legs played by a player.
        /// </summary>
        /// <param name="legs">The list of legs from which to take</param>
        /// <returns>The legs played by the provided player</returns>
        private List<LegByPlayer> GetLegsByPlayer(List<Leg> legs)
        {
            return legs.SelectMany(x =>
                x.LegByPlayers.Where(l => l.PlayerId == this.Player.Id.OrElse(this.Player.Name))).ToList();
        }

        /// <summary>
        /// Gets the three dart average of a player.
        /// </summary>
        /// <returns>The three dart average of the player, rounded to two decimals.</returns>
        private double GetAverage()
        {
            if (this.LegsByPlayer.First().DartsThrown.Sum() <= 0)
                return 0.00;

            var totalScore = this.LegsByPlayer.SelectMany(x => x.Scores.Select(s => s)).Sum();
            var totalDarts = this.LegsByPlayer.Select(x => x.DartsThrown.Sum()).Sum() / 3.0;

            return Math.Round(totalScore / totalDarts, 2) * 1.00;
        }

        /// <summary>
        /// Gets the number of legs won by this player.
        /// </summary>
        /// <returns>The number of legs won by this player.</returns>
        private int GetNumberWonLegs() => this.LegsByPlayer.Count(x => x.IsWon.OrElse(false));

        /// <summary>
        /// Gets the number of legs won in this set by this player.
        /// </summary>
        /// <returns>The number of legs won by this player.</returns>
        private int GetNumberWonLegsInSet(StandardMatch match)
        {
            var legsInLastSet = match.SetsPlayed.Value.Last().Legs;
            var legsByPlayerInSet = legsInLastSet.SelectMany(x => x.LegByPlayers.Where(p => p.PlayerId == this.Player.Id.OrElse(this.Player.Name)));

            return legsByPlayerInSet.Count(l => l.IsWon.OrElse(false));
        }



        /// <summary>
        /// Gets the number of sets that are won by a player.
        /// </summary>
        /// <param name="match">The match for which the number of sets won has to be retrieved.</param>
        /// <returns>The number of sets won by the provided player.</returns>
        private int GetSetsWon(StandardMatch match)
        {
            if (match.StandardMatchType == StandardMatchType.Legs) return 0;

            var setsPlayed = match.SetsPlayed.Value;
            var listOfLegsInSet = setsPlayed.Select(x => x.Legs);

            var setsCount = 0;
            foreach (var set in listOfLegsInSet)
            {
                var legByPlayersInSet = set.Select(x =>
                        x.LegByPlayers.Where(p => p.PlayerId == this.Player.Id.OrElse(this.Player.Name)))
                    .SelectMany(x => x);

                if (legByPlayersInSet.Count(x => x.IsWon.OrElse(false)) == 3)
                {
                    setsCount++;
                }
            }

            return setsCount;
        }

        /// <summary>
        /// Gets the number of sets that are won by a player.
        /// </summary>
        /// <param name="matches">The matches for which the number of sets won has to be retrieved.</param>
        /// <returns>The number of sets won by the provided player.</returns>
        public int GetSetsWon(List<StandardMatch> matches)
        {
            //TODO Fix this method as above
            var setsByPlayer = matches.SelectMany(m => m.SetsPlayed.Value.Select(x => x.Legs.Select(l => l.LegByPlayers
                .FirstOrDefault(p => p.PlayerId == this.Player.Id.OrElse(this.Player.Name)))));

            return setsByPlayer.Count(s => s.Count(x => x.IsWon.OrElse(false)) == 3);
        }

        /// <summary>
        /// Gets the three dart average of a player over a leg.
        /// </summary>
        /// <param name="leg">A leg a player has thrown.</param>
        /// <returns>The three dart average of the player over the leg.</returns>
        private double GetAverage(LegByPlayer leg)
        {
            var totalScore = leg.Scores.Select(s => s).Sum();
            var totalDarts = leg.DartsThrown.Sum() / 3.0; // Since we want to compute three dart average.

            return Math.Round(totalScore / totalDarts, 2);
        }

        /// <summary>
        /// Gets the three dart average of a player over its best leg.
        /// </summary>
        /// <returns>The three dart average of the player over the best leg.</returns>
        private double GetAverageBestLeg()
        {
            return Math.Round(this.LegsByPlayer.First().Scores.Sum() / (this.GetBestLeg() / 3.0), 2);
        }

        /// <summary>
        /// Gets the three dart average of a player of the first nine darts of a leg.
        /// </summary>
        /// <param name="legs">The list of legs a player has thrown.</param>
        /// <returns>The three dart average of the player of the first nine darts.</returns>
        private double GetAverageFirstNine()
        {
            var totalScore = this.LegsByPlayer.SelectMany(x => x.Scores.Take(3).Select(s => s)).Sum();
            var totalDarts = this.LegsByPlayer.Select(x =>
            {
                if (x.DartsThrown.Sum() < 9)
                    return x.DartsThrown.Sum();
                else
                    return 9;
            }).Sum() / 3.0;

            return Math.Round(totalScore / totalDarts, 2);
        }

        /// <summary>
        /// Gets the number of scores greater than the provided score.
        /// </summary>
        /// <param name="score">The score for which the scores must be higher.</param>
        /// <returns>The number of scores that are greater than the provided score.</returns>
        private int GetScoresAbove(int score)
        {
            return this.LegsByPlayer.Select(x => x.Scores.Count(s => s >= score)).Sum();
        }

        /// <summary>
        /// Get the remaining score for the specified player in the specified match.
        /// </summary>
        /// <returns>The remaining score for the provided player in the match</returns>
        public int GetRemainingScore(StandardMatch match)
        {
            this.LegsByPlayer = this.GetLegsByPlayer(this.GetLegs(match));

            return match.Format - this.LegsByPlayer.Last().Scores.Sum();
        }

        /// <summary>
        /// Determines the highest score from a list of legs.
        /// </summary>
        /// <returns>The highest score of the player.</returns>
        private int GetHighestScore()
        {
            return this.LegsByPlayer.Select(x => x.Scores.Max()).Max();
        }

        /// <summary>
        /// Determines the highest checkout from a list of legs.
        /// </summary>
        /// <returns>The highest checkout of the player.</returns>
        private int GetHighestCheckout()
        {
            return this.LegsByPlayer.Where(x => x.IsWon.OrElse(false)).Select(x => x.Scores.Last()).Max();
        }

        /// <summary>
        /// Determines the best leg of list of legs.
        /// </summary>
        /// <returns>The best leg of the player.</returns>
        private int GetBestLeg()
        {
            return this.LegsByPlayer.Where(l => l.IsWon.OrElse(false)).Min(x => x.DartsThrown.Sum());
        }

        /// <summary>
        /// Determines the best leg of list of legs.
        /// </summary>
        /// <returns>The best leg of the player.</returns>
        private int GetWorstLeg()
        {
            return this.LegsByPlayer.Where(l => l.IsWon.OrElse(false)).Max(x => x.DartsThrown.Sum());
        }

        /// <summary>
        /// Gets the throwout advice for a score.
        /// </summary>
        /// <param name="score">The score which the throwout advice will be given.</param>
        /// <returns>The throwout advice for this score.</returns>
        private string GetThrowoutAdvice(int score)
        {
            return "T20 T20 BULL";
        }

        /// <summary>
        /// Gets the number of breaks in the list of legs. (i.e. the number of legs that weren't started by the player that won the).
        /// </summary>
        /// <returns></returns>
        private int GetBreaks()
        {
            return this.LegsByPlayer.Count(x => x.HasStarted && x.IsWon.OrElse(false));
        }
    }
}