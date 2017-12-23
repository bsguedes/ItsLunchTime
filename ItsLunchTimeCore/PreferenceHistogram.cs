using System.Linq;
using System.Collections.Generic;

namespace ItsLunchTimeCore
{
    public class PreferenceHistogram
    {
        public PlayerDescriptor Player { get; internal set; }
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