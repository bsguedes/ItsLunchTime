using System;
using System.Collections.ObjectModel;
using ItsLunchTimeCore.Decks;

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
        public ReadOnlyDictionary<DayOfWeek, ReadOnlyCollection<Player>> Visitors { get; private set; }

        internal bool HasPlayerVisited(Player player, DayOfWeek dayOfWeek)
        {
            return this.Visitors[dayOfWeek].Contains(player);
        }
    }

    public class RestaurantPlace : Place
    {
        public string Name { get; private set; }
        public int Price { get; private set; }
        public ReadOnlyCollection<FoodType> Menu { get; private set; }        
        public RestaurantDailyModifierCard Modifier { get; internal set; }
        
    }

    public class Home : Place
    {
        
    }
}
