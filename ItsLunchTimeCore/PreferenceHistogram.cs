using System.Collections.Generic;
using System.Linq;

namespace ItsLunchTimeCore
{
    public class PreferenceHistogram
    {
        public PlayerBase Player { get; internal set; }
        public Dictionary<Place, int> Preferences { get; }

        public PreferenceHistogram(Dictionary<Place, int> dict)
        {
            if (dict.Values.Sum() == 100)
            {
                this.Preferences = dict;
            }
        }
    }
}