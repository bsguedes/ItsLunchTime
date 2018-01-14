using System.Collections.Generic;
using System.Linq;

namespace ItsLunchTimeCore
{
    public class PreferenceHistogram
    {
        public PlayerBase Player { get; internal set; }
        public Dictionary<Place, int> Preferences { get; private set; }
        public bool Normalized { get; private set; }

        private PreferenceHistogram()
        {
            this.Normalized = false;
            this.Preferences = new Dictionary<Place, int>();
        }

        public PreferenceHistogram(PublicBoard board) : this()
        {
            this.Preferences = new Dictionary<Place, int>
            {
                { board.Home, 0 }
            };
            foreach (RestaurantPlace restaurant in board.Restaurants.Values)
            {
                this.Preferences.Add(restaurant, 0);
            }
        }

        public PreferenceHistogram(Dictionary<Place, int> dict) : this()
        {
            if (dict.Values.Sum() == 100)
            {
                this.Preferences = dict;
            }
        }

        public PreferenceHistogram Normalize()
        {
            this.Normalized = true;

            int sum = this.Preferences.Values.Sum();

            if (this.Preferences.Values.Any(x => x < 0))
            {
                throw new NegativePreferenceValueException();
            }

            if (sum == 0)
            {
                throw new ZeroSumException();
            }

            Dictionary<Place, int> dict = new Dictionary<Place, int>();

            foreach (Place place in this.Preferences.Keys)
            {
                dict[place] = (100 * this.Preferences[place] / sum);
            }
            while (dict.Values.Sum() > 100)
            {
                dict[dict.OrderByDescending(x => x.Value).Last().Key]--;
            }
            this.Preferences = dict;

            return this;
        }
    }
}