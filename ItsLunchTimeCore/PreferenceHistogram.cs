using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItsLunchTimeCore
{
    public class PreferenceHistogram
    {
        public PlayerBase Player { get; internal set; }
        public Dictionary<Place, int> Preferences { get; private set; }
        public bool Normalized { get; private set; }
        PublicBoard _board;

        private PreferenceHistogram()
        {
            this.Normalized = false;
            this.Preferences = new Dictionary<Place, int>();
        }

        public PreferenceHistogram(PublicBoard board) : this()
        {
            this._board = board;
            this.Preferences = new Dictionary<Place, int>
            {
                { board.Home, 0 }
            };
            foreach (RestaurantPlace restaurant in board.Restaurants.Values)
            {
                this.Preferences.Add(restaurant, 0);
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
                this.Preferences[this.Preferences.Keys.First(x => x is Home)] = 100;
                return this;
            }

            Dictionary<Place, int> dict = new Dictionary<Place, int>();

            foreach (Place place in this.Preferences.Keys)
            {
                dict[place] = (100 * this.Preferences[place] / sum);
            }
            while (dict.Values.Sum() > 100)
            {
                dict[dict.Where(x => x.Value > 0).OrderByDescending(x => x.Value).Last().Key]--;
            }
            this.Preferences = dict;            
            return this;
        }
    }
}