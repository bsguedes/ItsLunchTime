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
        public ReadOnlyCollection<Restaurant> Restaurants { get; private set; }
        public Home Home { get; private set; }
        public int CurrentDay { get; private set; }
        public ReadOnlyDictionary<Player, int> PlayerScores { get; private set; }
        public int TeamScore { get; private set; }
        public ReadOnlyDictionary<Restaurant, RestaurantTrack> RestaurantTracks { get; private set; }
        public ITeamBonus CurrentTeamBonus { get; private set; }
        public ReadOnlyCollection<IPlayerBonus> CurrentPlayerBonuses { get; private set; }
    }
}
