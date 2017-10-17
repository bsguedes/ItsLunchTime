using System;
using System.Collections.Generic;

namespace ItsLunchTimeCore.Decks
{
    internal class RestaurantDailyModifierDeck : Deck<RestaurantDailyModifierCard>
    {
        internal override IEnumerable<RestaurantDailyModifierCard> GetCards()
        {
            foreach(DayOfWeek day in Extensions.Weekdays)
            {
                yield return new Closed(new DayOfWeek[] { day });
                yield return new OneTeamPointIfMajority(new DayOfWeek[] { day });
                yield return new OneDollarDiscount(new DayOfWeek[] { day });
            }

            DayOfWeek[][] double_days = new DayOfWeek[][]
            {
                new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Wednesday},
                new DayOfWeek[] { DayOfWeek.Tuesday, DayOfWeek.Thursday },
                new DayOfWeek[] { DayOfWeek.Wednesday, DayOfWeek.Friday },
                new DayOfWeek[] { DayOfWeek.Thursday, DayOfWeek.Monday },
                new DayOfWeek[] { DayOfWeek.Friday, DayOfWeek.Tuesday }
            };

            foreach( DayOfWeek[] days in double_days)
            {
                yield return new DoesNotAdvanceTrackPlus2VictoryPoints(days);
                yield return new OneDollarIncrease(days);
                yield return new OneVictoryPointBonus(days);
            }            
        }
    }

    public abstract class RestaurantDailyModifierCard : Card
    {
        public IEnumerable<DayOfWeek> Days { get; }

        internal RestaurantDailyModifierCard(DayOfWeek[] days)
        {
            this.Days = days;
        }

    }

    public class Closed : RestaurantDailyModifierCard
    {
        internal Closed(DayOfWeek[] days) : base(days)
        {

        }
    }

    public class OneTeamPointIfMajority : RestaurantDailyModifierCard
    {
        internal OneTeamPointIfMajority(DayOfWeek[] days) : base(days)
        {

        }
    }

    public class OneDollarDiscount : RestaurantDailyModifierCard
    {
        internal OneDollarDiscount(DayOfWeek[] days) : base(days)
        {

        }
    }

    public class OneVictoryPointBonus : RestaurantDailyModifierCard
    {
        internal OneVictoryPointBonus(DayOfWeek[] days) : base(days)
        {

        }
    }

    public class OneDollarIncrease : RestaurantDailyModifierCard
    {
        internal OneDollarIncrease(DayOfWeek[] days) : base(days)
        {

        }
    }

    public class DoesNotAdvanceTrackPlus2VictoryPoints : RestaurantDailyModifierCard
    {
        internal DoesNotAdvanceTrackPlus2VictoryPoints(DayOfWeek[] days) : base(days)
        {

        }
    }
}