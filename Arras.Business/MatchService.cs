namespace Arras.Business
{
    using System;
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
        public StandardMatch StandardMatch;

        /// <summary>
        /// The player service of each player, containing all methods for the players.
        /// </summary>
        public readonly List<PlayerService> playerServices;

        /// <summary>
        /// Initializes a new instance of <see cref="MatchService"/>.
        /// </summary>
        /// <param name="match">The match for which this instance is created.</param>
        public MatchService(StandardMatch match)
        {
            this.StandardMatch = match;
            foreach (var player in match.Players)
            {
                this.playerServices.Add(new PlayerService(player));
            }
        }

        /// <summary>
        /// List of scores that a player is unable to throw.
        /// </summary>
        private readonly List<int> InvalidScores = new List<int>(){169, 166, 172, 173, 175, 176, 178, 179};

        /// <summary>
        /// Enters a score for the player whose at turn.
        /// </summary>
        /// <param name="score">The score to enter.</param>
        public ScoreValidationType EnterScore(int score, Player player)
        {
            var playerService = this.playerServices.First(x => x.Player == player);
            var remainingScore = playerService.GetRemainingScore(this.StandardMatch);

            // Check if the score is valid.
            if (InvalidScores.Exists(x => x == score) || score > 180 || remainingScore - score == 1 || remainingScore - score < 0)
                return ScoreValidationType.Invalid;


            this.AddScore(score, player);

            if (remainingScore - score == 0)
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
            var playerService = this.playerServices.First(x => x.Player == player);

            if (this.StandardMatch.StandardMatchType == StandardMatchType.Legs)
                return playerService.GetAllStats(this.StandardMatch).Legs == this.StandardMatch.Legs;
            else
                return playerService.GetAllStats(this.StandardMatch).Sets == this.StandardMatch.Sets.Value;
        }

        /// <summary>
        /// Ends the current leg that is being played.
        /// Sets the correct number of darts of the player who won the leg.
        /// Add a new leg to the list of legs/sets.
        /// Set the score of both players to the format.
        /// </summary>
        /// <param name="numDarts">The number of darts that took to finish the game.</param>
        /// <returns>The leg that was ended.</returns>
        public Leg EndLeg(int numDarts)
        {
            var endedLeg = this.GetCurrentLeg();

            var legByPlayer = endedLeg.LegByPlayers.First(p => p.Scores.Sum() == this.StandardMatch.Format);

            // Correct the number of darts thrown depending on the number of darts the player finished with.
            legByPlayer.DartsThrown[legByPlayer.DartsThrown.Count - 1] = numDarts;

            // Set that the player has won this leg
            legByPlayer.IsWon = true.ToMaybe();

            if (this.StandardMatch.StandardMatchType == StandardMatchType.Legs)
                this.InitializeGame();

            if (this.StandardMatch.StandardMatchType == StandardMatchType.Sets)
            {
                // Check if this leg finishes a set
                var legsWon = this.StandardMatch.SetsPlayed.Value.Last().Legs
                    .SelectMany(x => x.LegByPlayers.Where(l => l.PlayerId == legByPlayer.PlayerId))
                    .Count(p => p.IsWon.OrElse(false));

                if (legsWon == 3)
                {
                    this.InitializeGame();
                }
                else
                {
                    this.StandardMatch.SetsPlayed.Value.Last().Legs.Add(new Leg());
                    this.StandardMatch.Players.ForEach(x => this.GetCurrentLeg().LegByPlayers.Add(new LegByPlayer(x.Id.OrElse(x.Name))));

                }
            }

            return endedLeg;
        }

        /// <summary>
        /// Initializes the game
        /// </summary>
        public void InitializeGame()
        {
            if(this.StandardMatch.StandardMatchType == StandardMatchType.Legs)
            {
                this.StandardMatch.LegsPlayed.Value.Add(new Leg());
                this.StandardMatch.Players.ForEach(x => this.GetCurrentLeg().LegByPlayers.Add(new LegByPlayer(x.Id.OrElse(x.Name))));
            }
            else
            {
                this.StandardMatch.SetsPlayed.Value.Add(new Set());
                this.StandardMatch.SetsPlayed.Value.Last().Legs.Add(new Leg());
                this.StandardMatch.Players.ForEach(x => this.GetCurrentLeg().LegByPlayers.Add(new LegByPlayer(x.Id.OrElse(x.Name))));
            }

            

        }

        /// <summary>
        /// Gets the player who is at turn during a leg.
        /// </summary>
        /// <returns>The player who is at turn in this leg.</returns>
        public Player GetTurn()
        {
            var numPlayers = this.StandardMatch.Players.Count;
            var turnsInLeg = this.GetCurrentLeg().LegByPlayers.Select(x => x.Scores.Count).Sum();

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
        public Leg GetCurrentLeg()
        {
            if (this.StandardMatch.StandardMatchType == StandardMatchType.Legs)
            {
                if (this.StandardMatch.LegsPlayed.Value.Count <= 0)
                    this.StandardMatch.LegsPlayed.Value.Add(new Leg());

                return this.StandardMatch.LegsPlayed.Value.Last();
            }
            else
            {
                if (this.StandardMatch.SetsPlayed.Value.Last().Legs.Count <= 0)
                    this.StandardMatch.SetsPlayed.Value.Last().Legs.Add(new Leg());

                return this.StandardMatch.SetsPlayed.Value.Last().Legs.Last();
            }
        }

        /// <summary>
        /// Removes the current leg from the game.
        /// </summary>
        public void RemoveCurrentLeg()
        {
            if (this.StandardMatch.StandardMatchType == StandardMatchType.Legs)
            {
                var indexLast = this.StandardMatch.LegsPlayed.Value.Count - 1;
                this.StandardMatch.LegsPlayed.Value.RemoveAt(indexLast);

            }
            else
            {
                var indexLast = this.StandardMatch.SetsPlayed.Value.Last().Legs.Count - 1;
                this.StandardMatch.SetsPlayed.Value.Last().Legs.RemoveAt(indexLast);

                // If there are no legs left in de last set, remove this set that is empty.
                if (this.StandardMatch.SetsPlayed.Value.Last().Legs.Count == 0)
                {
                    var indexLastSet = this.StandardMatch.SetsPlayed.Value.Count() - 1;
                    this.StandardMatch.SetsPlayed.Value.RemoveAt(indexLastSet);
                }
            }
        }

        /// <summary>
        /// Adds a thrown score to a leg by a player.
        /// </summary>
        /// <param name="score">The score that was thrown.</param>
        /// <param name="player">The player that threw this score.</param>
        public void AddScore(int score, Player player)
        {
            var currentLegByPlayer = this.GetCurrentLegByPlayer(player);

            currentLegByPlayer.Scores.Add(score);
            currentLegByPlayer.DartsThrown.Add(3);
        }

        /// <summary>
        /// Gets the current leg of a specified player.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public LegByPlayer GetCurrentLegByPlayer(Player player)
        {
            return this.GetCurrentLeg().LegByPlayers
                .First(x => x.PlayerId == player.Id.OrElse(player.Name));
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
        /// Will undo the last score entered in this the system.
        /// </summary>
        public void UndoTurn()
        {
            // TODO: When the game is undo-ed to the start just return.

            // Get the player who was previous at turn
            var indexPreviousPlayer = Math.Abs(this.StandardMatch.Players.IndexOf(this.GetTurn()) - 1) % this.StandardMatch.Players.Count;
            var previousPlayer = this.StandardMatch.Players[indexPreviousPlayer];

            var legByPlayerToUndo = this.GetCurrentLegByPlayer(previousPlayer);
            if (legByPlayerToUndo.Scores.Count == 0)
            {
                // When no scores are played in the remove this leg and undo the last score.
                this.RemoveCurrentLeg();
                this.UndoTurn();
            }
            else
            {
                // Remove the last score from the list of scores added and remove the last item from darts thrown
                legByPlayerToUndo.Scores.RemoveAt(legByPlayerToUndo.Scores.Count - 1);
                legByPlayerToUndo.DartsThrown.RemoveAt(legByPlayerToUndo.DartsThrown.Count - 1);
            }
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