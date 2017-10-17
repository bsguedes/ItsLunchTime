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
