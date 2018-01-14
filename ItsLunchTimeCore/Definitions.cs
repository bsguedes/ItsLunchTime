using System;
using System.Collections.Generic;
using System.Linq;

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
        public static readonly DayOfWeek[] DaysOfWeek = new DayOfWeek[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday };
        public static readonly Restaurant[] Restaurants = new Restaurant[] { Restaurant.Russo, Restaurant.Palatus, Restaurant.GustoDiBacio, Restaurant.Silva, Restaurant.Panorama, Restaurant.JoeAndLeos };
        public static readonly FoodType[] FoodTypes = new FoodType[] { FoodType.Brazilian, FoodType.Burger, FoodType.Chinese, FoodType.Pasta, FoodType.Pizza, FoodType.Vegetarian };
        public static readonly DessertType[] DessertTypes = new DessertType[] { DessertType.Cake, DessertType.Coffee, DessertType.Cream, DessertType.IceCream, DessertType.Pudding, DessertType.Sagu };

        public static readonly Dictionary<Character, int> StartingMoney = new Dictionary<Character, int>
        {
            { Character.CEO, 60 },
            { Character.Environment, 60 },
            { Character.Finance, 60 },
            { Character.ForeignAffairs, 60 },
            { Character.HR, 60 },
            { Character.Intern, 60 },
            { Character.Marketing, 60 },
            { Character.Programmer, 60 },
            { Character.SalesRep, 60 },
            { Character.WarehouseManager, 60 },
        };

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

        public static bool ActionForCharacter(this IEnumerable<PlayerBase> players, Character character, Action<PlayerBase> action)
        {
            foreach (PlayerBase player in players)
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

        public static IEnumerable<T> Scramble<T>(this IEnumerable<T> coll)
        {
            List<T> list = coll.ToList();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

    }
}
