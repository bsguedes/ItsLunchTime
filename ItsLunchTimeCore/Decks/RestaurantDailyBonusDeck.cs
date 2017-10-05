using System.Collections.Generic;

namespace ItsLunchTimeCore.Decks
{
    internal class RestaurantDailyBonusDeck : Deck<RestaurantDailyBonusCard>
    {
        public RestaurantDailyBonusDeck()
        {
        }

        public override IEnumerable<RestaurantDailyBonusCard> GetCards()
        {
            throw new System.NotImplementedException();
        }
    }

    public class RestaurantDailyBonusCard : Card
    {

    }
}