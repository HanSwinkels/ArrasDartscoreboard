using System;

namespace Arras.Business
{
    using Common.Player;

    public class BotPlayerService : PlayerService
    {
        private const int MaxScore1 = 80;
        private const int MaxScoreDev1 = 10;
        private const int MaxScore2 = 80;
        private const int MaxScoreDev2 = 15;
        private const int MaxScore3 = 70;
        private const int MaxScoreDev3 = 25;
        private const int MaxScore4 = 90;
        private const int MaxScoreDev4 = 25;
        private const int MaxScore5 = 90;
        private const int MaxScoreDev5 = 30;
        private const int MaxScore6 = 80;
        private const int MaxScoreDev6 = 40;
        private const int MaxScore7 = 100;
        private const int MaxScoreDev7 = 40;
        private const int MaxScore8 = 120;
        private const int MaxScoreDev8 = 40;
        private const int MaxScore9 = 140;
        private const int MaxScoreDev9 = 40;


        /// <summary>
        /// Generate a random score, based on the level of the bot and the remaining score.
        /// </summary>
        /// <param name="remainingScore">The remaining score of the player.</param>
        /// <returns>A random score.</returns>
        public override int GenerateScore(int remainingScore)
        {
            if (this.Player.PlayerType != PlayerType.Bot)
                throw new Exception("Cannot generate a score for a player that is not a bot");

            var bot = this.Player as BotPlayer;

            switch (bot.Level)
            {
                case BotLevel.One:
                    return GenerateScoreBot(remainingScore, MaxScore1, MaxScoreDev1);
                case BotLevel.Two:
                    return GenerateScoreBot(remainingScore, MaxScore2, MaxScoreDev2);
                case BotLevel.Three:
                    return GenerateScoreBot(remainingScore, MaxScore3, MaxScoreDev3);
                case BotLevel.Four:
                    return GenerateScoreBot(remainingScore, MaxScore4, MaxScoreDev4);
                case BotLevel.Five:
                    return GenerateScoreBot(remainingScore, MaxScore5, MaxScoreDev5);
                case BotLevel.Six:
                    return GenerateScoreBot(remainingScore, MaxScore6, MaxScoreDev6);
                case BotLevel.Seven:
                    return GenerateScoreBot(remainingScore, MaxScore7, MaxScoreDev7);
                case BotLevel.Eight:
                    return GenerateScoreBot(remainingScore, MaxScore8, MaxScoreDev8);
                case BotLevel.Nine:
                    return GenerateScoreBot(remainingScore, MaxScore9, MaxScoreDev9);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Generates a random score for Bot Level 1, based on the remaining score.
        /// </summary>
        /// <param name="remainingScore">The remaining score for this player.</param>
        /// <param name="maxScore"></param>
        /// <param name="maxScoreDev"></param>
        /// <returns></returns>
        private static int GenerateScoreBot(int remainingScore, int maxScore, int maxScoreDev)
        {
            var random = new Random();

            if (remainingScore > 170)
            {
                return random.Next(maxScore) + maxScoreDev;
            }
            else if (remainingScore > 100)
            {
                return random.Next(maxScore - 5) + maxScoreDev;
            }
            else if (remainingScore > 50)
            {
                return random.Next(remainingScore - 10) + 5;
            }
            else
            {
                var val = random.Next(remainingScore);
                return val > remainingScore - 5 ? remainingScore : val;
            }
        }

        /// <summary>
        /// Initializes the player service.
        /// </summary>
        /// <param name="player">The player for which this player service exists.</param>
        public BotPlayerService(Player player) : base(player)
        {
        }
    }
}
