using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItsLunchTimeCore.Decks
{
    internal class PlayerBonusDeck : Deck<PlayerBonusCard>
    {
        internal override IEnumerable<PlayerBonusCard> GetCards()
        {
            yield return new LastInATrackAlone();
            yield return new LeadATrackAlone();
            yield return new ThreeOnARowSameRestaurant();
            yield return new NoFoodFromHome();
            yield return new NeverLunchAlone();
            yield return new Eat3TimesChinese();
            yield return new WentToFourDistinctRestaurants();
            yield return new HadBeenPartOfMajority3Times();
            yield return new NoBrazilianFood();
            yield return new EatSameTypeOfFood5Times();
            yield return new NeverEatTwiceInARowSameRestaurant();
            yield return new MissedMajorityAtMaxOnce();
        }
    }

    public abstract class PlayerBonusCard : Card
    {
        public abstract int Points { get; }

        internal abstract bool HasCompletedForPlayer(Player player, PublicBoard board);
    }

    public class LastInATrackAlone : PlayerBonusCard
    {
        public override int Points => 4;

        internal override bool HasCompletedForPlayer(Player player, PublicBoard board)
        {
            foreach(Restaurant restaurant in Enum.GetValues(typeof(Restaurant)))
            {
                RestaurantTrack track = board.RestaurantTracks[restaurant];
                int playerScore = track.PlayerScores[player];
                if (track.PlayerScores.Values.Where(x => x <= playerScore).Count() == 1)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class LeadATrackAlone : PlayerBonusCard
    {
        public override int Points => 4;

        internal override bool HasCompletedForPlayer(Player player, PublicBoard board)
        {
            foreach (Restaurant restaurant in Enum.GetValues(typeof(Restaurant)))
            {
                RestaurantTrack track = board.RestaurantTracks[restaurant];
                int playerScore = track.PlayerScores[player];
                if (track.PlayerScores.Values.Where(x => x >= playerScore).Count() == 1)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class ThreeOnARowSameRestaurant : PlayerBonusCard
    {
        public override int Points => 5;

        internal override bool HasCompletedForPlayer(Player player, PublicBoard board)
        {
            foreach(Restaurant restaurant in Extensions.Restaurants)
            {
                if (board.Restaurants[restaurant].HasPlayerVisited(player, DayOfWeek.Monday) && 
                    board.Restaurants[restaurant].HasPlayerVisited(player, DayOfWeek.Tuesday) && 
                    board.Restaurants[restaurant].HasPlayerVisited(player, DayOfWeek.Wednesday))
                {
                    return true;
                }
                if (board.Restaurants[restaurant].HasPlayerVisited(player, DayOfWeek.Tuesday) && 
                    board.Restaurants[restaurant].HasPlayerVisited(player, DayOfWeek.Wednesday ) && 
                    board.Restaurants[restaurant].HasPlayerVisited(player, DayOfWeek.Thursday))
                {
                    return true;
                }
                if (board.Restaurants[restaurant].HasPlayerVisited(player, DayOfWeek.Wednesday) && 
                    board.Restaurants[restaurant].HasPlayerVisited(player, DayOfWeek.Thursday) && 
                    board.Restaurants[restaurant].HasPlayerVisited(player, DayOfWeek.Friday ))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class NoFoodFromHome : PlayerBonusCard
    {
        public override int Points => 3;

        internal override bool HasCompletedForPlayer(Player player, PublicBoard board)
        {
            foreach(DayOfWeek day in Extensions.Weekdays)
            {
                if (board.Home.HasPlayerVisited(player, day))
                {
                    return false;
                }
            }
            return true;            
        }
    }

    public class NeverLunchAlone : PlayerBonusCard
    {
        public override int Points => 5;

        internal override bool HasCompletedForPlayer(Player player, PublicBoard board)
        {
            foreach (DayOfWeek day in Extensions.Weekdays)
            {
                if (board.Home.Visitors[day].Contains(player))
                {
                    return false;
                }
                foreach(Restaurant restaurant in Extensions.Restaurants)
                {
                    if (board.Restaurants[restaurant].Visitors[day].Contains(player) && board.Restaurants[restaurant].Visitors[day].Count == 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public class Eat3TimesChinese : PlayerBonusCard
    {
        public override int Points => 4;

        internal override bool HasCompletedForPlayer(Player player, PublicBoard board)
        {
            return board.PlayerDescriptors[player.Character].VisitedPlaces.Values
                .Where(x => x is RestaurantPlace)
                .Where(y => ((RestaurantPlace)y).Menu.Contains(FoodType.Chinese)).Count() >= 3;
        }
    }

    public class WentToFourDistinctRestaurants : PlayerBonusCard
    {
        public override int Points => 6;

        internal override bool HasCompletedForPlayer(Player player, PublicBoard board)
        {
            return board.PlayerDescriptors[player.Character].VisitedPlaces.Values.Where(x => !(x is Home)).Distinct().Count() >= 4;
        }
    }

    public class HadBeenPartOfMajority3Times : PlayerBonusCard
    {
        public override int Points => 5;

        internal override bool HasCompletedForPlayer(Player player, PublicBoard board)
        {
            int majorities = 0;
            foreach (DayOfWeek day in Extensions.Weekdays)
            {
                if (board.IsPlayerInMajority(day, player))
                {
                    majorities++;
                }
            }
            return majorities >= 3;
        }
    }

    public class NoBrazilianFood : PlayerBonusCard
    {
        public override int Points => 4;

        internal override bool HasCompletedForPlayer(Player player, PublicBoard board)
        {
            return board.PlayerDescriptors[player.Character].VisitedPlaces.Values
                .Where(x => x is RestaurantPlace)
                .Where(y => ((RestaurantPlace)y).Menu.Contains(FoodType.Brazilian)).Count() == 0;
        }
    }

    public class EatSameTypeOfFood5Times : PlayerBonusCard
    {
        public override int Points => 5;

        internal override bool HasCompletedForPlayer(Player player, PublicBoard board)
        {
            Dictionary<FoodType, int> foodTypes = new Dictionary<FoodType, int>();
            foreach (FoodType type in Extensions.FoodTypes)
            {
                foodTypes.Add(type, 0);
            }
            foreach (DayOfWeek day in Extensions.Weekdays)
            {
                Place place = board.PlayerDescriptors[player.Character].VisitedPlaces[day];
                if (place is RestaurantPlace)
                {
                    foreach(FoodType foodType in (place as RestaurantPlace).Menu)
                    {
                        foodTypes[foodType]++;
                    }
                }
            }
            return foodTypes.Values.Contains(Extensions.Weekdays.Count());
        }
    }

    public class NeverEatTwiceInARowSameRestaurant : PlayerBonusCard
    {
        public override int Points => 6;

        internal override bool HasCompletedForPlayer(Player player, PublicBoard board)
        {
            PlayerDescriptor descriptor = board.PlayerDescriptors[player.Character];

            Place monday = descriptor.VisitedPlaces[DayOfWeek.Monday];
            Place tuesday = descriptor.VisitedPlaces[DayOfWeek.Tuesday];
            Place wednesday = descriptor.VisitedPlaces[DayOfWeek.Wednesday];
            Place thursday = descriptor.VisitedPlaces[DayOfWeek.Thursday];
            Place friday = descriptor.VisitedPlaces[DayOfWeek.Friday];

            if (monday is RestaurantPlace && tuesday is RestaurantPlace && monday == tuesday)
            {
                return false;
            }
            if (tuesday is RestaurantPlace && wednesday is RestaurantPlace && tuesday == wednesday)
            {
                return false;
            }
            if (wednesday is RestaurantPlace && thursday is RestaurantPlace && wednesday == thursday)
            {
                return false;
            }
            if (thursday is RestaurantPlace && friday is RestaurantPlace && thursday == friday)
            {
                return false;
            }

            return true;
        }
    }

    public class MissedMajorityAtMaxOnce : PlayerBonusCard
    {
        public override int Points => 5;

        internal override bool HasCompletedForPlayer(Player player, PublicBoard board)
        {
            int majorities = 0;
            int playerInMajority = 0;
            foreach(DayOfWeek day in Extensions.Weekdays)
            {
                if (board.HasMajority(day))
                {
                    majorities++;
                    if (board.IsPlayerInMajority(day, player))
                    {
                        playerInMajority++;
                    }
                }
            }
            return majorities - playerInMajority <= 1;
        }
    }
}
