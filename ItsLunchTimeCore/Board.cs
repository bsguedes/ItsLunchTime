﻿using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ItsLunchTimeCore
{
    public class PublicBoard
    {
        public int CurrentDay { get; internal set; }
        public DayOfWeek CurrentWeekDay { get { return Extensions.DaysOfWeek[(CurrentDay - 1) % 7]; } }
        public int TeamScore { get; private set; }
        public TeamBonusCard CurrentTeamBonus { get; internal set; }
        public int TeamBonusDoneCount { get; internal set; }
        public Home Home { get; }

        private Dictionary<Restaurant, RestaurantPlace> _restaurants;
        public ReadOnlyDictionary<Restaurant, RestaurantPlace> Restaurants { get; }

        private Dictionary<PlayerBase, Dictionary<VictoryPointsSource, int>> _separatedScores;
        public Dictionary<PlayerBase, Dictionary<VictoryPointsSource, int>> SeparatedScores => _separatedScores;

        private Dictionary<PlayerBase, int> _playerScores;
        public ReadOnlyDictionary<PlayerBase, int> PlayerScores { get; }

        private Dictionary<PlayerBase, int> _playerCash;

        public ReadOnlyDictionary<PlayerBase, int> PlayerCash { get; }

        private Dictionary<PlayerBase, Restaurant> _undesiredRestaurants;
        public ReadOnlyDictionary<PlayerBase, Restaurant> UndesiredRestaurants { get; }

        private Dictionary<PlayerBase, List<FoodType>> _favoriteFood;
        public ReadOnlyDictionary<PlayerBase, List<FoodType>> FavoriteFood { get; }

        private List<PlayerBonusCard> _currentPlayerBonuses;
        public ReadOnlyCollection<PlayerBonusCard> CurrentPlayerBonuses { get; }

        private Dictionary<PlayerBase, ReadOnlyDictionary<DayOfWeek, Place>> _visitedPlaces;
        private Dictionary<PlayerBase, Dictionary<DayOfWeek, Place>> _internalVisitedPlaces;
        public ReadOnlyDictionary<PlayerBase, ReadOnlyDictionary<DayOfWeek, Place>> VisitedPlaces { get; }

        private Dictionary<Restaurant, RestaurantTrack> _restaurantTracks;
        public ReadOnlyDictionary<Restaurant, RestaurantTrack> RestaurantTracks { get; private set; }

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

        public PublicBoard(List<PlayerBase> players, DifficultyLevel difficulty)
        {
            TeamBonusDoneCount = 0;
            switch (difficulty)
            {
                case DifficultyLevel.Easy:
                    this.TeamScore = 10;
                    break;
                case DifficultyLevel.Medium:
                    this.TeamScore = 5;
                    break;
                case DifficultyLevel.Hard:
                    this.TeamScore = 0;
                    break;
            }
            this.Players = players;
            this.Home = new Home();
            this._restaurants = new Dictionary<Restaurant, RestaurantPlace>
            {
                { Restaurant.Russo, new RestaurantPlace(Restaurant.Russo, 2, FoodType.Brazilian) },
                { Restaurant.Palatus, new RestaurantPlace(Restaurant.Palatus, 3, FoodType.Pasta) },
                { Restaurant.GustoDiBacio, new RestaurantPlace(Restaurant.GustoDiBacio, 4, FoodType.Pizza) },
                { Restaurant.Silva, new RestaurantPlace(Restaurant.Silva, 4, FoodType.Burger) },
                { Restaurant.Panorama, new RestaurantPlace(Restaurant.Panorama, 5, FoodType.Chinese) },
                { Restaurant.JoeAndLeos, new RestaurantPlace(Restaurant.JoeAndLeos, 6, FoodType.Vegetarian) }
            };
            this.Restaurants = new ReadOnlyDictionary<Restaurant, RestaurantPlace>(_restaurants);

            this._playerScores = new Dictionary<PlayerBase, int>();
            players.ForEach(player => this._playerScores.Add(player, Game.STARTING_SCORE));
            this.PlayerScores = new ReadOnlyDictionary<PlayerBase, int>(_playerScores);

            this._playerCash = new Dictionary<PlayerBase, int>();
            players.ForEach(player => this._playerCash.Add(player, Extensions.StartingMoney[player.Character]));
            this.PlayerCash = new ReadOnlyDictionary<PlayerBase, int>(this._playerCash);

            this._undesiredRestaurants = new Dictionary<PlayerBase, Restaurant>();
            this.UndesiredRestaurants = new ReadOnlyDictionary<PlayerBase, Restaurant>(_undesiredRestaurants);

            this._favoriteFood = new Dictionary<PlayerBase, List<FoodType>>();
            this.FavoriteFood = new ReadOnlyDictionary<PlayerBase, List<FoodType>>(_favoriteFood);

            this._internalVisitedPlaces = new Dictionary<PlayerBase, Dictionary<DayOfWeek, Place>>();
            this._visitedPlaces = new Dictionary<PlayerBase, ReadOnlyDictionary<DayOfWeek, Place>>();
            foreach (PlayerBase player in Players)
            {
                Dictionary<DayOfWeek, Place> _playerVisitedPlaces = new Dictionary<DayOfWeek, Place>();
                this._internalVisitedPlaces.Add(player, _playerVisitedPlaces);
                this._visitedPlaces.Add(player, new ReadOnlyDictionary<DayOfWeek, Place>(_playerVisitedPlaces));
            }
            this.VisitedPlaces = new ReadOnlyDictionary<PlayerBase, ReadOnlyDictionary<DayOfWeek, Place>>(this._visitedPlaces);

            this._currentPlayerBonuses = new List<PlayerBonusCard>();
            this.CurrentPlayerBonuses = new ReadOnlyCollection<PlayerBonusCard>(_currentPlayerBonuses);

            this._restaurantTracks = new Dictionary<Restaurant, RestaurantTrack>
            {
                { Restaurant.Russo, new RestaurantTrack(this.Players, new int[] { 4 }, 2) },
                { Restaurant.Palatus, new RestaurantTrack(this.Players, new int[] { 4, 7 }, 2) },
                { Restaurant.GustoDiBacio, new RestaurantTrack(this.Players, new int[] { 4, 6 }, 2) },
                { Restaurant.Silva, new RestaurantTrack(this.Players, new int[] { 3, 6 }, 3) },
                { Restaurant.Panorama, new RestaurantTrack(this.Players, new int[] { 3, 5 }, 3) },
                { Restaurant.JoeAndLeos, new RestaurantTrack(this.Players, new int[] { 2, 4, 6 }, 4) }
            };
            this.RestaurantTracks = new ReadOnlyDictionary<Restaurant, RestaurantTrack>(_restaurantTracks);

            _separatedScores = new Dictionary<PlayerBase, Dictionary<VictoryPointsSource, int>>();
            Players.ForEach(player =>
            {
                _separatedScores.Add(player, new Dictionary<VictoryPointsSource, int>());
                foreach (VictoryPointsSource v in Enum.GetValues(typeof(VictoryPointsSource)))
                {
                    _separatedScores[player].Add(v, 0);
                }
            });
        }

        internal Func<PlayerBase, List<DessertCard>> DessertsHandler { get; set; }

        internal IEnumerable<DessertCard> GetDessertsFromPlayer(PlayerBase player)
        {
            return this.DessertsHandler(player);
        }

        internal void AddTeamScore(int v)
        {
            this.TeamScore += v;
            if (this.TeamScore >= Game.MAX_TEAM_SCORE)
            {
                this.TeamScore = Game.MAX_TEAM_SCORE;
            }
            if (this.TeamScore < 0)
            {
                this.TeamScore = 0;
            }
        }

        internal void AddVictoryPointsToPlayer(int points, PlayerBase player, VictoryPointsSource source)
        {
            this._separatedScores[player][source] += points;
            this._playerScores[player] += points;
        }

        internal void AddCashToPlayer(int cash, PlayerBase player)
        {
            if (this._playerCash[player] + cash < 0)
            {
                throw new CantSpendCashException();
            }
            this._playerCash[player] += cash;

        }

        public bool HasMajority(DayOfWeek day)
        {
            return RestaurantWithMajority(day) != null;
        }

        public bool HasUnanimity(DayOfWeek day)
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

        public bool IsSomeoneAlone(DayOfWeek day)
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

        public bool RestaurantHasModifierForThisDay<T>(Restaurant restaurant, DayOfWeek day)
            where T : RestaurantDailyModifierCard
        {
            return Restaurants[restaurant].Modifier is T && Restaurants[restaurant].Modifier.Days.Contains(day);
        }

        internal void ClearVisitedPlaces()
        {
            this._internalVisitedPlaces.Clear();
            this._visitedPlaces.Clear();
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

        public bool IsPlayerInMajority(DayOfWeek day, PlayerBase player)
        {
            Restaurant? majorityRestaurant = RestaurantWithMajority(day);
            return majorityRestaurant != null ? Restaurants[majorityRestaurant.Value].Visitors[day].Contains(player) : false;
        }

        public bool IsPlayerAlone(DayOfWeek day, PlayerBase player)
        {
            if (Home.Visitors[day].Contains(player))
            {
                return true;
            }
            return VisitedPlaces[player][day].Visitors[day].Count == 1;
        }

        public Restaurant? RestaurantWithMajority(DayOfWeek day)
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
            this._currentPlayerBonuses.Clear();
            this._currentPlayerBonuses.AddRange(bonuses);
        }
    }
}
