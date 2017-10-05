using System;
using System.Collections.Generic;

namespace ItsLunchTimeCore.Decks
{
    internal class FavoriteFoodDeck : Deck<FoodCard>
    {        
        public const int FOOD_TYPE_CARDS_REPEAT_COUNT = 3;

        public FavoriteFoodDeck()
        {            
            foreach (FoodType type in Enum.GetValues(typeof(FoodCard)))
            {
                for (int i = 0; i < FOOD_TYPE_CARDS_REPEAT_COUNT; i++)
                {
                    Cards.Add(new FoodCard(type));
                }
            }

            base.Shuffle();
        }
    }    

    public class FoodCard : Card
    {
        public FoodType FoodType { get; private set; }

        public FoodCard(FoodType type)
        {
            this.FoodType = type;
        }
    }
}
