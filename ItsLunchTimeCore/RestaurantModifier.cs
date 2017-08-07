using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItsLunchTimeCore
{
    public interface IRestaurantModifier
    {
    }

    public class DiscountRestaurantModifier : IRestaurantModifier
    {

    }

    public class ClosedRestaurantModifier : IRestaurantModifier
    {

    }

    public class NoBanRestaurantModifier : IRestaurantModifier
    {

    }

    public class TeamMajorityRestaurantModifier : IRestaurantModifier
    {

    }

    public class TeamPointsPerPlayerRestaurantModifier : IRestaurantModifier
    {

    }

    public class VictoryPointsToPlayerRestaurantModifier : IRestaurantModifier
    {

    }

    
}
