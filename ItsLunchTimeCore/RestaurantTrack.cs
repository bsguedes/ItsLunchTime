using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ItsLunchTimeCore
{
    public class RestaurantTrack
    {
        private Dictionary<PlayerBase, int> _scores;
        public ReadOnlyDictionary<PlayerBase, int> PlayerScores { get; private set; }
        public int[] RewardLevels { get; }
        public int CardAmount { get; }

        internal RestaurantTrack(List<PlayerBase> players, int[] reward_levels, int card_amount)
        {
            this._scores = new Dictionary<PlayerBase, int>();
            this.PlayerScores = new ReadOnlyDictionary<PlayerBase, int>(_scores);
            this.RewardLevels = reward_levels;
            this.CardAmount = card_amount;

            foreach (PlayerBase player in players)
            {
                this._scores.Add(player, 0);
            }
        }

        internal bool AdvancePlayer(PlayerBase player)
        {
            this._scores[player]++;
            return RewardLevels.Contains(this._scores[player]);
        }
    }
}
