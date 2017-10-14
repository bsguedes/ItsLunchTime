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
}
