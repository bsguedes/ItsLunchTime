using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;
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
        
    }

    public class PlayerDescriptor
    {
        public Character Character { get; private set; }
        public FoodCard FoodCard { get; private set; }

    }
}
