namespace Arras.Business
{
    using System;
    using Common.Player;

    public class NormalPlayerService : PlayerService
    {
        public NormalPlayerService(Player player) : base(player)
        {
        }

        public override int GenerateScore(int remainingScore)
        {
            throw new Exception("Cannot generate a random score for a normal player");
        }
    }
}