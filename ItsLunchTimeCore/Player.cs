using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        public abstract DessertCard ChooseDessert(List<DessertCard> cards);

        internal PlayerDescriptor Descriptor
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal void AddMoney(int v)
        {
            this.Descriptor.CurrentCash += v;
        }
    }

    public class PlayerDescriptor
    {
        public Character Character { get; private set; }
        public FoodCard FoodCard { get; private set; }

        private Dictionary<DayOfWeek, Place> _internalVisitedPlaces = new Dictionary<DayOfWeek, Place>();
        public ReadOnlyDictionary<DayOfWeek, Place> VisitedPlaces { get; private set; }

        internal Restaurant UndesiredRestaurant
        {
            get;
        }
        public int CurrentCash { get; internal set; }

        private Player player;

        internal PlayerDescriptor(Player player)
        {
            this.player = player;
            this.VisitedPlaces = new ReadOnlyDictionary<DayOfWeek, Place>(_internalVisitedPlaces);
        }

        internal void VisitPlace(DayOfWeek day, Place place)
        {
            this._internalVisitedPlaces.Add(day, place);
        }

    }
}
