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
            throw new NotImplementedException();
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
            foreach(RestaurantPlace restaurant in board.Restaurants)
            {
                if (restaurant.HasPlayerVisited(player, DayOfWeek.Monday) && restaurant.HasPlayerVisited(player, DayOfWeek.Tuesday) && restaurant.HasPlayerVisited(player, DayOfWeek.Wednesday))
                {
                    return true;
                }
                if (restaurant.HasPlayerVisited(player, DayOfWeek.Tuesday) && restaurant.HasPlayerVisited(player, DayOfWeek.Wednesday ) && restaurant.HasPlayerVisited(player, DayOfWeek.Thursday))
                {
                    return true;
                }
                if (restaurant.HasPlayerVisited(player, DayOfWeek.Wednesday) && restaurant.HasPlayerVisited(player, DayOfWeek.Thursday) && restaurant.HasPlayerVisited(player, DayOfWeek.Friday ))
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
                foreach(RestaurantPlace restaurant in board.Restaurants)
                {
                    if (restaurant.Visitors[day].Contains(player) && restaurant.Visitors[day].Count == 1)
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



}
