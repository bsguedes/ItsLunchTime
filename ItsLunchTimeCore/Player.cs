using ItsLunchTimeCore.Decks;
using System.Collections.Generic;
using System.Linq;

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
        protected PlayerBase(Character character)
        {
            this.Character = character;
        }

        public PlayerBase Left { get; internal set; }
        public PlayerBase Right { get; internal set; }

        public Character Character { get; private set; }

        internal void SetBoard(PublicBoard board)
        {
            this.Board = board;
        }

        PublicBoard Board { get; set; }

        public int Cash => Board == null ? 0 : Board.PlayerCash[this];
        public int VictoryPoints => Board == null ? 0 : Board.PlayerScores[this];
        public IEnumerable<FoodType> FavoriteFood => Board?.FavoriteFood[this];
        protected IEnumerable<DessertCard> Desserts => Board.GetDessertsFromPlayer(this);
        public Dictionary<PlayerBase, int> DessertCount => Board.Players.Select(x => new { P = x, C = Board.GetDessertsFromPlayer(x).Count() }).ToDictionary(a => a.P, b => b.C);

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
        protected internal abstract Restaurant ChooseRestaurantToAdvanceTrack(PublicBoard publicBoard);
        protected internal abstract bool ShouldSwitchCashForVPAndTP(PublicBoard board, int cash, int vp, int tp);
    }

}
