using ItsLunchTimeCore;
using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;

namespace BotAgressive
{
    public class AgressiveBot : PlayerBase
    {
        protected AgressiveBot(Character character) : base(character)
        {
        }

        protected override List<FoodType> AskFavoriteFood(PublicBoard board)
        {
            throw new NotImplementedException();
        }

        protected override int AskForDonationTeamObjective(PublicBoard board, Dictionary<PlayerBase, Dictionary<PlayerBase, int>> opinion, Dictionary<PlayerBase, int> intents)
        {
            throw new NotImplementedException();
        }

        protected override int AskForDonationTeamObjectiveIntent(PublicBoard board, Dictionary<PlayerBase, Dictionary<PlayerBase, int>> opinion)
        {
            throw new NotImplementedException();
        }

        protected override LoyaltyCard AskLoyalty(PublicBoard board)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<PlayerBase, int> AskOpinionForDonationTeamObjective(PublicBoard board)
        {
            throw new NotImplementedException();
        }

        protected override PreferenceCard AskPreferences(PublicBoard board)
        {
            throw new NotImplementedException();
        }

        protected override List<int> ChooseDessert(PublicBoard board, IEnumerable<DessertCard> cards, int amountToTake)
        {
            throw new NotImplementedException();
        }

        protected override TeamBonusCard ChooseOneTeamBonus(PublicBoard board, TeamBonusCard teamBonusCard1, TeamBonusCard teamBonusCard2)
        {
            throw new NotImplementedException();
        }

        protected override Restaurant ChooseRestaurantToAdvanceTrack(PublicBoard publicBoard)
        {
            throw new NotImplementedException();
        }

        protected override PreferenceHistogram GetPreferenceHistogram(PublicBoard board, int iteration, IEnumerable<PreferenceHistogram> last)
        {
            throw new NotImplementedException();
        }

        protected override void GiveFoodCard(FoodCard foodCard)
        {
            throw new NotImplementedException();
        }

        protected override void GiveLoyaltyCard(LoyaltyCard loyaltyCard)
        {
            throw new NotImplementedException();
        }

        protected override void GivePreferenceCard(PreferenceCard preferenceCard)
        {
            throw new NotImplementedException();
        }

        protected override bool ShouldSwitchCashForVPAndTP(PublicBoard board, int cash, int vp, int tp)
        {
            throw new NotImplementedException();
        }

        protected override void SignalNewWeek(PublicBoard board)
        {
            throw new NotImplementedException();
        }
    }
}
