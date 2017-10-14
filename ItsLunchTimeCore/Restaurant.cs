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

    public class RestaurantPlace
    {
        public string Name { get; private set; }
        public int Price { get; private set; }
        public ReadOnlyCollection<FoodType> Menu { get; private set; }
        public ReadOnlyDictionary<DayOfWeek, ReadOnlyCollection<Player>> Visitors { get; private set; }
        public RestaurantDailyModifierCard Modifier { get; internal set; }

        internal bool HasPlayerVisited(Player player, DayOfWeek dayOfWeek)
        {
            return this.Visitors[dayOfWeek].Contains(player);
        }
    }

    public class Home
    {
        public ReadOnlyDictionary<DayOfWeek, ReadOnlyCollection<Player>> Visitors { get; private set; }
    }
}
