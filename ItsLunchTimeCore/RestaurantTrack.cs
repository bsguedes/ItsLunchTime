using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItsLunchTimeCore
{
    public delegate void RestaurantTrackRewardDelegate();    

    public class RestaurantTrack
    {
        public ReadOnlyDictionary<PlayerDescriptor, int> PlayerScores { get; private set; }
        internal Dictionary<int, RestaurantTrackRewardDelegate> Rewards { get; private set; }
    }
}
