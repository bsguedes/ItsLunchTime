using System;
using System.Collections.Generic;
using System.Linq;

namespace ItsLunchTimeCore.Decks
{
    internal class TeamBonusDeck : Deck<TeamBonusCard>
    {
        internal override IEnumerable<TeamBonusCard> GetCards()
        {
            yield return new NoPriceIncreasedThisWeek();
            yield return new NoMajorityInPastaRestaurant();
            yield return new NoMajorityInVegetarianRestaurant();
            yield return new NoOneAteFromHome();
            yield return new WentOnceToUndesired();
            yield return new AllEatBurgerAndPizzaThreeTimes();
            yield return new AllEatBrazilianAndChineseThreeTimes();
            yield return new AllParticipatedIn2Majorities();
            yield return new NoMoreThan1MajorityInARestaurant();
            yield return new NoMoreThan15CashDifference();
            yield return new AllEatAllFoodAtLeastTwice();
            yield return new CollectivelyDonate7Cash();
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
            return !board.Restaurants.Any(x => x.Value.Visitors.Count() >= board.Players.Count + 2);
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
            Dictionary<PlayerBase, bool> status = new Dictionary<PlayerBase, bool>();
            foreach (PlayerBase player in board.Players)
            {
                status[player] = false;
                foreach (DayOfWeek day in Extensions.Weekdays)
                {
                    if (board.VisitedPlaces[player][day] is RestaurantPlace && board.UndesiredRestaurants[player] == (board.VisitedPlaces[player][day] as RestaurantPlace).Identifier)
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
            Dictionary<PlayerBase, int> statusBurger = new Dictionary<PlayerBase, int>();
            Dictionary<PlayerBase, int> statusPizza = new Dictionary<PlayerBase, int>();
            foreach (PlayerBase player in board.Players)
            {
                statusBurger.Add(player, 0);
                statusPizza.Add(player, 0);
                foreach (DayOfWeek day in Extensions.Weekdays)
                {
                    statusBurger[player] += board.VisitedPlaces[player][day].Menu.Count(x => x == FoodType.Burger);
                    statusPizza[player] += board.VisitedPlaces[player][day].Menu.Count(x => x == FoodType.Pizza);
                }
            }
            return statusBurger.Values.All(x => x >= 3) && statusPizza.Values.All(x => x >= 3);
        }
    }

    public class AllEatBrazilianAndChineseThreeTimes : TeamBonusCard
    {
        internal override bool HasCompletedTeamBonus(PublicBoard board)
        {
            Dictionary<PlayerBase, int> statusBrazilian = new Dictionary<PlayerBase, int>();
            Dictionary<PlayerBase, int> statusChinese = new Dictionary<PlayerBase, int>();
            foreach (PlayerBase player in board.Players)
            {
                statusBrazilian.Add(player, 0);
                statusChinese.Add(player, 0);
                foreach (DayOfWeek day in Extensions.Weekdays)
                {
                    statusBrazilian[player] += board.VisitedPlaces[player][day].Menu.Count(x => x == FoodType.Brazilian);
                    statusChinese[player] += board.VisitedPlaces[player][day].Menu.Count(x => x == FoodType.Chinese);
                }
            }
            return statusBrazilian.Values.All(x => x >= 3) && statusChinese.Values.All(x => x >= 3);
        }
    }

    public class AllParticipatedIn2Majorities : TeamBonusCard
    {
        internal override bool HasCompletedTeamBonus(PublicBoard board)
        {
            Dictionary<PlayerBase, int> status = new Dictionary<PlayerBase, int>();
            foreach (PlayerBase player in board.Players)
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
            IEnumerable<int> cash = board.PlayerCash.Values;
            return cash.Max() - cash.Min() <= 15;
        }
    }

    public class AllEatAllFoodAtLeastTwice : TeamBonusCard
    {
        internal override bool HasCompletedTeamBonus(PublicBoard board)
        {
            Dictionary<PlayerBase, Dictionary<FoodType, int>> dictionary = new Dictionary<PlayerBase, Dictionary<FoodType, int>>();
            foreach (PlayerBase player in board.Players)
            {
                dictionary.Add(player, new Dictionary<FoodType, int>());
                foreach (FoodType food in Extensions.FoodTypes)
                {
                    dictionary[player].Add(food, 0);
                }
                foreach (DayOfWeek day in Extensions.Weekdays)
                {
                    foreach (FoodType food in board.VisitedPlaces[player][day].Menu)
                    {
                        dictionary[player][food]++;
                    }
                }
            }
            return dictionary.All(x => x.Value.All(y => y.Value >= 2));
        }
    }

    public class CollectivelyDonate7Cash : TeamBonusCard
    {
        internal override bool HasCompletedTeamBonus(PublicBoard board)
        {
            Dictionary<PlayerBase, Dictionary<PlayerBase, int>> _opinion = new Dictionary<PlayerBase, Dictionary<PlayerBase, int>>();
            board.Players.ForEach(player =>
           {
               _opinion.Add(player, player.AskOpinionForDonationTeamObjective(board));
           });

            Dictionary<PlayerBase, int> _intents = new Dictionary<PlayerBase, int>();
            board.Players.ForEach(player =>
            {
                _intents.Add(player, player.AskForDonationTeamObjectiveIntent(board, _opinion));
            });

            Dictionary<PlayerBase, int> _response = new Dictionary<PlayerBase, int>();
            board.Players.ForEach(player =>
            {
                _response.Add(player, player.AskForDonationTeamObjective(board, _opinion, _intents));
            });

            while (_response.Values.Sum() > 7)
            {
                _response[_response.Where(x => x.Value == _response.Values.Max()).Scramble().First().Key]--;
            }

            return _response.Values.Sum() >= 7;
        }
    }
}
