using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public abstract class Player
    {
        public Character Character { get; private set; }
        public abstract TeamBonusCard ChooseOneTeamBonus(TeamBonusCard teamBonusCard1, TeamBonusCard teamBonusCard2);
        public abstract void GiveFoodCard(FoodCard foodCard);
        public abstract void GiveLoyaltyCard(LoyaltyCard loyaltyCard);
        public abstract void GivePreferenceCard(PreferenceCard preferenceCard);
        public abstract FoodCard AskFavoriteFood();
        public abstract PreferenceCard AskPreferences();
        public abstract LoyaltyCard AskLoyalty();
        public abstract Dictionary<PlayerDescriptor, int> AskOpinionForDonationTeamObjective(PublicBoard board);
        public abstract int AskForDonationTeamObjectiveIntent(PublicBoard board, Dictionary<PlayerDescriptor, Dictionary<PlayerDescriptor, int>> opinion);
        public abstract int AskForDonationTeamObjective(PublicBoard board, Dictionary<PlayerDescriptor, Dictionary<PlayerDescriptor, int>> opinion, Dictionary<PlayerDescriptor, int> intents);
        public abstract PreferenceHistogram GetPreferenceHistogram(int i, List<PreferenceHistogram> last);

        internal PlayerDescriptor Descriptor
        {
            get
            {
                throw new NotImplementedException();
            }
        }

    }

    public class PlayerDescriptor
    {
        public Character Character { get; private set; }
        public FoodCard FoodCard { get; private set; }        
        public ReadOnlyDictionary<DayOfWeek, Place> VisitedPlaces { get; private set; }
        private Dictionary<DayOfWeek, Place> _internalVisitedPlaces = new Dictionary<DayOfWeek, Place>();
        internal Restaurant UndesiredRestaurant { get; private set; }
        public int CurrentCash { get; private set; }

        public PlayerDescriptor()
        {
            this.VisitedPlaces = new ReadOnlyDictionary<DayOfWeek, Place>(_internalVisitedPlaces);
        }

        internal void VisitPlace(DayOfWeek day, Place place)
        {
            this._internalVisitedPlaces.Add(day, place);
        }

    }
}
