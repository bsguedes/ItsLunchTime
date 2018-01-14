using ItsLunchTimeCore;
using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BotSample
{
    public class SampleBot : PlayerBase
    {
        List<FoodCard> _foodCards;
        List<LoyaltyCard> _loyaltyCards;
        List<PreferenceCard> _prefCards;

        protected override void SignalNewWeek(PublicBoard board)
        {
            this._foodCards = new List<FoodCard>();
            this._loyaltyCards = new List<LoyaltyCard>();
            this._prefCards = new List<PreferenceCard>();
        }

        protected override List<FoodType> AskFavoriteFood(PublicBoard board)
        {
            return new List<FoodType> { this._foodCards.First().Type };
        }

        protected override Dictionary<PlayerBase, int> AskOpinionForDonationTeamObjective(PublicBoard board)
        {
            Dictionary<PlayerBase, int> dict = new Dictionary<PlayerBase, int>();
            board.Players.ForEach(player => dict.Add(player, 7 / board.Players.Count + 1));
            return dict;
        }

        protected override int AskForDonationTeamObjectiveIntent(PublicBoard board, Dictionary<PlayerBase, Dictionary<PlayerBase, int>> opinion)
        {
            return Math.Min(7 / board.Players.Count + 1, board.PlayerCash[this]);
        }

        protected override int AskForDonationTeamObjective(PublicBoard board, Dictionary<PlayerBase, Dictionary<PlayerBase, int>> opinion, Dictionary<PlayerBase, int> intents)
        {
            return Math.Min(7 / board.Players.Count + 1, board.PlayerCash[this]);
        }

        protected override LoyaltyCard AskLoyalty(PublicBoard board)
        {
            return this._loyaltyCards.First();
        }

        protected override PreferenceCard AskPreferences(PublicBoard board)
        {
            return this._prefCards.First();
        }

        protected override List<int> ChooseDessert(PublicBoard board, IEnumerable<DessertCard> cards, int amountToTake)
        {
            return Enumerable.Range(0, amountToTake).ToList();
        }

        protected override TeamBonusCard ChooseOneTeamBonus(PublicBoard board, TeamBonusCard teamBonusCard1, TeamBonusCard teamBonusCard2)
        {
            return teamBonusCard1;
        }

        protected override PreferenceHistogram GetPreferenceHistogram(PublicBoard board, int i, IEnumerable<PreferenceHistogram> last)
        {
            PreferenceHistogram preferenceHistogram = new PreferenceHistogram(board);
            foreach (Restaurant restaurant in Extensions.Restaurants.Scramble())
            {
                RestaurantPlace r = board.Restaurants[restaurant];
                RestaurantDailyModifierCard modifier = r.Modifier;
                if (modifier is OneVictoryPointBonus && modifier.Days.Contains(board.CurrentWeekDay))
                {
                    preferenceHistogram.Preferences[r] += 10;
                }
                if (modifier is OneTeamPointIfMajority && modifier.Days.Contains(board.CurrentWeekDay))
                {
                    preferenceHistogram.Preferences[r] += 20;
                }
                if (modifier is OneDollarDiscount && modifier.Days.Contains(board.CurrentWeekDay))
                {
                    preferenceHistogram.Preferences[r] += 10;
                }
                if (modifier is DoesNotAdvanceTrackPlus2VictoryPoints && modifier.Days.Contains(board.CurrentWeekDay))
                {
                    preferenceHistogram.Preferences[r] += 20;
                }
                if (this._prefCards.First().FirstPreference == restaurant)
                {
                    preferenceHistogram.Preferences[r] += 40;
                }
                if (this._prefCards.First().SecondPreference == restaurant)
                {
                    preferenceHistogram.Preferences[r] += 20;
                }
                if (this._prefCards.First().Undesired == restaurant)
                {
                    preferenceHistogram.Preferences[r] /= 2;
                }
                if (modifier is OneDollarIncrease && modifier.Days.Contains(board.CurrentWeekDay))
                {
                    preferenceHistogram.Preferences[r] /= 2;
                }
                foreach (FoodType food in _foodCards.Select(x => x.Type).Distinct())
                {
                    if (r.Menu.Contains(food))
                    {
                        preferenceHistogram.Preferences[r] += 10;
                    }
                }
                if (this._loyaltyCards.First().Restaurant == restaurant)
                {
                    preferenceHistogram.Preferences[r] += 30;
                }
                if (last != null)
                {
                    foreach (PreferenceHistogram pref in last)
                    {
                        foreach (KeyValuePair<Place, int> histogramEntry in pref.Preferences)
                        {
                            preferenceHistogram.Preferences[histogramEntry.Key] += (histogramEntry.Value * i) / 10;
                        }
                    }
                }                
            }
            foreach(Restaurant restaurant in Extensions.Restaurants.Scramble())
            {
                RestaurantPlace r = board.Restaurants[restaurant];
                RestaurantDailyModifierCard modifier = r.Modifier;
                int cost = r.Cost;
                if (modifier is OneDollarDiscount && modifier.Days.Contains(board.CurrentWeekDay))
                {
                    cost--;
                }
                if (modifier is OneDollarIncrease && modifier.Days.Contains(board.CurrentWeekDay))
                {
                    cost++;
                }
                if (cost > board.PlayerCash[this])
                {
                    preferenceHistogram.Preferences[r] = 0;
                }
                if (modifier is Closed && modifier.Days.Contains(board.CurrentWeekDay))
                {
                    preferenceHistogram.Preferences[r] = 0;
                }
            }
            return preferenceHistogram.Normalize();
        }

        protected override void GiveFoodCard(FoodCard foodCard)
        {
            this._foodCards.Add(foodCard);
        }

        protected override void GiveLoyaltyCard(LoyaltyCard loyaltyCard)
        {
            this._loyaltyCards.Add(loyaltyCard);
        }

        protected override void GivePreferenceCard(PreferenceCard preferenceCard)
        {
            this._prefCards.Add(preferenceCard);
        }


    }
}
