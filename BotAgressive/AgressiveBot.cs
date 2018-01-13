using ItsLunchTimeCore;
using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;

namespace BotAgressive
{
    public class AgressiveBot : PlayerBase
    {

        public override List<FoodType> AskFavoriteFood()
        {
            throw new NotImplementedException();
        }

        public override int AskForDonationTeamObjective(PublicBoard board, Dictionary<PlayerBase, Dictionary<PlayerBase, int>> opinion, Dictionary<PlayerBase, int> intents)
        {
            throw new NotImplementedException();
        }

        public override int AskForDonationTeamObjectiveIntent(PublicBoard board, Dictionary<PlayerBase, Dictionary<PlayerBase, int>> opinion)
        {
            throw new NotImplementedException();
        }

        public override LoyaltyCard AskLoyalty()
        {
            throw new NotImplementedException();
        }

        public override Dictionary<PlayerBase, int> AskOpinionForDonationTeamObjective(PublicBoard board)
        {
            throw new NotImplementedException();
        }

        public override PreferenceCard AskPreferences()
        {
            throw new NotImplementedException();
        }

        public override DessertCard ChooseDessert(List<DessertCard> cards)
        {
            throw new NotImplementedException();
        }

        public override TeamBonusCard ChooseOneTeamBonus(TeamBonusCard teamBonusCard1, TeamBonusCard teamBonusCard2)
        {
            throw new NotImplementedException();
        }

        public override PreferenceHistogram GetPreferenceHistogram(int i, List<PreferenceHistogram> last)
        {
            throw new NotImplementedException();
        }

        public override void GiveFoodCard(FoodCard foodCard)
        {
            throw new NotImplementedException();
        }

        public override void GiveLoyaltyCard(LoyaltyCard loyaltyCard)
        {
            throw new NotImplementedException();
        }

        public override void GivePreferenceCard(PreferenceCard preferenceCard)
        {
            throw new NotImplementedException();
        }
    }
}
