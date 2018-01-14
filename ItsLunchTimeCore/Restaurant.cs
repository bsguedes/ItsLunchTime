using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ItsLunchTimeCore
{
    public enum Restaurant
    {
        Russo,
        Palatus,
        GustoDiBacio,
        Silva,
        Panorama,
        JoeAndLeos
    }

    public abstract class Place
    {
        private Dictionary<DayOfWeek, ReadOnlyCollection<PlayerBase>> _visitors;
        private Dictionary<DayOfWeek, List<PlayerBase>> _list_visitors;
        private List<FoodType> _menu;
        public int BaseCost { get; }

        internal Place(int cost)
        {
            this.BaseCost = cost;
            this.Cost = cost;
            this._visitors = new Dictionary<DayOfWeek, ReadOnlyCollection<PlayerBase>>();
            this._list_visitors = new Dictionary<DayOfWeek, List<PlayerBase>>();
            Visitors = new ReadOnlyDictionary<DayOfWeek, ReadOnlyCollection<PlayerBase>>(_visitors);
            this._menu = new List<FoodType>();
            Menu = new ReadOnlyCollection<FoodType>(_menu);

            foreach (DayOfWeek dow in Extensions.Weekdays)
            {
                _list_visitors[dow] = new List<PlayerBase>();
                this._visitors.Add(dow, new ReadOnlyCollection<PlayerBase>(_list_visitors[dow]));
            }
        }

        internal void ClearRestaurant()
        {
            this._menu.Clear();
            this._visitors.Clear();
            foreach (DayOfWeek dow in Extensions.Weekdays)
            {
                _list_visitors[dow] = new List<PlayerBase>();
                this._visitors.Add(dow, new ReadOnlyCollection<PlayerBase>(_list_visitors[dow]));
            }
        }

        internal void AddFoodToMenu(FoodType food)
        {
            this._menu.Add(food);
        }

        internal void VisitPlace(PlayerBase player, DayOfWeek dow)
        {
            this._list_visitors[dow].Add(player);
        }

        public ReadOnlyDictionary<DayOfWeek, ReadOnlyCollection<PlayerBase>> Visitors { get; private set; }
        public ReadOnlyCollection<FoodType> Menu { get; }
        public int Cost { get; internal set; }

        internal bool HasPlayerVisited(PlayerBase player, DayOfWeek dayOfWeek)
        {
            return this.Visitors[dayOfWeek].Contains(player);
        }
    }

    public class RestaurantPlace : Place
    {
        internal RestaurantPlace(Restaurant identifier, int cost, FoodType baseFood) : base(cost)
        {
            this.BaseFood = baseFood;
            this.AddFoodToMenu(baseFood);
            this.Identifier = identifier;
        }

        public FoodType BaseFood { get; }
        public string Name { get; }
        public Restaurant Identifier { get; }
        public RestaurantDailyModifierCard Modifier { get; private set; }

        internal void AdjustPrice(int count)
        {
            int adjust = 0;
            int visitorCount = this.Visitors.Values.Sum(x => x.Count);
            if (visitorCount <= count - 1) adjust = -1;
            if (visitorCount >= count + 2) adjust = 1;

            if (!(this.Cost + adjust < BaseCost - 1 || this.Cost + adjust > BaseCost + 1))
            {
                this.Cost += adjust;
            }
        }

        internal void SetDailyModifier(RestaurantDailyModifierCard card)
        {
            this.Modifier = card;
        }
    }

    public class Home : Place
    {
        internal Home() : base(0)
        {

        }
    }
}
