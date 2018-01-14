using ItsLunchTimeCore.Decks;
using System.Collections.Generic;
using System.Linq;

namespace ItsLunchTimeCore
{
    internal class LoyaltyDeck : Deck<LoyaltyCard>
    {
        internal override IEnumerable<LoyaltyCard> GetCards()
        {
            yield return new LoyaltyCardVIP(Restaurant.Russo, new int[] { 1, 2, 3, 4 }, 1, 1);
            yield return new LoyaltyCardVIP(Restaurant.Palatus, new int[] { 1, 3, 5, 7 }, 1, 1);
            yield return new LoyaltyCardVIP(Restaurant.GustoDiBacio, new int[] { 2, 4, 6, 8 }, 2, 1);
            yield return new LoyaltyCardVIP(Restaurant.Silva, new int[] { 2, 4, 6, 8 }, 3, 1);
            yield return new LoyaltyCardVIP(Restaurant.Panorama, new int[] { 3, 5, 7, 10 }, 2, 2);
            yield return new LoyaltyCardVIP(Restaurant.JoeAndLeos, new int[] { 4, 6, 8, 11 }, 3, 1);
            yield return new LoyaltyCardGOLD(Restaurant.Russo, new int[] { 1, 2, 4 }, 2);
            yield return new LoyaltyCardGOLD(Restaurant.Palatus, new int[] { 2, 4, 6 }, 2);
            yield return new LoyaltyCardGOLD(Restaurant.GustoDiBacio, new int[] { 2, 5, 8 }, 3);
            yield return new LoyaltyCardGOLD(Restaurant.Silva, new int[] { 3, 6, 9 }, 3);
            yield return new LoyaltyCardGOLD(Restaurant.Panorama, new int[] { 4, 6, 9 }, 4);
            yield return new LoyaltyCardGOLD(Restaurant.JoeAndLeos, new int[] { 4, 7, 11 }, 5);
            yield return new LoyaltyCardPLUS(Restaurant.Russo, new int[] { 2, 4, 7, 11 });
            yield return new LoyaltyCardPLUS(Restaurant.Palatus, new int[] { 3, 5, 8, 12 });
            yield return new LoyaltyCardPLUS(Restaurant.GustoDiBacio, new int[] { 3, 6, 10, 13 });
            yield return new LoyaltyCardPLUS(Restaurant.Silva, new int[] { 4, 7, 11, 14 });
            yield return new LoyaltyCardPLUS(Restaurant.Panorama, new int[] { 5, 8, 13, 16 });
            yield return new LoyaltyCardPLUS(Restaurant.JoeAndLeos, new int[] { 6, 10, 15, 19 });
        }
    }

    public abstract class LoyaltyCard : Card
    {
        public LoyaltyType Type { get; }
        public Restaurant Restaurant { get; }
        public int[] VictoryPoints { get; }

        internal LoyaltyCard(LoyaltyType type, Restaurant restaurant, int[] bonus)
        {
            this.Restaurant = restaurant;
            this.Type = type;
            this.VictoryPoints = new int[Extensions.Weekdays.Length + 1];
            this.VictoryPoints[0] = 0;
            for (int i = 0; i < bonus.Length; i++)
            {
                this.VictoryPoints[i + 1] = bonus[i];
            }
            for (int i = bonus.Length; i <= Extensions.Weekdays.Length; i++)
            {
                this.VictoryPoints[i] = bonus.Max();
            }
        }

    }

    public class LoyaltyCardVIP : LoyaltyCard
    {
        public int DessertTakeCount { get; }
        public int DessertOptions { get; }

        internal LoyaltyCardVIP(Restaurant restaurant, int[] bonus, int dessertTake, int dessertChooseFrom) : base(LoyaltyType.VIP, restaurant, bonus)
        {
            this.DessertTakeCount = dessertTake;
            this.DessertOptions = dessertChooseFrom;
        }
    }

    public class LoyaltyCardPLUS : LoyaltyCard
    {
        internal LoyaltyCardPLUS(Restaurant restaurant, int[] bonus) : base(LoyaltyType.PLUS, restaurant, bonus)
        {

        }
    }

    public class LoyaltyCardGOLD : LoyaltyCard
    {
        public int ExtraCash { get; }

        internal LoyaltyCardGOLD(Restaurant restaurant, int[] bonus, int moneyTake) : base(LoyaltyType.GOLD, restaurant, bonus)
        {
            this.ExtraCash = moneyTake;
        }
    }
}