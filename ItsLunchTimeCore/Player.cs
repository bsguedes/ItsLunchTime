using ItsLunchTimeCore.Decks;
using System.Collections.Generic;

namespace ItsLunchTimeCore
{
    public enum Character
    {
        WarehouseManager,
        ForeignAffairs,
        CEO,
        HR,
        Marketing,
        Intern,
        Finance,
        SalesRep,
        Programmer,
        Environment
    }

    public abstract class PlayerBase
    {
        public PlayerBase Left { get; internal set; }
        public PlayerBase Right { get; internal set; }

        public Character Character { get; private set; }

        public abstract TeamBonusCard ChooseOneTeamBonus(TeamBonusCard teamBonusCard1, TeamBonusCard teamBonusCard2);
        public abstract void GiveFoodCard(FoodCard foodCard);
        public abstract void GiveLoyaltyCard(LoyaltyCard loyaltyCard);
        public abstract void GivePreferenceCard(PreferenceCard preferenceCard);
        public abstract List<FoodType> AskFavoriteFood();
        public abstract PreferenceCard AskPreferences();
        public abstract LoyaltyCard AskLoyalty();
        public abstract Dictionary<PlayerBase, int> AskOpinionForDonationTeamObjective(PublicBoard board);
        public abstract int AskForDonationTeamObjectiveIntent(PublicBoard board, Dictionary<PlayerBase, Dictionary<PlayerBase, int>> opinion);
        public abstract int AskForDonationTeamObjective(PublicBoard board, Dictionary<PlayerBase, Dictionary<PlayerBase, int>> opinion, Dictionary<PlayerBase, int> intents);
        public abstract PreferenceHistogram GetPreferenceHistogram(int i, List<PreferenceHistogram> last);
        public abstract DessertCard ChooseDessert(List<DessertCard> cards);

    }

}
