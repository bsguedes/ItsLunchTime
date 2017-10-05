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

    public static class Extensions
    {
        private static Random rng = new Random();

        public static void Each(this int n, Action<int> action)
        {
            for (int i = 0; i < n; i++)
            {
                action(i);
            }
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
