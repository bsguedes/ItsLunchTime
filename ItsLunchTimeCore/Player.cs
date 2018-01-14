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

        protected internal abstract void SignalNewWeek(PublicBoard board);
        protected internal abstract void GiveFoodCard(FoodCard foodCard);
        protected internal abstract void GiveLoyaltyCard(LoyaltyCard loyaltyCard);
        protected internal abstract void GivePreferenceCard(PreferenceCard preferenceCard);
        protected internal abstract List<FoodType> AskFavoriteFood(PublicBoard board);
        protected internal abstract LoyaltyCard AskLoyalty(PublicBoard board);
        protected internal abstract PreferenceCard AskPreferences(PublicBoard board);
        protected internal abstract PreferenceHistogram GetPreferenceHistogram(PublicBoard board, int iteration, IEnumerable<PreferenceHistogram> last);
        protected internal abstract List<int> ChooseDessert(PublicBoard board, IEnumerable<DessertCard> cards, int amount);
        protected internal abstract Dictionary<PlayerBase, int> AskOpinionForDonationTeamObjective(PublicBoard board);
        protected internal abstract int AskForDonationTeamObjectiveIntent(PublicBoard board, Dictionary<PlayerBase, Dictionary<PlayerBase, int>> opinion);
        protected internal abstract int AskForDonationTeamObjective(PublicBoard board, Dictionary<PlayerBase, Dictionary<PlayerBase, int>> opinion, Dictionary<PlayerBase, int> intents);
        protected internal abstract TeamBonusCard ChooseOneTeamBonus(PublicBoard board, TeamBonusCard teamBonusCard1, TeamBonusCard teamBonusCard2);
    }

}
