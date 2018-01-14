using ItsLunchTimeCore;
using ItsLunchTimeCore.Decks;
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
            return 7 / board.Players.Count + 1;
        }

        protected override int AskForDonationTeamObjective(PublicBoard board, Dictionary<PlayerBase, Dictionary<PlayerBase, int>> opinion, Dictionary<PlayerBase, int> intents)
        {
            return 7 / board.Players.Count + 1;
        }

        protected override LoyaltyCard AskLoyalty(PublicBoard board)
        {
            return this._loyaltyCards.First();
        }

        protected override PreferenceCard AskPreferences(PublicBoard board)
        {
            return this._prefCards.First();
        }

        protected override DessertCard ChooseDessert(PublicBoard board, List<DessertCard> cards)
        {
            return cards.First();
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
                RestaurantDailyModifierCard modifier = board.Restaurants[restaurant].Modifier;
                if (modifier is OneVictoryPointBonus && modifier.Days.Contains(board.CurrentWeekDay))
                {
                    preferenceHistogram.Preferences[board.Restaurants[restaurant]] += 10;
                }
                if (modifier is OneTeamPointIfMajority && modifier.Days.Contains(board.CurrentWeekDay))
                {
                    preferenceHistogram.Preferences[board.Restaurants[restaurant]] += 20;
                }
                if (modifier is OneDollarDiscount && modifier.Days.Contains(board.CurrentWeekDay))
                {
                    preferenceHistogram.Preferences[board.Restaurants[restaurant]] += 10;
                }
                if (modifier is DoesNotAdvanceTrackPlus2VictoryPoints && modifier.Days.Contains(board.CurrentWeekDay))
                {
                    preferenceHistogram.Preferences[board.Restaurants[restaurant]] += 20;
                }
                if (this._prefCards.First().FirstPreference == restaurant)
                {
                    preferenceHistogram.Preferences[board.Restaurants[restaurant]] += 40;
                }
                if (this._prefCards.First().SecondPreference == restaurant)
                {
                    preferenceHistogram.Preferences[board.Restaurants[restaurant]] += 20;
                }
                if (this._prefCards.First().Undesired == restaurant)
                {
                    preferenceHistogram.Preferences[board.Restaurants[restaurant]] /= 2;
                }
                if (modifier is OneDollarIncrease && modifier.Days.Contains(board.CurrentWeekDay))
                {
                    preferenceHistogram.Preferences[board.Restaurants[restaurant]] /= 2;
                }
                foreach (FoodType food in _foodCards.Select(x => x.Type).Distinct())
                {
                    if (board.Restaurants[restaurant].Menu.Contains(food))
                    {
                        preferenceHistogram.Preferences[board.Restaurants[restaurant]] += 10;
                    }
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
                if (modifier is Closed && modifier.Days.Contains(board.CurrentWeekDay))
                {
                    preferenceHistogram.Preferences[board.Restaurants[restaurant]] = 0;
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
