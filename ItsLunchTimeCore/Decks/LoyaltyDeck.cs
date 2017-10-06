using System.Collections.Generic;
using ItsLunchTimeCore.Decks;

namespace ItsLunchTimeCore
{
    internal class LoyaltyDeck : Deck<LoyaltyCard>
    {
        public LoyaltyDeck()
        {
        }

        public override IEnumerable<LoyaltyCard> GetCards()
        {
            yield return new LoyaltyCardVIP(Restaurant.Russo, new int[] { 1, 2, 3, 4 }, 1, 1);
        }
    }

    public abstract class LoyaltyCard : Card
    {
        public LoyaltyType Type { get; }

        internal LoyaltyCard(LoyaltyType type)
        {
            this.Type = type;
        }

    }

    public class LoyaltyCardVIP : LoyaltyCard
    {
        public Restaurant Restaurant { get; }

        internal LoyaltyCardVIP(Restaurant restaurant, int[] bonus, int dessertTake, int dessertChooseFrom) : base(LoyaltyType.VIP)
        {

        }
    }
}