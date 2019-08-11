namespace Arras.Business.Test
{
    using System.Collections.Generic;
    using Common;
    using Common.Match;
    using Common.Player;
    using Functional.Maybe;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Class that tests the methods from <see cref="PlayerService"/>
    /// </summary>
    [TestClass]
    public class PlayerServiceTest
    {
        /// <summary>
        /// A player used for test cases.
        /// </summary>
        private Player Player;

        /// <summary>
        /// A list of legs used for test cases.
        /// </summary>
        private List<LegByPlayer> Legs;

        /// <summary>
        /// The service that provides all methods regarding players.
        /// </summary>
        private PlayerService Service;

        /// <summary>
        /// A standard match used for the test cases.
        /// </summary>
        private StandardMatch Match;

        /// <summary>
        /// Creates a mock player with stats, used for test purposes
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.Player = new Player("PlayerOne", PlayerType.Normal);
            this.Service = new PlayerService();
            var players = new List<Player>()
            {
                new Player("PlayerOne", PlayerType.Normal),
                new Player("PlayerTwo", PlayerType.Normal)
            };

            this.Match = new StandardMatch(StandardMatchType.Legs, Maybe<int>.Nothing, 6, 501, players);
            var leg1 = new Leg();
            leg1.LegByPlayers.Add(new LegByPlayer(this.Player.Id.OrElse(this.Player.Name))
            {
                DartsThrown = 20,
                HasStarted = true,
                IsWon = true.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 100, 140, 31}
            });
            leg1.LegByPlayers.Add(new LegByPlayer("PlayerTwo")
            {
                DartsThrown = 20,
                HasStarted = true,
                IsWon = true.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 100, 140, 31 }
            });
            var leg2 = new Leg();
            leg2.LegByPlayers.Add(new LegByPlayer(this.Player.Id.OrElse(this.Player.Name))
            {
                DartsThrown = 21,
                HasStarted = false,
                IsWon = false.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 100, 83, 42 }
            });
            leg2.LegByPlayers.Add(new LegByPlayer(this.Player.Id.OrElse(this.Player.Name)) 
            {
                DartsThrown = 20,
                HasStarted = false,
                IsWon = true.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 100, 140, 31 }
            });
            var leg3 = new Leg();
            leg3.LegByPlayers.Add(new LegByPlayer(this.Player.Id.OrElse(this.Player.Name))
            {
                DartsThrown = 36,
                HasStarted = true,
                IsWon = false.ToMaybe(),
                Scores = new List<int> { 46, 83, 41, 60, 8, 10, 31, 46, 30, 20}
            });

            this.Match.LegsPlayed = new List<Leg> { leg1, leg2, leg3 }.ToMaybe();
        }

        /// <summary>
        /// Test whether the method <see cref="PlayerService.GetBasicStats"/> returns the right properties with the
        /// correct values.
        /// </summary>
        [TestMethod]
        public void GetBasicStats()
        {
            var stats = this.Service.GetBasicStats(this.Match, this.Player);

            Assert.AreEqual(56.66, stats.Average);
            Assert.AreEqual(56.67, stats.AverageFirstNine);
            Assert.AreEqual(140, stats.HighestScore);
            Assert.AreEqual(5, stats.Plus100);
            Assert.AreEqual(2, stats.Plus140);
            Assert.AreEqual(10, stats.Plus80);
            Assert.AreEqual(0, stats.OneEighties);
        }

        /// <summary>
        /// Test whether the method <see cref="PlayerService.GetAllStats"/> returns the right properties with the
        /// correct values.
        /// </summary>
        [TestMethod]
        public void GetAllStats()
        {
            var stats = this.Service.GetAllStats(this.Match, this.Player);

            Assert.AreEqual(56.66, stats.Average);
            Assert.AreEqual(56.67, stats.AverageFirstNine);
            Assert.AreEqual(140, stats.HighestScore);
            Assert.AreEqual(5, stats.Plus100);
            Assert.AreEqual(2, stats.Plus140);
            Assert.AreEqual(10, stats.Plus80);
            Assert.AreEqual(0, stats.OneEighties);
            Assert.AreEqual(75.15, stats.AverageBestLeg);
            Assert.AreEqual(20, stats.BestLeg);
            Assert.AreEqual(20, stats.WorstLeg);
            Assert.AreEqual(31, stats.HighestCheckout);
            Assert.AreEqual(1, stats.Breaks);
        }
    }
}
