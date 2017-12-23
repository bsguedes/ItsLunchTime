using System;
using System.Collections.ObjectModel;
using ItsLunchTimeCore.Decks;
using System.Collections.Generic;

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
        private Dictionary<DayOfWeek, ReadOnlyCollection<PlayerDescriptor>> _visitors;
        private Dictionary<DayOfWeek, List<PlayerDescriptor>> _list_visitors;

        internal Place()
        {
            this._visitors = new Dictionary<DayOfWeek, ReadOnlyCollection<PlayerDescriptor>>();
            this._list_visitors = new Dictionary<DayOfWeek, List<PlayerDescriptor>>();
            Visitors = new ReadOnlyDictionary<DayOfWeek, ReadOnlyCollection<PlayerDescriptor>>(_visitors);

            foreach (DayOfWeek dow in Extensions.Weekdays)
            {
                _list_visitors[dow] = new List<PlayerDescriptor>();
                this._visitors.Add(dow, new ReadOnlyCollection<PlayerDescriptor>(_list_visitors[dow]));
            }
        }

        internal void VisitPlace(PlayerDescriptor player, DayOfWeek dow)
        {
            this._list_visitors[dow].Add(player);
        }

        public ReadOnlyDictionary<DayOfWeek, ReadOnlyCollection<PlayerDescriptor>> Visitors { get; private set; }
        public ReadOnlyCollection<FoodType> Menu { get; private set; }

        internal bool HasPlayerVisited(PlayerDescriptor player, DayOfWeek dayOfWeek)
        {
            return this.Visitors[dayOfWeek].Contains(player);
        }
    }

    public class RestaurantPlace : Place
    {
        public string Name { get; }        
        public int Price { get; private set; }
        public Restaurant RestaurantIdentifier { get; }        
        public RestaurantDailyModifierCard Modifier { get; internal set; }        
    }

    public class Home : Place
    {
        
    }
}
