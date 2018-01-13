﻿using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

        private Dictionary<PlayerBase, int> _playerScores;
        public ReadOnlyDictionary<PlayerBase, int> PlayerScores { get; }

        private Dictionary<PlayerBase, int> _playerCash;
        public ReadOnlyDictionary<PlayerBase, int> PlayerCash { get; }

        private Dictionary<PlayerBase, Restaurant> _undesiredRestaurants;
        public ReadOnlyDictionary<PlayerBase, Restaurant> UndesiredRestaurants { get; }

        private Dictionary<PlayerBase, List<FoodType>> _favoriteFood;
        public ReadOnlyDictionary<PlayerBase, List<FoodType>> FavoriteFood { get; }

        private Dictionary<PlayerBase, ReadOnlyDictionary<DayOfWeek, Place>> _visitedPlaces;
        private Dictionary<PlayerBase, Dictionary<DayOfWeek, Place>> _internalVisitedPlaces;
        public ReadOnlyDictionary<PlayerBase, ReadOnlyDictionary<DayOfWeek, Place>> VisitedPlaces { get; }

        public ReadOnlyDictionary<Restaurant, RestaurantTrack> RestaurantTracks { get; private set; }
        public ReadOnlyCollection<PlayerBonusCard> CurrentPlayerBonuses { get; private set; }

        public List<PlayerBase> Players { get; }

        public int MinimumMajoritySize
        {
            get
            {
                switch (Players.Count)
                {
                    case 3: return 2;
                    case 4: return 3;
                    case 5: return 3;
                    case 6: return 4;
                }
                return 0;
            }
        }

        public PublicBoard(List<PlayerBase> players)
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

            this._playerScores = new Dictionary<PlayerBase, int>();
            players.ForEach(player => this._playerScores.Add(player, 0));
            this.PlayerScores = new ReadOnlyDictionary<PlayerBase, int>(_playerScores);

            this._playerCash = new Dictionary<PlayerBase, int>();
            players.ForEach(player => this._playerCash.Add(player, 0));

            this._undesiredRestaurants = new Dictionary<PlayerBase, Restaurant>();
            this.UndesiredRestaurants = new ReadOnlyDictionary<PlayerBase, Restaurant>(_undesiredRestaurants);

            this._favoriteFood = new Dictionary<PlayerBase, List<FoodType>>();
            this.FavoriteFood = new ReadOnlyDictionary<PlayerBase, List<FoodType>>(_favoriteFood);
        }


        internal void AddVictoryPointsToPlayer(int points, PlayerBase player)
        {
            this._playerScores[player] += points;
        }

        internal void AddCashToPlayer(int cash, PlayerBase player)
        {
            this._playerCash[player] += cash;
        }

        internal bool HasMajority(DayOfWeek day)
        {
            return RestaurantWithMajority(day) != null;
        }

        internal bool HasUnanimity(DayOfWeek day)
        {
            foreach (Restaurant restaurant in Extensions.Restaurants)
            {
                if (Restaurants[restaurant].Visitors[day].Count == Players.Count)
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

        internal bool RestaurantHasModifierForThisDay<T>(Restaurant restaurant, DayOfWeek day)
            where T : RestaurantDailyModifierCard
        {
            return Restaurants[restaurant].Modifier is T && Restaurants[restaurant].Modifier.Days.Contains(day);
        }

        internal void ClearVisitedPlaces()
        {
            this._internalVisitedPlaces = new Dictionary<PlayerBase, Dictionary<DayOfWeek, Place>>();
            this._visitedPlaces = new Dictionary<PlayerBase, ReadOnlyDictionary<DayOfWeek, Place>>();
            foreach (PlayerBase player in Players)
            {
                Dictionary<DayOfWeek, Place> _playerVisitedPlaces = new Dictionary<DayOfWeek, Place>();
                this._internalVisitedPlaces.Add(player, _playerVisitedPlaces);
                this._visitedPlaces.Add(player, new ReadOnlyDictionary<DayOfWeek, Place>(_playerVisitedPlaces));
            }
        }

        internal void VisitPlace(PlayerBase player, DayOfWeek day, Place place)
        {
            this._internalVisitedPlaces[player].Add(day, place);
            place.VisitPlace(player, day);
        }

        internal void ClearFavoriteFood()
        {
            this._favoriteFood.Clear();
            Players.ForEach(player => this._favoriteFood.Add(player, new List<FoodType>()));
        }

        internal void SetFavoriteFoodForPlayer(PlayerBase player, List<FoodType> food)
        {
            this._favoriteFood[player].AddRange(food);
        }

        internal void ClearUndesiredRestaurants()
        {
            this._undesiredRestaurants.Clear();
        }

        internal void SetUndesiredRestaurantOfTheWeek(PlayerBase player, Restaurant undesired)
        {
            this._undesiredRestaurants.Add(player, undesired);
        }

        internal bool IsPlayerInMajority(DayOfWeek day, PlayerBase player)
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
