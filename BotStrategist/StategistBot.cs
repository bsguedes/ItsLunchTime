using ItsLunchTimeCore;
using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BotStrategist
{
    enum MoneyStatus
    {
        Good,
        Medium,
        Bad
    }

    public class StategistBot : PlayerBase
    {
        List<FoodCard> _foodCards;
        List<LoyaltyCard> _loyaltyCards;
        List<PreferenceCard> _prefCards;
        PreferenceCard _chosenPreference;
        FoodType _chosenFood;
        FoodType _chosenFood2;
        Dictionary<Restaurant, int> _attractiveness;

        protected StategistBot(Character character) : base(character)
        {

        }

        void EvaluateRestaurantAttractiveness(PublicBoard board)
        {
            _attractiveness = new Dictionary<Restaurant, int>();
            foreach (Restaurant r in Extensions.Restaurants)
            {
                _attractiveness[r] = 0;
            }
            if (board.CurrentPlayerBonuses.Any(x => x is Eat3TimesChinese))
            {
                foreach (Restaurant r in Extensions.Restaurants)
                {
                    if (board.Restaurants[r].Menu.Contains(FoodType.Chinese))
                    {
                        _attractiveness[r] += 10;
                    }
                }
            }
            if (board.CurrentPlayerBonuses.Any(x => x is NoBrazilianFood))
            {
                foreach (Restaurant r in Extensions.Restaurants)
                {
                    if (board.Restaurants[r].Menu.Contains(FoodType.Brazilian))
                    {
                        _attractiveness[r] -= 20;
                    }
                }
            }
            if (board.CurrentTeamBonus is AllEatBurgerAndPizzaThreeTimes)
            {
                foreach (Restaurant r in Extensions.Restaurants)
                {
                    if (board.Restaurants[r].Menu.Contains(FoodType.Burger) || board.Restaurants[r].Menu.Contains(FoodType.Pizza))
                    {
                        _attractiveness[r] += 10;
                    }
                }
            }
            if (board.CurrentTeamBonus is AllEatBrazilianAndChineseThreeTimes)
            {
                foreach (Restaurant r in Extensions.Restaurants)
                {
                    if (board.Restaurants[r].Menu.Contains(FoodType.Brazilian) || board.Restaurants[r].Menu.Contains(FoodType.Chinese))
                    {
                        _attractiveness[r] += 10;
                    }
                }
            }
            if (Character == Character.Environment)
            {
                foreach (Restaurant r in Extensions.Restaurants)
                {
                    if (board.Restaurants[r].Menu.Contains(FoodType.Vegetarian))
                    {
                        _attractiveness[r] += 30;
                    }
                }
            }
            if (Character == Character.CEO)
            {
                foreach (Restaurant r in Extensions.Restaurants)
                {
                    if (board.Restaurants[r].Cost >= 4)
                    {
                        _attractiveness[r] += 15;
                    }
                }
            }

            switch (MoneyStatus(board))
            {
                case BotStrategist.MoneyStatus.Good:
                    _attractiveness[Restaurant.JoeAndLeos] += 15;
                    _attractiveness[Restaurant.Panorama] += 15;
                    break;
                case BotStrategist.MoneyStatus.Medium:
                    _attractiveness[Restaurant.Silva] += 15;
                    _attractiveness[Restaurant.GustoDiBacio] += 15;
                    break;
                case BotStrategist.MoneyStatus.Bad:
                    _attractiveness[Restaurant.Palatus] += 15;
                    _attractiveness[Restaurant.Russo] += 15;
                    break;
            }
        }

        private MoneyStatus MoneyStatus(PublicBoard board)
        {
            int day_count = board.CurrentDay / 7 * 5 + (board.CurrentDay % 7) - 1;
            if (Cash > 3 * day_count)
            {
                return BotStrategist.MoneyStatus.Good;
            }
            else if (Cash > 3 * day_count - 5)
            {
                return BotStrategist.MoneyStatus.Medium;
            }
            return BotStrategist.MoneyStatus.Bad;
        }

        protected override void SignalNewWeek(PublicBoard board)
        {
            this._foodCards = new List<FoodCard>();
            this._loyaltyCards = new List<LoyaltyCard>();
            this._prefCards = new List<PreferenceCard>();

            EvaluateRestaurantAttractiveness(board);
        }

        protected override List<FoodType> AskFavoriteFood(PublicBoard board)
        {
            if (Character == Character.Environment)
            {
                foreach (FoodCard card in _foodCards)
                {
                    if (card.Type == FoodType.Vegetarian)
                    {
                        _chosenFood = card.Type;
                        return new List<FoodType> { card.Type };
                    }
                }
            }
            if (Character == Character.SalesRep)
            {
                int coverage_a = GetMenuCoverageForCards(board, _foodCards[0], _foodCards[1]);
                int coverage_b = GetMenuCoverageForCards(board, _foodCards[0], _foodCards[2]);
                int coverage_c = GetMenuCoverageForCards(board, _foodCards[1], _foodCards[2]);

                if (coverage_a > coverage_b && coverage_a > coverage_c)
                {
                    _chosenFood = _foodCards[0].Type;
                    _chosenFood2 = _foodCards[1].Type;
                    return new List<FoodType> { _foodCards[0].Type, _foodCards[1].Type };
                }
                if (coverage_b > coverage_a && coverage_b > coverage_c)
                {
                    _chosenFood = _foodCards[0].Type;
                    _chosenFood2 = _foodCards[2].Type;
                    return new List<FoodType> { _foodCards[0].Type, _foodCards[2].Type };
                }
                _chosenFood = _foodCards[1].Type;
                _chosenFood2 = _foodCards[2].Type;
                return new List<FoodType> { _foodCards[1].Type, _foodCards[2].Type };
            }
            foreach (FoodCard card in _foodCards)
            {
                int coverage = 0;
                foreach (Restaurant r in Extensions.Restaurants)
                {
                    if (board.Restaurants[r].Menu.Contains(card.Type))
                    {
                        coverage++;
                    }
                }
                if (coverage == 6)
                {
                    return new List<FoodType> { card.Type };
                }
            }
            return new List<FoodType> { _foodCards[0].Type };
        }

        private int GetMenuCoverageForCards(PublicBoard board, FoodCard foodCard1, FoodCard foodCard2)
        {
            int coverage = 0;

            foreach (Restaurant r in Extensions.Restaurants)
            {
                if (board.Restaurants[r].Menu.Contains(foodCard1.Type) || board.Restaurants[r].Menu.Contains(foodCard2.Type))
                {
                    coverage++;
                }
            }

            return coverage;
        }

        protected override Dictionary<PlayerBase, int> AskOpinionForDonationTeamObjective(PublicBoard board)
        {
            throw new NotImplementedException();
        }

        protected override int AskForDonationTeamObjectiveIntent(PublicBoard board, Dictionary<PlayerBase, Dictionary<PlayerBase, int>> opinion)
        {
            throw new NotImplementedException();
        }

        protected override int AskForDonationTeamObjective(PublicBoard board, Dictionary<PlayerBase, Dictionary<PlayerBase, int>> opinion, Dictionary<PlayerBase, int> intents)
        {
            throw new NotImplementedException();
        }

        protected override LoyaltyCard AskLoyalty(PublicBoard board)
        {
            Dictionary<LoyaltyCard, int> loyalties = new Dictionary<LoyaltyCard, int>();
            foreach (LoyaltyCard card in _loyaltyCards)
            {
                loyalties.Add(card, 0);
                switch (Character)
                {
                    case Character.Environment:
                        if (board.Restaurants[card.Restaurant].Menu.Contains(FoodType.Vegetarian))
                        {
                            loyalties[card] += 30;
                        }
                        break;
                    case Character.Intern:
                        if (card.Type == LoyaltyType.VIP)
                        {
                            loyalties[card] += 30;
                        }
                        break;
                    case Character.Marketing:
                        if (card.Type == LoyaltyType.GOLD)
                        {
                            loyalties[card] += 15;
                        }
                        break;
                }

                foreach (FoodType food in FavoriteFood)
                {
                    if (board.Restaurants[card.Restaurant].Menu.Contains(food))
                    {
                        loyalties[card] += 10;
                    }
                }
                if (card.Restaurant == _chosenPreference.FirstPreference)
                {
                    loyalties[card] += 20;
                }
                if (card.Restaurant == _chosenPreference.SecondPreference)
                {
                    loyalties[card] += 10;
                }

                loyalties[card] += _attractiveness[card.Restaurant];

                if (card.Restaurant == _chosenPreference.Undesired)
                {
                    loyalties[card] = 0;
                }
            }

            return loyalties.OrderByDescending(x => x.Value).First().Key;
        }

        protected override PreferenceCard AskPreferences(PublicBoard board)
        {
            throw new NotImplementedException();
        }

        protected override List<int> ChooseDessert(PublicBoard board, IEnumerable<DessertCard> cards, int amount)
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
            this._foodCards.Add(foodCard);
        }

        protected override void GiveLoyaltyCard(LoyaltyCard loyaltyCard)
        {
            this._loyaltyCards.Add(loyaltyCard);
        }

        protected override void GivePreferenceCard(PreferenceCard preferenceCard)
        {
            this._prefCards.Add(preferenceCard);
        }

        protected override bool ShouldSwitchCashForVPAndTP(PublicBoard board, int cash, int vp, int tp)
        {
            throw new NotImplementedException();
        }


    }
}
