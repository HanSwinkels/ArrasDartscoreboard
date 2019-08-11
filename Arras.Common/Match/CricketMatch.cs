namespace Arras.Common.Match
{
    using System.Collections.Generic;
    using Player;

    public class CricketMatch : Match
    {
        public CricketMatch(MatchType type, List<Player> players) : base(type, players)
        {
        }
    }
}