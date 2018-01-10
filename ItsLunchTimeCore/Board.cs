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
        public int CurrentDay { get; private set; }
        public int TeamScore { get; private set; }
        public TeamBonusCard CurrentTeamBonus { get; internal set; }
        public Home Home { get; }

        private Dictionary<Restaurant, RestaurantPlace> _restaurants;
        public ReadOnlyDictionary<Restaurant, RestaurantPlace> Restaurants { get; }
        public ReadOnlyDictionary<PlayerDescriptor, int> PlayerScores { get; }
        public ReadOnlyDictionary<Restaurant, RestaurantTrack> RestaurantTracks { get; private set; }
        public ReadOnlyCollection<PlayerBonusCard> CurrentPlayerBonuses { get; private set; }
        public ReadOnlyDictionary<Character, PlayerDescriptor> PlayerDescriptors { get; private set; }

        public int MinimumMajoritySize
        {
            get
            {
                switch (PlayerDescriptors.Count)
                {
                    case 3: return 2;
                    case 4: return 3;
                    case 5: return 3;
                    case 6: return 4;                        
                }
                return 0;
            }
        }

        public PublicBoard()
        {
            this.Home = new Home();
            this._restaurants = new Dictionary<Restaurant, RestaurantPlace>();
            this._restaurants.Add(Restaurant.Russo, new RestaurantPlace(2));
            this._restaurants.Add(Restaurant.Palatus, new RestaurantPlace(3));
            this._restaurants.Add(Restaurant.GustoDiBacio, new RestaurantPlace(4));
            this._restaurants.Add(Restaurant.Silva, new RestaurantPlace(4));
            this._restaurants.Add(Restaurant.Panorama, new RestaurantPlace(5));
            this._restaurants.Add(Restaurant.JoeAndLeos, new RestaurantPlace(6));
            this.Restaurants = new ReadOnlyDictionary<Restaurant, RestaurantPlace>(_restaurants);
        }

        internal bool HasMajority(DayOfWeek day)
        {
            return RestaurantWithMajority(day) != null;
        }

        internal bool IsPlayerInMajority(DayOfWeek day, PlayerDescriptor player)
        {
            Restaurant? majorityRestaurant = RestaurantWithMajority(day);
            return majorityRestaurant != null ? Restaurants[majorityRestaurant.Value].Visitors[day].Contains(player) : false;
        }

        internal Restaurant? RestaurantWithMajority(DayOfWeek day)
        {
            foreach (Restaurant restaurant in Extensions.Restaurants)
            {
                if (Restaurants[restaurant].Visitors[day].Count >= MinimumMajoritySize)
                {
                    return restaurant;
                }
            }
            return null;
        }

        internal void SetNewPlayerBonuses(IList<PlayerBonusCard> bonuses)
        {
            this.CurrentPlayerBonuses = new ReadOnlyCollection<PlayerBonusCard>(bonuses);
        }
    }
}
