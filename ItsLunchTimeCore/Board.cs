using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ItsLunchTimeCore
{
    public class PublicBoard
    {
        public int CurrentDay { get; private set; }
        public int TeamScore { get; internal set; }
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
            this._restaurants = new Dictionary<Restaurant, RestaurantPlace>
            {
                { Restaurant.Russo, new RestaurantPlace(2) },
                { Restaurant.Palatus, new RestaurantPlace(3) },
                { Restaurant.GustoDiBacio, new RestaurantPlace(4) },
                { Restaurant.Silva, new RestaurantPlace(4) },
                { Restaurant.Panorama, new RestaurantPlace(5) },
                { Restaurant.JoeAndLeos, new RestaurantPlace(6) }
            };
            this.Restaurants = new ReadOnlyDictionary<Restaurant, RestaurantPlace>(_restaurants);
        }

        internal bool HasMajority(DayOfWeek day)
        {
            return RestaurantWithMajority(day) != null;
        }

        internal bool HasUnanimity(DayOfWeek day)
        {
            foreach (Restaurant restaurant in Extensions.Restaurants)
            {
                if (Restaurants[restaurant].Visitors[day].Count == PlayerDescriptors.Count)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool HasSomeoneAlone(DayOfWeek day)
        {
            foreach (Restaurant restaurant in Extensions.Restaurants)
            {
                if (Restaurants[restaurant].Visitors[day].Count == 1)
                {
                    return true;
                }
            }
            return Home.Visitors.Count > 0;
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
