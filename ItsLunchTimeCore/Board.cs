using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItsLunchTimeCore
{
    public class PublicBoard
    {
        public ReadOnlyCollection<RestaurantPlace> Restaurants { get; }
        public Home Home { get; }
        public int CurrentDay { get; private set; }
        public ReadOnlyDictionary<Player, int> PlayerScores { get; }
        public int TeamScore { get; private set; }
        public ReadOnlyDictionary<Restaurant, RestaurantTrack> RestaurantTracks { get; private set; }
        public TeamBonusCard CurrentTeamBonus { get; internal set; }
        public ReadOnlyCollection<PlayerBonusCard> CurrentPlayerBonuses { get; private set; }
        public List<PlayerDescriptor> PlayerDescriptors { get; private set; }

        public PublicBoard()
        {
            
        }

        internal void SetNewPlayerBonuses(IList<PlayerBonusCard> bonuses)
        {
            this.CurrentPlayerBonuses = new ReadOnlyCollection<PlayerBonusCard>(bonuses);
        }
    }
}
