using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        internal RestaurantDailyBonusDeck RestaurantDailyBonusDeck { get; }
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
            RestaurantDailyBonusDeck = new RestaurantDailyBonusDeck();
            DessertDeck = new DessertDeck(players.Count);

            MAX_WEEKS.Each(turn_index =>
           {
               RevealPlayerWeeklyObjectives(turn_index);
               RevealTeamObjective();
               RevealDailyModifiers();
               DealCardsToPlayers();
               ChooseAFavoriteMeal();
               ChooseRestaurantPreferences();

               DAYS_IN_WEEK.Each(day =>
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        private void ChooseRestaurantPreferences()
        {
            throw new NotImplementedException();
        }

        private void ChooseAFavoriteMeal()
        {
            throw new NotImplementedException();
        }

        private void DealCardsToPlayers()
        {
            throw new NotImplementedException();
        }

        private void RevealDailyModifiers()
        {
            throw new NotImplementedException();
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
