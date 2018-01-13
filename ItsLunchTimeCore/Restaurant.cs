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

        internal Place(int cost)
        {
            this.Cost = cost;
            this._visitors = new Dictionary<DayOfWeek, ReadOnlyCollection<PlayerBase>>();
            this._list_visitors = new Dictionary<DayOfWeek, List<PlayerBase>>();
            Visitors = new ReadOnlyDictionary<DayOfWeek, ReadOnlyCollection<PlayerBase>>(_visitors);

            foreach (DayOfWeek dow in Extensions.Weekdays)
            {
                _list_visitors[dow] = new List<PlayerBase>();
                this._visitors.Add(dow, new ReadOnlyCollection<PlayerBase>(_list_visitors[dow]));
            }
        }

        internal void VisitPlace(PlayerBase player, DayOfWeek dow)
        {
            this._list_visitors[dow].Add(player);
        }

        public ReadOnlyDictionary<DayOfWeek, ReadOnlyCollection<PlayerBase>> Visitors { get; private set; }
        public ReadOnlyCollection<FoodType> Menu { get; private set; }
        public int Cost { get; }

        internal bool HasPlayerVisited(PlayerBase player, DayOfWeek dayOfWeek)
        {
            return this.Visitors[dayOfWeek].Contains(player);
        }
    }

    public class RestaurantPlace : Place
    {
        internal RestaurantPlace(int cost) : base(cost)
        {

        }

        public string Name { get; }
        public int Price { get; private set; }
        public Restaurant Identifier { get; }
        public RestaurantDailyModifierCard Modifier { get; internal set; }

        internal int AdjustPrice(int count)
        {
            int visitorCount = this.Visitors.Values.Sum(x => x.Count);
            if (visitorCount <= count - 1) return -1;
            if (visitorCount >= count + 2) return 1;
            return 0;
        }
    }

    public class Home : Place
    {
        internal Home() : base(0)
        {

        }
    }
}
