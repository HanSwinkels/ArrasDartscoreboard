namespace Arras.Business.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Match;
    using Common.Player;
    using Functional.Maybe;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test class for <see cref="MatchService"/>
    /// </summary>
    [TestClass]
    public class MatchServiceTest
    {
        /// <summary>
        /// A standard match with <see cref="StandardMatchType.Legs"/>
        /// </summary>
        private StandardMatch LegsMatch;

        /// <summary>
        /// A standard match with <see cref="StandardMatchType.Sets"/>
        /// </summary>
        private StandardMatch SetsMatch;

        /// <summary>
        /// A mockup of example legs.
        /// </summary>
        private List<Leg> Legs;

        /// <summary>
        /// A mockup of example legs.
        /// </summary>
        private List<Player> Players;

        [TestInitialize]
        public void InitializeTest()
        {
            this.Players = new List<Player>()
            {
                new NormalPlayer("PlayerOne"),
                new NormalPlayer("PlayerTwo")
            };
            this.LegsMatch = new StandardMatch(StandardMatchType.Legs, Maybe<int>.Nothing, 6, 501, this.Players);
            this.SetsMatch = new StandardMatch(StandardMatchType.Sets, 3.ToMaybe(), 6, 501, this.Players);

            var leg1 = new Leg();
            leg1.LegByPlayers.Add(new LegByPlayer("PlayerTwo")
            {
                DartsThrown = {5,5,5,5},
                HasStarted = true,
                IsWon = false.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 100, 140, 31 }
            });
            leg1.LegByPlayers.Add(new LegByPlayer("PlayerTwo")
            {
                DartsThrown = {5,5,5,5},
                HasStarted = true,
                IsWon = true.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 100, 140, 31 }
            });
            var leg2 = new Leg();
            leg2.LegByPlayers.Add(new LegByPlayer("PlayerOne")
            {
                DartsThrown = {5,5,5,5,1},
                HasStarted = false,
                IsWon = false.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 100, 83, 42 }
            });
            leg2.LegByPlayers.Add(new LegByPlayer("PlayerTwo")
            {
                DartsThrown = {5,5,5,5},
                HasStarted = false,
                IsWon = true.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 100, 140, 31 }
            });
            var leg3 = new Leg();
            leg3.LegByPlayers.Add(new LegByPlayer("PlayerOne")
            {
                DartsThrown = {6,6,6,6,6,6},
                HasStarted = true,
                IsWon = false.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 8, 10, 31, 46, 30, 20 }
            });
            leg3.LegByPlayers.Add(new LegByPlayer("PlayerTwo")
            {
                DartsThrown = {5,5,5,5},
                HasStarted = false,
                IsWon = true.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 100, 140, 31 }
            });
            this.Legs = new List<Leg> { leg1, leg1, leg3 };
            this.LegsMatch.LegsPlayed = this.Legs.ToMaybe();


        }

        #region LegsMatch

        /// <summary>
        /// Tests whether the method <see cref="MatchService.GetTotalLegs"/> returns the correct value.
        /// </summary>
        [TestMethod]
        public void GetTotalLegs()
        {
            var match = new MatchService(this.LegsMatch);
            Assert.AreEqual(match.GetTotalLegs(), 3);
        }

        /// <summary>
        /// Tests whether the method <see cref="MatchService.GetCurrentLeg"/> returns the correct value.
        /// </summary>
        [TestMethod]
        public void GetLastLeg()
        {
            var match = new MatchService(this.LegsMatch);
            Assert.AreEqual(match.GetCurrentLeg(), this.Legs.Last());
        }

        /// <summary>
        /// Tests whether the method <see cref="MatchService.EnterScore"/> returns the correct value.
        /// </summary>
        [TestMethod]
        public void EnterScore()
        {
            var match = new MatchService(this.LegsMatch);
            match.StandardMatch.Format = 91;
            Assert.AreEqual(ScoreValidationType.Invalid, match.EnterScore(100, Players.First()));

            match = new MatchService(this.LegsMatch);
            match.StandardMatch.Format = 91;
            Assert.AreEqual(ScoreValidationType.Invalid, match.EnterScore(90, Players.First()));

            match = new MatchService(this.LegsMatch);
            match.StandardMatch.Format = 91;
            Assert.AreEqual(ScoreValidationType.Valid, match.EnterScore(40, Players.First()));

            match = new MatchService(this.LegsMatch);
            match.StandardMatch.Format = 51;
            Assert.AreEqual(ScoreValidationType.EndsLeg, match.EnterScore(51, Players.First()));
        }

        /// <summary>
        /// Tests whether the method <see cref="MatchService.IsGameFinished"/> returns the correct value.
        /// </summary>
        [TestMethod]
        public void IsGameFinished()
        {
            this.LegsMatch.Legs = 3;
            var match = new MatchService(this.LegsMatch);
            Assert.AreEqual(true, match.IsGameFinished(this.Players.Last()));

        }

        /// <summary>
        /// Tests whether the method <see cref="MatchService.EndLeg"/> returns the correct value.
        /// </summary>
        [TestMethod]
        public void EndLeg()
        {
            var leg = new Leg();
            leg.LegByPlayers.Add(new LegByPlayer("PlayerOne")
            {
                DartsThrown = {5,5,5,5},
                HasStarted = true,
                IsWon = true.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 180}
            });
            leg.LegByPlayers.Add(new LegByPlayer("PlayerTwo")
            {
                DartsThrown = {5,5,5,5},
                HasStarted = true,
                IsWon = true.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 180}
            });
            this.LegsMatch.LegsPlayed.Value.Add(leg);
            var match = new MatchService(this.LegsMatch);
            Assert.AreEqual(19, match.EndLeg(2).LegByPlayers.First(x => x.PlayerId == this.Players.First().Name).DartsThrown);
        }

        /// <summary>
        /// Tests whether the method <see cref="MatchService.AddScore"/> returns the correct value.
        /// </summary>
        [TestMethod]
        public void AddScore()
        {
            var match = new MatchService(this.LegsMatch);
            match.AddScore(377, this.Players.First());
            Assert.AreEqual(377, match.GetCurrentLeg().LegByPlayers.First(x => x.PlayerId == this.Players.First().Name).Scores.Last());
        }

        /// <summary>
        /// Tests whether the method <see cref="MatchService.GetStartingPlayer"/> returns the correct value.
        /// </summary>
        [TestMethod]
        public void GetStartingPlayer()
        {
            var match = new MatchService(this.LegsMatch);
            Assert.AreEqual(this.Players.First(), match.GetStartingPlayer());
        }

        /// <summary>
        /// Tests whether the method <see cref="MatchService.GetTurn"/> switches to the correct player.
        /// </summary>
        [TestMethod]
        public void GetTurn()
        {
            var leg = new Leg();
            leg.LegByPlayers.Add(new LegByPlayer("PlayerOne")
            {
                DartsThrown = {5,5,5,5},
                HasStarted = true,
                IsWon = true.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60}
            });
            leg.LegByPlayers.Add(new LegByPlayer("PlayerTwo")
            {
                DartsThrown = {5,5,5,5},
                HasStarted = true,
                IsWon = true.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60}
            });
            this.LegsMatch.LegsPlayed.Value.Add(leg);
            var match = new MatchService(this.LegsMatch);

            Assert.AreEqual(this.Players.Skip(1).First(), match.GetTurn());

            this.LegsMatch.LegsPlayed.Value.Last().LegByPlayers.Last().Scores.Add(55);
            match = new MatchService(this.LegsMatch);
            Assert.AreEqual(this.Players.First(), match.GetTurn());
        }

        #endregion LegsMatch

        #region SetsMatch

        // TODO: Add test cases for a match which has a set format.

        #endregion SetsMatch
    }
}