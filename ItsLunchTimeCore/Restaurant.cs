using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public IRestaurantModifier Modifier { get; private set; }
    }

    public class Home
    {
        public ReadOnlyDictionary<DayOfWeek, ReadOnlyCollection<Player>> Visitors { get; private set; }
    }
}
