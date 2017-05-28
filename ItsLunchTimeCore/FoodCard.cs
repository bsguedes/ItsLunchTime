using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItsLunchTimeCore
{
    public enum FoodType
    {
        Brazilian,
        Chinese,
        Burger,
        Pizza,
        Pasta,
        Vegetarian
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
