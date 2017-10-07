using System.Collections.Generic;

namespace ItsLunchTimeCore.Decks
{
    internal class RestaurantDailyModifierDeck : Deck<RestaurantDailyModifierCard>
    {
        internal override IEnumerable<RestaurantDailyModifierCard> GetCards()
        {
            throw new System.NotImplementedException();
        }
    }

    public class RestaurantDailyModifierCard : Card
    {

    }
}