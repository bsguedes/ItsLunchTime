using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        internal List<Player> Players { get; }

        public Game( List<Player> players, DifficultyLevel difficulty )
        {
            this.Players = players;

            PublicBoard = new PublicBoard();
            
            FoodDeck = new FavoriteFoodDeck();
            LoyaltyDeck = new LoyaltyDeck();
            PreferencesDeck = new PreferencesDeck();
            TeamBonusDeck = new TeamBonusDeck();
            PlayerBonusDeck = new PlayerBonusDeck();
            RestaurantDailyModifierDeck = new RestaurantDailyModifierDeck();
            DessertDeck = new DessertDeck(players.Count);

            MAX_WEEKS.Times(turn_index =>
           {
               RevealPlayerWeeklyObjectives(turn_index);
               RevealTeamObjective();
               RevealDailyModifiers();
               DealCardsToPlayers();
               ChooseAFavoriteMeal();
               ChooseRestaurantPreferences();

               DAYS_IN_WEEK.Times(day =>
              {
                  ChooseRestaurant();
                  RevealChoices();
                  AdvanceRestaurantTracks();
                  PayForLunchAndSetMarkers();
                  ScoreTeamPoints();
                  ScoreDailyModifiers();
                  ScoreVPs();
              });

               ReadjustRestaurantPrices();
               ScorePreferencesAndLoyalty();
               ScoreTeamBonus();
               ScorePlayerBonus();

           });
        }

        private void ScorePlayerBonus()
        {
            foreach(PlayerBonusCard playerBonus in this.PublicBoard.CurrentPlayerBonuses)
            {
                foreach(Player player in this.Players)
                {
                    if (playerBonus.HasCompletedForPlayer(player, PublicBoard))
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
            throw new NotImplementedException();
        }

        private void ScoreVPs()
        {
            throw new NotImplementedException();
        }

        private void ScoreDailyModifiers()
        {
            throw new NotImplementedException();
        }

        private void ScoreTeamPoints()
        {
            throw new NotImplementedException();
        }

        private void PayForLunchAndSetMarkers()
        {
            throw new NotImplementedException();
        }

        private void AdvanceRestaurantTracks()
        {
            throw new NotImplementedException();
        }

        private void RevealChoices()
        {
            throw new NotImplementedException();
        }

        private void ChooseRestaurant()
        {
            
        }

        private void ChooseRestaurantPreferences()
        {
            Dictionary<Player, PreferenceCard> responses = new Dictionary<Player, PreferenceCard>();
            this.Players.ForEach(player => responses[player] = player.AskPreferences());

            Dictionary<Player, LoyaltyCard> responsesLoyalty = new Dictionary<Player, LoyaltyCard>();
            this.Players.ForEach(player => responsesLoyalty[player] = player.AskLoyalty());
        }

        private void ChooseAFavoriteMeal()
        {
            Dictionary<Player, FoodCard> responses = new Dictionary<Player, FoodCard>();
            this.Players.ForEach(player => responses[player] = player.AskFavoriteFood());
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
            foreach(Restaurant restaurant in Extensions.Restaurants)
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
