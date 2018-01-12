using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItsLunchTimeCore
{
    public class Game
    {
        public readonly int MAX_WEEKS = 4;
        public readonly int DAYS_IN_WEEK = 5;

        internal PublicBoard PublicBoard { get; }
        internal FavoriteFoodDeck FoodDeck { get; }
        internal LoyaltyDeck LoyaltyDeck { get; }
        internal PreferencesDeck PreferencesDeck { get; }
        internal TeamBonusDeck TeamBonusDeck { get; }
        internal PlayerBonusDeck PlayerBonusDeck { get; }
        internal RestaurantDailyModifierDeck RestaurantDailyModifierDeck { get; }
        internal DessertDeck DessertDeck { get; }

        internal Dictionary<Player, List<DessertCard>> DessertsPerPlayer { get; set; }
        internal List<Player> Players { get; }

        Dictionary<Player, FoodType> _favoriteFood;
        Dictionary<Player, PreferenceCard> _preferenceCards;
        Dictionary<Player, LoyaltyCard> _loyaltyCards;
        Dictionary<Player, List<DessertType>> _dessertCards;

        public Game(List<Player> players, DifficultyLevel difficulty)
        {
            this.Players = players;
            this._dessertCards = new Dictionary<Player, List<DessertType>>();
            players.ForEach(player => _dessertCards.Add(player, new List<DessertType>()));

            PublicBoard = new PublicBoard(players);

            FoodDeck = new FavoriteFoodDeck();
            LoyaltyDeck = new LoyaltyDeck();
            PreferencesDeck = new PreferencesDeck();
            TeamBonusDeck = new TeamBonusDeck();
            PlayerBonusDeck = new PlayerBonusDeck();
            RestaurantDailyModifierDeck = new RestaurantDailyModifierDeck();
            DessertDeck = new DessertDeck(players.Count);
            DessertsPerPlayer = new Dictionary<Player, List<DessertCard>>();
            Players.ForEach((player) => DessertsPerPlayer.Add(player, new List<DessertCard>()));

            MAX_WEEKS.Times(turn_index =>
           {
               _favoriteFood = new Dictionary<Player, FoodType>();
               _preferenceCards = new Dictionary<Player, PreferenceCard>();
               _loyaltyCards = new Dictionary<Player, LoyaltyCard>();

               RevealPlayerWeeklyObjectives(turn_index);
               RevealTeamObjective();
               RevealDailyModifiers();
               DealCardsToPlayers();
               ChooseAFavoriteMeal();
               ChooseRestaurantPreferences();

               DAYS_IN_WEEK.Times(day =>
              {
                  DayOfWeek weekday = Extensions.Weekdays[day];
                  ChooseRestaurant(weekday);
                  AdvanceRestaurantTracks(weekday);
                  PayForLunchAndSetMarkers(weekday);
                  ScoreTeamPoints(weekday);
                  ScoreDailyModifiers(weekday);
                  ScoreVPs(weekday);
              });

               ReadjustRestaurantPrices();
               ScorePreferencesAndLoyalty();
               ScoreTeamBonus();
               ScorePlayerBonus();

           });
        }

        private void ScorePlayerBonus()
        {
            foreach (PlayerBonusCard playerBonus in this.PublicBoard.CurrentPlayerBonuses)
            {
                foreach (Player player in this.Players)
                {
                    if (playerBonus.HasCompletedForPlayer(player.Descriptor, PublicBoard))
                    {

                    }
                }
            }
        }

        private void ScoreTeamBonus()
        {
            throw new NotImplementedException();
        }

        private void ScorePreferencesAndLoyalty()
        {
            throw new NotImplementedException();
        }

        private void ReadjustRestaurantPrices()
        {
            foreach (Restaurant restaurant in Extensions.Restaurants)
            {
                PublicBoard.Restaurants[restaurant].AdjustPrice(this.Players.Count);
            }
        }

        private void ScoreVPs(DayOfWeek day)
        {
            foreach (Restaurant restaurant in Extensions.Restaurants)
            {
                foreach (PlayerDescriptor player in PublicBoard.Restaurants[restaurant].Visitors[day])
                {
                    if (PublicBoard.Restaurants[restaurant].Menu.Contains(player.FoodCard))
                    {
                        PublicBoard.AddVictoryPointsToPlayer(1, player);
                    }
                }
            }
        }

        private void ScoreDailyModifiers(DayOfWeek day)
        {
            foreach (Restaurant restaurant in Extensions.Restaurants)
            {
                if (PublicBoard.RestaurantHasModifierForThisDay<OneTeamPointIfMajority>(restaurant, day) && PublicBoard.HasMajority(day))
                {
                    PublicBoard.TeamScore += 1;
                }
                if (PublicBoard.RestaurantHasModifierForThisDay<OneVictoryPointBonus>(restaurant, day))
                {
                    foreach (PlayerDescriptor player in PublicBoard.Restaurants[restaurant].Visitors[day])
                    {
                        PublicBoard.AddVictoryPointsToPlayer(1, player);
                    }
                }

            }
        }

        private void ScoreTeamPoints(DayOfWeek day)
        {
            int net_score = 0;
            net_score += 1 * (PublicBoard.HasMajority(day) ? 1 : -1);
            if (PublicBoard.HasUnanimity(day))
            {
                net_score++;
            }
            if (PublicBoard.HasSomeoneAlone(day))
            {
                net_score--;
            }
            PublicBoard.TeamScore += net_score;
            if (PublicBoard.TeamScore < 0)
            {
                PublicBoard.TeamScore = 0;
            }
        }

        private void PayForLunchAndSetMarkers(DayOfWeek day)
        {
            Players.ForEach(player =>
            {
                int cost = 0;
                Place visitedPlace = player.Descriptor.VisitedPlaces[day];
                if (visitedPlace is RestaurantPlace)
                {
                    RestaurantPlace restaurant = visitedPlace as RestaurantPlace;
                    if (PublicBoard.RestaurantHasModifierForThisDay<OneDollarIncrease>(restaurant.Identifier, day))
                    {
                        cost++;
                    }
                    if (PublicBoard.RestaurantHasModifierForThisDay<OneDollarDiscount>(restaurant.Identifier, day))
                    {
                        cost--;
                    }
                    cost += restaurant.Cost;
                }
                player.AddMoney(-cost);
            });
        }

        private void AdvanceRestaurantTracks(DayOfWeek day)
        {
            this.Players.ForEach(player =>
            {
                Place place = player.Descriptor.VisitedPlaces[day];
                if (place is RestaurantPlace &&
                    !PublicBoard.RestaurantHasModifierForThisDay<DoesNotAdvanceTrackPlus2VictoryPoints>((place as RestaurantPlace).Identifier, day))
                {
                    if (this.PublicBoard.RestaurantTracks[(place as RestaurantPlace).Identifier].AdvancePlayer(player.Descriptor))
                    {
                        List<DessertCard> cards = new List<DessertCard>();
                        this.PublicBoard.RestaurantTracks[(place as RestaurantPlace).Identifier].CardAmount.Times(() => cards.Add(DessertDeck.Draw()));
                        DessertCard chosenCard = player.ChooseDessert(cards);
                        DessertsPerPlayer[player].Add(chosenCard);
                    }
                }
            });
        }

        private void ChooseRestaurant(DayOfWeek day)
        {
            List<PreferenceHistogram> last = null;
            for (int i = 0; i < 3; i++)
            {
                List<PreferenceHistogram> curr = new List<PreferenceHistogram>();
                this.Players.ForEach(player =>
                {
                    PreferenceHistogram pref = player.GetPreferenceHistogram(i, last);
                    pref.Player = player.Descriptor;
                    curr.Add(pref);
                });
                last = curr;
            }
            foreach (PreferenceHistogram pref in last)
            {
                Place choice = pref.Preferences.FirstOrDefault(x => x.Value == pref.Preferences.Values.Max()).Key;
                if (choice is Home)
                {
                    this.PublicBoard.Home.VisitPlace(pref.Player, day);
                }
                else
                {
                    if (PublicBoard.RestaurantHasModifierForThisDay<Closed>((choice as RestaurantPlace).Identifier, day))
                    {
                        throw new InvalidRestaurantException();
                    }
                    this.PublicBoard.Restaurants[(choice as RestaurantPlace).Identifier].VisitPlace(pref.Player, day);
                }
            }
        }

        private void ChooseRestaurantPreferences()
        {
            this.Players.ForEach(player => _preferenceCards.Add(player, player.AskPreferences()));
            this.Players.ForEach(player => _loyaltyCards.Add(player, player.AskLoyalty()));
        }

        private void ChooseAFavoriteMeal()
        {
            this.Players.ForEach(player => _favoriteFood.Add(player, player.AskFavoriteFood().Type));
        }

        private void DealCardsToPlayers()
        {
            FoodDeck.Recreate();
            LoyaltyDeck.Recreate();
            PreferencesDeck.Recreate();

            3.Times(() => this.Players.ForEach(player => player.GiveFoodCard(this.FoodDeck.Draw())));
            3.Times(() => this.Players.ForEach(player => player.GiveLoyaltyCard(this.LoyaltyDeck.Draw())));
            4.Times(() => this.Players.ForEach(player => player.GivePreferenceCard(this.PreferencesDeck.Draw())));
        }

        private void RevealDailyModifiers()
        {
            foreach (Restaurant restaurant in Extensions.Restaurants)
            {
                PublicBoard.Restaurants[restaurant].Modifier = this.RestaurantDailyModifierDeck.Draw();
            }
        }

        private void RevealTeamObjective()
        {
            if (!this.Players.ActionForCharacter(Character.CEO, ceo =>
            {
                PublicBoard.CurrentTeamBonus = ceo.ChooseOneTeamBonus(this.TeamBonusDeck.Draw(), this.TeamBonusDeck.Draw());
            }))
            {
                PublicBoard.CurrentTeamBonus = this.TeamBonusDeck.Draw();
            }
        }

        private void RevealPlayerWeeklyObjectives(int turn_index)
        {
            List<PlayerBonusCard> list = new List<PlayerBonusCard>
            {
                this.PlayerBonusDeck.Draw()
            };
            if (turn_index >= 3)
            {
                list.Add(this.PlayerBonusDeck.Draw());
            }
            if (turn_index >= 4)
            {
                list.Add(this.PlayerBonusDeck.Draw());
            }
            PublicBoard.SetNewPlayerBonuses(list);
        }
    }
}
