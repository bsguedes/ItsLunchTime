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
            throw new System.NotImplementedException();
        }
    }

    public class LoyaltyCard : Card
    {

    }
}