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
                new Player("PlayerOne", PlayerType.Normal),
                new Player("PlayerTwo", PlayerType.Normal)
            };
            this.LegsMatch = new StandardMatch(StandardMatchType.Legs, Maybe<int>.Nothing, 6, 501, this.Players);
            this.SetsMatch = new StandardMatch(StandardMatchType.Sets, 3.ToMaybe(), 6, 501, this.Players);

            var leg1 = new Leg();
            leg1.LegByPlayers.Add(new LegByPlayer("PlayerTwo")
            {
                DartsThrown = 20,
                HasStarted = true,
                IsWon = false.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 100, 140, 31 }
            });
            leg1.LegByPlayers.Add(new LegByPlayer("PlayerTwo")
            {
                DartsThrown = 20,
                HasStarted = true,
                IsWon = true.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 100, 140, 31 }
            });
            var leg2 = new Leg();
            leg2.LegByPlayers.Add(new LegByPlayer("PlayerOne")
            {
                DartsThrown = 21,
                HasStarted = false,
                IsWon = false.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 100, 83, 42 }
            });
            leg2.LegByPlayers.Add(new LegByPlayer("PlayerTwo")
            {
                DartsThrown = 20,
                HasStarted = false,
                IsWon = true.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 100, 140, 31 }
            });
            var leg3 = new Leg();
            leg3.LegByPlayers.Add(new LegByPlayer("PlayerOne")
            {
                DartsThrown = 36,
                HasStarted = true,
                IsWon = false.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 8, 10, 31, 46, 30, 20 }
            });
            leg3.LegByPlayers.Add(new LegByPlayer("PlayerTwo")
            {
                DartsThrown = 20,
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
        /// Tests whether the method <see cref="MatchService.GetLastLeg"/> returns the correct value.
        /// </summary>
        [TestMethod]
        public void GetLastLeg()
        {
            var match = new MatchService(this.LegsMatch);
            Assert.AreEqual(match.GetLastLeg(), this.Legs.Last());
        }

        /// <summary>
        /// Tests whether the method <see cref="MatchService.EnterScore"/> returns the correct value.
        /// </summary>
        [TestMethod]
        public void EnterScore()
        {
            this.LegsMatch.Players.First().Score = 91;
            this.LegsMatch.Players.Last().Score = 91;
            var match = new MatchService(this.LegsMatch);
            Assert.AreEqual(ScoreValidationType.Valid, match.EnterScore(40));
           
            match = new MatchService(this.LegsMatch);
            Assert.AreEqual(ScoreValidationType.Invalid, match.EnterScore(100));

            match = new MatchService(this.LegsMatch);
            Assert.AreEqual(ScoreValidationType.Invalid, match.EnterScore(90));

            match = new MatchService(this.LegsMatch);
            Assert.AreEqual(ScoreValidationType.Valid, match.EnterScore(40));

            match = new MatchService(this.LegsMatch);
            Assert.AreEqual(ScoreValidationType.EndsLeg, match.EnterScore(51));
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
                DartsThrown = 20,
                HasStarted = true,
                IsWon = true.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 180}
            });
            leg.LegByPlayers.Add(new LegByPlayer("PlayerTwo")
            {
                DartsThrown = 20,
                HasStarted = true,
                IsWon = true.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 180}
            });
            this.LegsMatch.LegsPlayed.Value.Add(leg);
            var match = new MatchService(this.LegsMatch);
            Assert.AreEqual(19, match.EndLeg(2, this.Players.First()).LegByPlayers.First(x => x.PlayerId == this.Players.First().Name).DartsThrown);
        }

        /// <summary>
        /// Tests whether the method <see cref="MatchService.AddScore"/> returns the correct value.
        /// </summary>
        [TestMethod]
        public void AddScore()
        {
            var match = new MatchService(this.LegsMatch);
            match.AddScore(377, this.Players.First());
            Assert.AreEqual(377, match.GetLastLeg().LegByPlayers.First(x => x.PlayerId == this.Players.First().Name).Scores.Last());
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
                DartsThrown = 20,
                HasStarted = true,
                IsWon = true.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60}
            });
            leg.LegByPlayers.Add(new LegByPlayer("PlayerTwo")
            {
                DartsThrown = 20,
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