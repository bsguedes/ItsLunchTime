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

        internal Dictionary<PlayerBase, List<DessertCard>> DessertsPerPlayer { get; set; }
        internal List<PlayerBase> Players { get; }

        Dictionary<PlayerBase, PreferenceCard> _preferenceCards;
        Dictionary<PlayerBase, LoyaltyCard> _loyaltyCards;
        Dictionary<PlayerBase, List<DessertType>> _dessertCards;

        public Game(List<PlayerBase> players, DifficultyLevel difficulty)
        {
            this.Players = players;
            for (int i = 0; i < players.Count; i++)
            {
                this.Players[i].Right = this.Players[(i + 1) % players.Count];
                this.Players[i].Left = this.Players[(i + players.Count - 1) % players.Count];
            }
            this._dessertCards = new Dictionary<PlayerBase, List<DessertType>>();
            players.ForEach(player => _dessertCards.Add(player, new List<DessertType>()));

            PublicBoard = new PublicBoard(players);

            FoodDeck = new FavoriteFoodDeck();
            LoyaltyDeck = new LoyaltyDeck();
            PreferencesDeck = new PreferencesDeck();
            TeamBonusDeck = new TeamBonusDeck();
            PlayerBonusDeck = new PlayerBonusDeck();
            RestaurantDailyModifierDeck = new RestaurantDailyModifierDeck();
            DessertDeck = new DessertDeck(players.Count);
            DessertsPerPlayer = new Dictionary<PlayerBase, List<DessertCard>>();
            Players.ForEach((player) => DessertsPerPlayer.Add(player, new List<DessertCard>()));
            PublicBoard.CurrentDay = 0;

            MAX_WEEKS.Times(turn_index =>
           {
               PublicBoard.CurrentDay++;
               PublicBoard.ClearVisitedPlaces();
               PublicBoard.ClearFavoriteFood();
               _preferenceCards = new Dictionary<PlayerBase, PreferenceCard>();
               _loyaltyCards = new Dictionary<PlayerBase, LoyaltyCard>();

               CreateRestaurantMenu();
               RevealPlayerWeeklyObjectives(turn_index);
               RevealTeamObjective();
               RevealDailyModifiers();
               DealCardsToPlayers();
               ChooseAFavoriteMeal();
               ChooseRestaurantPreferences();

               DAYS_IN_WEEK.Times(day =>
              {
                  PublicBoard.CurrentDay++;
                  DayOfWeek weekday = Extensions.Weekdays[day];
                  ChooseRestaurant(weekday);
                  AdvanceRestaurantTracks(weekday);
                  PayForLunchAndSetMarkers(weekday);
                  ScoreTeamPoints(weekday);
                  ScoreDailyModifiers(weekday);
                  ScoreVPs(weekday);
              });

               PublicBoard.CurrentDay++;
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
                foreach (PlayerBase player in this.Players)
                {
                    if (playerBonus.HasCompletedForPlayer(player, PublicBoard))
                    {
                        this.PublicBoard.AddVictoryPointsToPlayer(playerBonus.Points, player);
                    }
                }
            }
        }

        private void ScoreTeamBonus()
        {
            PublicBoard.TeamScore += PublicBoard.CurrentTeamBonus.HasCompletedTeamBonus(this.PublicBoard) ? 2 : -2;
            PublicBoard.ClearUndesiredRestaurants();
        }

        private void ScorePreferencesAndLoyalty()
        {
            foreach (PlayerBase player in this.Players)
            {
                PublicBoard.SetUndesiredRestaurantOfTheWeek(player, _preferenceCards[player].Undesired);
                foreach (DayOfWeek day in Extensions.Weekdays)
                {
                    if (PublicBoard.RestaurantWithMajority(day) == _preferenceCards[player].FirstPreference && PublicBoard.IsPlayerInMajority(day, player))
                    {
                        PublicBoard.AddVictoryPointsToPlayer(_preferenceCards[player].FirstPreferenceBonus, player);
                    }
                    if (PublicBoard.RestaurantWithMajority(day) == _preferenceCards[player].SecondPreference && PublicBoard.IsPlayerInMajority(day, player))
                    {
                        PublicBoard.AddVictoryPointsToPlayer(_preferenceCards[player].SecondPreferenceBonus, player);
                    }
                    if (PublicBoard.Restaurants[_preferenceCards[player].Undesired].Visitors[day].Contains(player))
                    {
                        switch (_preferenceCards[player].Punishment.Type)
                        {
                            case PunishmentType.Cash:
                                PublicBoard.AddCashToPlayer(_preferenceCards[player].Punishment.Value, player);
                                break;
                            case PunishmentType.VictoryPoints:
                                PublicBoard.AddVictoryPointsToPlayer(_preferenceCards[player].Punishment.Value, player);
                                break;
                        }
                    }
                }
            }
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
                foreach (PlayerBase player in PublicBoard.Restaurants[restaurant].Visitors[day])
                {
                    foreach (FoodType food in PublicBoard.FavoriteFood[player])
                    {
                        if (PublicBoard.Restaurants[restaurant].Menu.Contains(food))
                        {
                            PublicBoard.AddVictoryPointsToPlayer(1, player);
                        }
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
                    foreach (PlayerBase player in PublicBoard.Restaurants[restaurant].Visitors[day])
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
                Place visitedPlace = PublicBoard.VisitedPlaces[player][day];
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
                PublicBoard.AddCashToPlayer(-cost, player);
            });
        }

        private void AdvanceRestaurantTracks(DayOfWeek day)
        {
            this.Players.ForEach(player =>
            {
                Place place = PublicBoard.VisitedPlaces[player][day];
                if (place is RestaurantPlace &&
                    !PublicBoard.RestaurantHasModifierForThisDay<DoesNotAdvanceTrackPlus2VictoryPoints>((place as RestaurantPlace).Identifier, day))
                {
                    if (this.PublicBoard.RestaurantTracks[(place as RestaurantPlace).Identifier].AdvancePlayer(player))
                    {
                        List<DessertCard> cards = new List<DessertCard>();
                        this.PublicBoard.RestaurantTracks[(place as RestaurantPlace).Identifier].CardAmount.Times(() => cards.Add(DessertDeck.Draw()));
                        DessertCard chosenCard = player.ChooseDessert(this.PublicBoard, cards);
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
                    PreferenceHistogram pref = player.GetPreferenceHistogram(this.PublicBoard, i, last == null ? last : last.Where(x => x.Player != player));
                    pref.Player = player;
                    curr.Add(pref);
                });
                last = curr;
            }
            foreach (PreferenceHistogram pref in last)
            {
                Place choice = pref.Preferences.First(x => x.Value == pref.Preferences.Values.Max()).Key;
                if (!(choice is Home) && PublicBoard.RestaurantHasModifierForThisDay<Closed>((choice as RestaurantPlace).Identifier, day))
                {
                    throw new InvalidRestaurantException();
                }
                this.PublicBoard.VisitPlace(pref.Player, day, choice);
            }
        }

        private void ChooseRestaurantPreferences()
        {
            this.Players.ForEach(player => _preferenceCards.Add(player, player.AskPreferences(this.PublicBoard)));
            this.Players.ForEach(player => _loyaltyCards.Add(player, player.AskLoyalty(this.PublicBoard)));
        }

        private void ChooseAFavoriteMeal()
        {
            this.Players.ForEach(player => PublicBoard.SetFavoriteFoodForPlayer(player, player.AskFavoriteFood(this.PublicBoard)));
        }

        private void DealCardsToPlayers()
        {
            FoodDeck.Recreate();
            LoyaltyDeck.Recreate();
            PreferencesDeck.Recreate();

            this.Players.ForEach(player => player.SignalNewWeek(this.PublicBoard));

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
                PublicBoard.CurrentTeamBonus = ceo.ChooseOneTeamBonus(this.PublicBoard, this.TeamBonusDeck.Draw(), this.TeamBonusDeck.Draw());
            }))
            {
                PublicBoard.CurrentTeamBonus = this.TeamBonusDeck.Draw();
            }
        }

        private void CreateRestaurantMenu()
        {
            List<FoodType> pool = new List<FoodType>();
            2.Times(() =>
           {
               foreach (FoodType type in Extensions.FoodTypes)
               {
                   pool.Add(type);
               }
           });
            pool.Shuffle();

            int i = 0;
            foreach (Restaurant restaurant in Extensions.Restaurants)
            {
                RestaurantPlace place = PublicBoard.Restaurants[restaurant];
                place.ClearFood();
                place.AddFoodToMenu(place.BaseFood);
                if (restaurant == Restaurant.Russo)
                {
                    1.Times(() => place.AddFoodToMenu(pool[i++]));
                }
                else if (restaurant == Restaurant.JoeAndLeos)
                {
                    3.Times(() => place.AddFoodToMenu(pool[i++]));
                }
                else
                {
                    2.Times(() => place.AddFoodToMenu(pool[i++]));
                }
            }
        }

        private void RevealPlayerWeeklyObjectives(int turn_index)
        {
            List<PlayerBonusCard> list = new List<PlayerBonusCard>
            {
                this.PlayerBonusDeck.Draw()
            };
            if (turn_index >= 2)
            {
                list.Add(this.PlayerBonusDeck.Draw());
            }
            if (turn_index >= 3)
            {
                list.Add(this.PlayerBonusDeck.Draw());
            }
            PublicBoard.SetNewPlayerBonuses(list);
        }
    }
}
