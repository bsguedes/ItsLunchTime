using System.Collections.Generic;

namespace ItsLunchTimeCore.Decks
{
    internal class FavoriteFoodDeck : Deck<FoodCard>
    {
        public const int FOOD_TYPE_CARDS_REPEAT_COUNT = 3;

        internal override IEnumerable<FoodCard> GetCards()
        {
            foreach (FoodType type in Extensions.FoodTypes)
            {
                for (int i = 0; i < FOOD_TYPE_CARDS_REPEAT_COUNT; i++)
                {
                    yield return new FoodCard(type);
                }
            }
        }
    }

    public class FoodCard : Card
    {
        public FoodType Type { get; }

        public FoodCard(FoodType type)
        {
            this.Type = type;
        }
    }
}
