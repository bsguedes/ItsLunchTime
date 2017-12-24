using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ItsLunchTimeCore
{
    public class RestaurantTrack
    {
        private Dictionary<PlayerDescriptor, int> _scores;
        public ReadOnlyDictionary<PlayerDescriptor, int> PlayerScores { get; private set; }
        public int[] RewardLevels { get; }
        public int CardAmount { get; }

        internal RestaurantTrack(List<PlayerDescriptor> players, int[] reward_levels, int card_amount)
        {
            this._scores = new Dictionary<PlayerDescriptor, int>();
            this.PlayerScores = new ReadOnlyDictionary<PlayerDescriptor, int>(_scores);
            this.RewardLevels = reward_levels;
            this.CardAmount = card_amount;

            foreach(PlayerDescriptor player in players)
            {
                this._scores.Add(player, 0);
            }
        }

        internal bool AdvancePlayer(PlayerDescriptor player)
        {
            this._scores[player]++;
            return RewardLevels.Contains(this._scores[player]);
        }
    }
}
