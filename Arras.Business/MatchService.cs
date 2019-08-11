namespace Arras.Business
{
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Common.Match;
    using Common.Player;
    using Data;
    using Functional.Maybe;

    /// <summary>
    /// A class that provides methods regarding a match.
    /// </summary>
    public class MatchService
    {
        /// <summary>
        /// The database with matches.
        /// </summary>
        private MatchRepository MatchDatabase;

        /// <summary>
        /// The match for which this service provides methods.
        /// </summary>
        private readonly StandardMatch StandardMatch;

        /// <summary>
        /// The player service containing methods for players.
        /// </summary>
        private readonly PlayerService playerService;

        /// <summary>
        /// Initializes a new instance of <see cref="MatchService"/>.
        /// </summary>
        /// <param name="match">The match for which this instance is created.</param>
        public MatchService(StandardMatch match)
        {
            this.MatchDatabase = new MatchRepository();
            this.playerService = new PlayerService();

            this.StandardMatch = match;
        }

        /// <summary>
        /// Enters a score for the player whose at turn.
        /// </summary>
        /// <param name="score">The score to enter.</param>
        public ScoreValidationType EnterScore(int score)
        {
            // TODO: Add checker for invalid values (e.g. > 180, 169, 159)
            var playerAtTurn = this.GetTurn();

            if (playerAtTurn.Score - score == 1 || playerAtTurn.Score - score < 0)
                return ScoreValidationType.Invalid;

            playerAtTurn.Score -= score;
            this.AddScore(score, playerAtTurn);

            if (playerAtTurn.Score == 0)
                return ScoreValidationType.EndsLeg;

            return ScoreValidationType.Valid;
        }

        /// <summary>
        /// Checks whether the target sets/legs has been reached by the player whom ended the leg and thus whether the game is finished.
        /// </summary>
        /// <param name="player">The player who ended the last leg.</param>
        /// <returns>Whether the game is finished.</returns>
        public bool IsGameFinished(Player player)
        {
            if (this.StandardMatch.StandardMatchType == StandardMatchType.Legs)
                return this.playerService.GetAllStats(this.StandardMatch, player).Legs == this.StandardMatch.Legs;
            else
                return this.playerService.GetAllStats(this.StandardMatch, player).Sets == this.StandardMatch.Sets.Value;
        }

        /// <summary>
        /// Ends the current leg that is being played.
        /// Sets the correct number of darts of the player who won the leg.
        /// Add a new leg to the list of legs/sets.
        /// Set the score of both players to the format.
        /// </summary>
        /// <param name="numDarts">The number of darts that took to finish the game.</param>
        /// <param name="player">The player that won the leg.</param>
        /// <returns>The leg that was ended.</returns>
        public Leg EndLeg(int numDarts, Player player)
        {
            this.GetLastLeg().LegByPlayers.First(p => p.PlayerId == player.Id.OrElse(player.Name)).DartsThrown -= (3 - numDarts);
            var endedLeg = this.GetLastLeg();
            if (this.StandardMatch.StandardMatchType == StandardMatchType.Legs)
                this.StandardMatch.LegsPlayed.Value.Add(new Leg());

            if (this.StandardMatch.StandardMatchType == StandardMatchType.Sets)
            {
                // Check if this leg finishes a set
                var legsWon = this.StandardMatch.SetsPlayed.Value.Last().Legs
                    .Select(x => x.LegByPlayers.Where(l => l.PlayerId == player.Id.OrElse(player.Name))).First()
                    .Count(p => p.IsWon.OrElse(false));

                if (legsWon == 3)
                {
                    this.StandardMatch.SetsPlayed.Value.Add(new Set());
                    this.StandardMatch.SetsPlayed.Value.Last().Legs = new List<Leg>();
                }
                else
                {
                    this.StandardMatch.SetsPlayed.Value.Last().Legs.Add(new Leg());
                }
            }

            return endedLeg;
        }

        /// <summary>
        /// Gets the player who is at turn during a leg.
        /// </summary>
        /// <returns>The player who is at turn in this leg.</returns>
        public Player GetTurn()
        {
            var numPlayers = this.StandardMatch.Players.Count;
            var turnsInLeg = this.GetLastLeg().LegByPlayers.Select(x => x.Scores.Count).Sum();

            if (this.StandardMatch.StandardMatchType == StandardMatchType.Legs)
            {
                return this.StandardMatch.Players.Skip((turnsInLeg + this.GetTotalLegs() - 1) % numPlayers).First();
            }
            else
            {
                var totalSets = this.StandardMatch.SetsPlayed.Value.Count;

                return totalSets % numPlayers == 0
                    ? this.StandardMatch.Players.Skip((this.GetLegsInSet() + turnsInLeg) % numPlayers).First()
                    : this.StandardMatch.Players.Skip((this.GetLegsInSet() + (numPlayers - 1) + turnsInLeg) % numPlayers).First();
            }
        }

        /// <summary>
        /// Gets the last leg that is currently being played in this match.
        /// </summary>
        /// <returns>The last leg that is currently being played in this match.</returns>
        public Leg GetLastLeg()
        {
            return this.StandardMatch.StandardMatchType == StandardMatchType.Legs
                ? this.StandardMatch.LegsPlayed.Value.Last()
                : this.StandardMatch.SetsPlayed.Value.Last().Legs.Last();
        }

        /// <summary>
        /// Adds a thrown score to a leg by a player.
        /// </summary>
        /// <param name="score">The score that was thrown.</param>
        /// <param name="player">The player that threw this score.</param>
        public void AddScore(int score, Player player)
        {
            var lastLegByPlayer = this.GetLastLeg().LegByPlayers
                .First(x => x.PlayerId == player.Id.OrElse(player.Name));

            lastLegByPlayer.Scores.Add(score);
            lastLegByPlayer.DartsThrown += 3;
        }

        /// <summary>
        /// Gets the player who starts the current leg.
        /// </summary>
        /// <returns>The player who started the current leg.</returns>
        public Player GetStartingPlayer()
        {
            var totalLegs = this.GetTotalLegs();

            if (this.StandardMatch.StandardMatchType == StandardMatchType.Legs)
                return this.StandardMatch.Players.Skip((totalLegs - 1) % this.StandardMatch.Players.Count).First();
            else
            {
                var totalSets = this.StandardMatch.SetsPlayed.Value.Count;

                return totalSets % 2 == 0
                    ? this.StandardMatch.Players.Skip(this.GetLegsInSet() % this.StandardMatch.Players.Count).First()
                    : this.StandardMatch.Players.Skip((this.GetLegsInSet() + 1) % this.StandardMatch.Players.Count).First();
            }
        }

        /// <summary>
        /// Gets the total number of legs in the current set.
        /// </summary>
        /// <returns>The total number of legs being played in the current set.</returns>
        private int GetLegsInSet()
        {
            return this.StandardMatch.SetsPlayed.Value.Last().Legs.Count;
        }

        /// <summary>
        /// Gets the total number of legs that are played in this match.
        /// </summary>
        /// <returns>The number of legs that are played in this match.</returns>
        public int GetTotalLegs()
        {
            return this.StandardMatch.StandardMatchType == StandardMatchType.Legs
                ? this.StandardMatch.LegsPlayed.Value.Count
                : this.StandardMatch.SetsPlayed.Value.Select(x => x.Legs.Count).Sum();
        }

        /// <summary>
        /// Will end the match and save it to the database.
        /// </summary>
        public void EndMatch()
        {
        }

        /// <summary>
        /// Saves the match to the database.
        /// </summary>
        public void SaveMatch()
        {

        }
    }
}