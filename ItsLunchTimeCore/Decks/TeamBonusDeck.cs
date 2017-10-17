using System;
using System.Linq;
using System.Collections.Generic;

namespace ItsLunchTimeCore.Decks
{
    internal class TeamBonusDeck : Deck<TeamBonusCard>
    {
        internal override IEnumerable<TeamBonusCard> GetCards()
        {
            throw new System.NotImplementedException();
        }
    }

    public abstract class TeamBonusCard : Card
    {
        internal abstract bool HasCompletedTeamBonus(PublicBoard board);
    }

    public class NoPriceIncreasedThisWeek : TeamBonusCard
    {
        internal override bool HasCompletedTeamBonus(PublicBoard board)
        {
            throw new System.NotImplementedException();
        }
    }

    public class NoMajorityInPastaRestaurant : TeamBonusCard
    {
        internal override bool HasCompletedTeamBonus(PublicBoard board)
        {
            foreach (DayOfWeek day in Extensions.Weekdays)
            {
                foreach (Restaurant restaurant in Extensions.Restaurants)
                {
                    if (board.Restaurants[restaurant].Menu.Contains(FoodType.Pasta) && board.RestaurantWithMajority(day) == restaurant)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public class NoMajorityInVegetarianRestaurant : TeamBonusCard
    {
        internal override bool HasCompletedTeamBonus(PublicBoard board)
        {
            foreach (DayOfWeek day in Extensions.Weekdays)
            {
                foreach (Restaurant restaurant in Extensions.Restaurants)
                {
                    if (board.Restaurants[restaurant].Menu.Contains(FoodType.Vegetarian) && board.RestaurantWithMajority(day) == restaurant)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public class NoOneAteFromHome : TeamBonusCard
    {
        internal override bool HasCompletedTeamBonus(PublicBoard board)
        {
            foreach (DayOfWeek day in Extensions.Weekdays)
            {
                if (board.Home.Visitors[day].Count > 0)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class WentOnceToUndesired : TeamBonusCard
    {        
        internal override bool HasCompletedTeamBonus(PublicBoard board)
        {
            Dictionary<PlayerDescriptor, bool> status = new Dictionary<PlayerDescriptor, bool>();
            foreach (PlayerDescriptor player in board.PlayerDescriptors.Values)
            {
                status[player] = false;
                foreach (DayOfWeek day in Extensions.Weekdays)
                {
                    if (player.VisitedPlaces[day] is RestaurantPlace && player.UndesiredRestaurant == (player.VisitedPlaces[day] as RestaurantPlace).RestaurantIdentifier )
                    {
                        status[player] = true;
                    }
                }
            }
            return status.Values.All(x => x);
        }
    }

    public class AllEatBurgerAndPizzaThreeTimes : TeamBonusCard
    {
        internal override bool HasCompletedTeamBonus(PublicBoard board)
        {
            Dictionary<PlayerDescriptor, int> statusBurger = new Dictionary<PlayerDescriptor, int>();
            Dictionary<PlayerDescriptor, int> statusPizza = new Dictionary<PlayerDescriptor, int>();
            foreach (PlayerDescriptor player in board.PlayerDescriptors.Values)
            {
                statusBurger.Add(player, 0);
                statusPizza.Add(player, 0);
                foreach (DayOfWeek day in Extensions.Weekdays)
                {
                    statusBurger[player] += player.VisitedPlaces[day].Menu.Count(x => x == FoodType.Burger);
                    statusPizza[player] += player.VisitedPlaces[day].Menu.Count(x => x == FoodType.Pizza);
                }
            }
            return statusBurger.Values.All(x => x >= 3) && statusPizza.Values.All( x => x >= 3);
        }
    }

    public class AllEatBrazilianAndChineseThreeTimes : TeamBonusCard
    {
        internal override bool HasCompletedTeamBonus(PublicBoard board)
        {
            Dictionary<PlayerDescriptor, int> statusBrazilian = new Dictionary<PlayerDescriptor, int>();
            Dictionary<PlayerDescriptor, int> statusChinese = new Dictionary<PlayerDescriptor, int>();
            foreach (PlayerDescriptor player in board.PlayerDescriptors.Values)
            {
                statusBrazilian.Add(player, 0);
                statusChinese.Add(player, 0);
                foreach (DayOfWeek day in Extensions.Weekdays)
                {
                    statusBrazilian[player] += player.VisitedPlaces[day].Menu.Count(x => x == FoodType.Brazilian);
                    statusChinese[player] += player.VisitedPlaces[day].Menu.Count(x => x == FoodType.Chinese);
                }
            }
            return statusBrazilian.Values.All(x => x >= 3) && statusChinese.Values.All(x => x >= 3);
        }
    }

    public class AllParticipatedIn2Majorities : TeamBonusCard
    {
        internal override bool HasCompletedTeamBonus(PublicBoard board)
        {
            Dictionary<PlayerDescriptor, int> status = new Dictionary<PlayerDescriptor, int>();          
            foreach (PlayerDescriptor player in board.PlayerDescriptors.Values)
            {                
                status.Add(player, 0);
                foreach (DayOfWeek day in Extensions.Weekdays)
                {
                    status[player] += board.IsPlayerInMajority(day, player) ? 1 : 0;                 
                }
            }
            return status.Values.All(x => x >= 2);
        }
    }

    public class NoMoreThan1MajorityInARestaurant : TeamBonusCard
    {
        internal override bool HasCompletedTeamBonus(PublicBoard board)
        {
            Dictionary<Restaurant, int> majorities = new Dictionary<Restaurant, int>();
            foreach (Restaurant restaurant in Extensions.Restaurants)
            {
                majorities.Add(restaurant, 0);                
            }
            foreach (DayOfWeek day in Extensions.Weekdays)
            {
                if (board.HasMajority(day))
                {
                    majorities[board.RestaurantWithMajority(day).Value]++;
                }
            }
            return majorities.Values.All(x => x <= 1);
        }
    }

    public class NoMoreThan15CashDifference : TeamBonusCard
    {
        internal override bool HasCompletedTeamBonus(PublicBoard board)
        {
            IEnumerable<int> cash = board.PlayerDescriptors.Values.Select(x => x.CurrentCash);
            return cash.Max() - cash.Min() <= 15;
        }
    }

    public class AllEatAllFoodAtLeastTwice : TeamBonusCard
    {
        internal override bool HasCompletedTeamBonus(PublicBoard board)
        {
            Dictionary<PlayerDescriptor, Dictionary<FoodType, int>> dictionary = new Dictionary<PlayerDescriptor, Dictionary<FoodType, int>>();
            foreach(PlayerDescriptor player in board.PlayerDescriptors.Values)
            {
                dictionary.Add(player, new Dictionary<FoodType, int>());
                foreach(FoodType food in Extensions.FoodTypes)
                {
                    dictionary[player].Add(food, 0);
                }
                foreach(DayOfWeek day in Extensions.Weekdays)
                {
                    foreach (FoodType food in board.PlayerDescriptors[player.Character].VisitedPlaces[day].Menu)
                    {
                        dictionary[player][food]++;
                    }
                }
            }
            return dictionary.All(x => x.Value.All(y => y.Value >= 2));
        }
    }
}