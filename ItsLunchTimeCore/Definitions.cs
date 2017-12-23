using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItsLunchTimeCore
{
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }

    public enum FoodType
    {
        Brazilian,
        Chinese,
        Burger,
        Pizza,
        Pasta,
        Vegetarian
    }

    public enum DessertType
    {
        Coffee,
        IceCream,
        Sagu,
        Cream,
        Pudding,
        Cake
    }

    public enum LoyaltyType
    {
        VIP,
        GOLD,
        PLUS
    }    

    public static class Extensions
    {
        private static Random rng = new Random();

        public static readonly DayOfWeek[] Weekdays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
        public static readonly Restaurant[] Restaurants = new Restaurant[] { Restaurant.Russo, Restaurant.Palatus, Restaurant.GustoDiBacio, Restaurant.Silva, Restaurant.Panorama, Restaurant.JoeAndLeos };
        public static readonly FoodType[] FoodTypes = new FoodType[] { FoodType.Brazilian, FoodType.Burger, FoodType.Chinese, FoodType.Pasta, FoodType.Pizza, FoodType.Vegetarian };
        public static readonly DessertType[] DessertTypes = new DessertType[] { DessertType.Cake, DessertType.Coffee, DessertType.Cream, DessertType.IceCream, DessertType.Pudding, DessertType.Sagu };       

        public static void Times(this int n, Action<int> action)
        {
            for (int i = 0; i < n; i++)
            {
                action(i);
            }
        }        

        public static void Times(this int n, Action action)
        {
            n.Times((i) => action());
        }

        public static bool ActionForCharacter(this IEnumerable<Player> players, Character character, Action<Player> action)
        {
            foreach (Player player in players)
            {
                if (player.Character == character)
                {
                    action(player);
                    return true;
                }
            }
            return false;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
