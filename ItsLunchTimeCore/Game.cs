using ItsLunchTimeCore.Decks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItsLunchTimeCore
{
    public class Game
    {
        public static readonly int MAX_WEEKS = 4;
        public static readonly int DAYS_IN_WEEK = 5;
        public static readonly int DESSERT_BUFFET_SIZE = 5;
        public static readonly int MAX_TRACK_LEVEL = 7;
        public static readonly int MAX_TEAM_SCORE = 20;
        public static readonly int STARTING_SCORE = 20;

        public PublicBoard PublicBoard { get; }

        internal FavoriteFoodDeck FoodDeck { get; }
        internal LoyaltyDeck LoyaltyDeck { get; }
        internal PreferencesDeck PreferencesDeck { get; }
        internal TeamBonusDeck TeamBonusDeck { get; }
        internal PlayerBonusDeck PlayerBonusDeck { get; }
        internal RestaurantDailyModifierDeck RestaurantDailyModifierDeck { get; }
        internal DessertDeck DessertDeck { get; }
        internal DessertBuffet DessertBuffet { get; }
        private int FirstPlayerIndex = 0;
        private List<PlayerBase> _players;
        internal IEnumerable<PlayerBase> Players
        {
            get
            {
                foreach (PlayerBase player in _players.Skip(FirstPlayerIndex).Concat(_players.Take(FirstPlayerIndex)))
                {
                    yield return player;
                }
            }
        }

        public bool Win => this.PublicBoard.CurrentDay == 28 && this.PublicBoard.TeamScore >= 15;

        Dictionary<PlayerBase, PreferenceCard> _preferenceCards;
        Dictionary<PlayerBase, LoyaltyCard> _loyaltyCards;
        Dictionary<PlayerBase, List<DessertCard>> _dessertCards;

        public Game(List<PlayerBase> players, DifficultyLevel difficulty)
        {
            this._players = players;
            for (int i = 0; i < players.Count; i++)
            {
                this._players[i].SetBoard(PublicBoard);
                this._players[i].Right = this._players[(i + 1) % players.Count];
                this._players[i].Left = this._players[(i + players.Count - 1) % players.Count];
            }
            this._dessertCards = new Dictionary<PlayerBase, List<DessertCard>>();
            players.ForEach(player => _dessertCards.Add(player, new List<DessertCard>()));

            PublicBoard = new PublicBoard(players, difficulty) { DessertsHandler = (player) => _dessertCards[player] };
            FoodDeck = new FavoriteFoodDeck();
            LoyaltyDeck = new LoyaltyDeck();
            PreferencesDeck = new PreferencesDeck();
            TeamBonusDeck = new TeamBonusDeck();
            PlayerBonusDeck = new PlayerBonusDeck();
            RestaurantDailyModifierDeck = new RestaurantDailyModifierDeck();
            DessertDeck = new DessertDeck(players.Count + 3);
            DessertBuffet = new DessertBuffet(DESSERT_BUFFET_SIZE, DessertDeck);

            PublicBoard.CurrentDay = 0;

            for (int i = 0; i < MAX_WEEKS; i++)
            {

                PublicBoard.CurrentDay++;
                PublicBoard.ClearVisitedPlaces();
                PublicBoard.ClearFavoriteFood();
                _preferenceCards = new Dictionary<PlayerBase, PreferenceCard>();
                _loyaltyCards = new Dictionary<PlayerBase, LoyaltyCard>();

                CreateRestaurantMenu();
                RevealPlayerWeeklyObjectives(i);
                RevealTeamObjective();
                RevealDailyModifiers();
                DealCardsToPlayers();
                ChooseAFavoriteMeal();
                ChooseRestaurantPreferences();

                for (int day = 0; day < DAYS_IN_WEEK; day++)
                {
                    PublicBoard.CurrentDay++;
                    DayOfWeek weekday = Extensions.Weekdays[day];

                    ChooseRestaurant(weekday);
                    AdvanceRestaurantTracks(weekday);
                    PayForLunchAndSetMarkers(weekday);
                    ScoreTeamPoints(weekday);
                    ScoreDailyModifiers(weekday);
                    ScoreVPs(weekday);
                    FirstPlayerIndex = (FirstPlayerIndex + 1) % this.Players.Count();
                }

                PublicBoard.CurrentDay++;
                ReadjustRestaurantPrices();
                ScorePreferencesAndLoyalty();
                ScoreTeamBonus();
                ScorePlayerBonus();
            }

            CalculateFinalScore();
        }

        private void CalculateFinalScore()
        {
            EvaluateDesserts();
            EvaluateRestaurantTracks();
            ConvertCashToVP();


            //Console.WriteLine("Scores:");
            //Console.WriteLine(string.Join(" ", this.PublicBoard.PlayerScores.Select(x => x.Value)));
            //Console.WriteLine("Cash:");
            //Console.WriteLine(string.Join(" ", this.PublicBoard.PlayerCash.Select(x => x.Value)));
            //Console.WriteLine("Team score: {0}", this.PublicBoard.TeamScore);
        }

        private void EvaluateRestaurantTracks()
        {
            foreach (Restaurant r in Extensions.Restaurants)
            {
                RestaurantTrack track = PublicBoard.RestaurantTracks[r];
                Dictionary<PlayerBase, int> ordered = track.PlayerScores.OrderByDescending(x => x.Value).ToDictionary(a => a.Key, b => b.Value);

                int rewardedPlayers = 0;
                var tiedAtFirst = ordered.Where(x => x.Value == ordered.First().Value);
                rewardedPlayers += tiedAtFirst.Count();
                int points = 6 + rewardedPlayers > 1 ? 4 : 0 + rewardedPlayers > 2 ? 2 : 0;
                foreach (var a in tiedAtFirst)
                {
                    PublicBoard.AddVictoryPointsToPlayer(points / rewardedPlayers, a.Key, VictoryPointsSource.Tracks);
                }
                if (rewardedPlayers < PublicBoard.Players.Count)
                {
                    var tiedAtSecond = ordered.Where(x => x.Value == ordered.ElementAt(rewardedPlayers).Value);
                    points = rewardedPlayers == 1 ? (4 + tiedAtSecond.Count() > 1 ? 2 : 0) : rewardedPlayers == 2 ? 2 : 0;
                    foreach (var b in tiedAtSecond)
                    {
                        PublicBoard.AddVictoryPointsToPlayer(points / tiedAtSecond.Count(), b.Key, VictoryPointsSource.Tracks);
                    }
                    rewardedPlayers += tiedAtSecond.Count();
                    if (rewardedPlayers == 2)
                    {
                        var tiedAtThird = ordered.Where(x => x.Value == ordered.ElementAt(2).Value);
                        foreach (var c in tiedAtThird)
                        {
                            PublicBoard.AddVictoryPointsToPlayer(points / tiedAtThird.Count(), c.Key, VictoryPointsSource.Tracks);
                        }
                    }
                }
            }
        }

        private void ConvertCashToVP()
        {
            foreach (PlayerBase player in Players)
            {
                if (player.Character == Character.Marketing)
                {
                    PublicBoard.AddVictoryPointsToPlayer(PublicBoard.PlayerCash[player], player, VictoryPointsSource.Money);
                }
                else
                {
                    PublicBoard.AddVictoryPointsToPlayer(PublicBoard.PlayerCash[player] / 2, player, VictoryPointsSource.Money);
                }
            }
        }

        private void EvaluateDesserts()
        {
            Dictionary<PlayerBase, int> _iceCreamDict = new Dictionary<PlayerBase, int>();
            foreach (PlayerBase player in Players)
            {
                int coffee_count = Math.Min(4, _dessertCards[player].Where(x => x.Type == DessertType.Coffee).Count());
                PublicBoard.AddVictoryPointsToPlayer(coffee_count.Terminal(), player, VictoryPointsSource.Dessert);

                int pudding_count = _dessertCards[player].Where(x => x.Type == DessertType.Pudding).Count();
                PublicBoard.AddVictoryPointsToPlayer(pudding_count * PublicBoard.TeamBonusDoneCount, player, VictoryPointsSource.Dessert);

                int other_cake_count = _dessertCards.Where(x => x.Key != player).Select(x => x.Value.Where(y => y.Type == DessertType.Cake).Count()).Sum();
                int cake_count = _dessertCards[player].Where(x => x.Type == DessertType.Cake).Count();
                PublicBoard.AddVictoryPointsToPlayer(cake_count * other_cake_count, player, VictoryPointsSource.Dessert);

                int cream_count = _dessertCards[player].Where(x => x.Type == DessertType.Cream).Count();
                int left_sago_count = _dessertCards[player.Left].Where(x => x.Type == DessertType.Sago).Count();
                int right_sago_count = _dessertCards[player.Right].Where(x => x.Type == DessertType.Sago).Count();
                PublicBoard.AddVictoryPointsToPlayer(cream_count * (1 + 2 * left_sago_count + 2 * right_sago_count), player, VictoryPointsSource.Dessert);

                int sago_count = _dessertCards[player].Where(x => x.Type == DessertType.Sago).Count();
                if (sago_count == 0)
                {
                    PublicBoard.AddVictoryPointsToPlayer(-3, player, VictoryPointsSource.Dessert);
                }
                else if (sago_count >= 2)
                {
                    PublicBoard.AddVictoryPointsToPlayer(-1 * sago_count, player, VictoryPointsSource.Dessert);
                }

                _iceCreamDict.Add(player, _dessertCards[player].Where(x => x.Type == DessertType.IceCream).Count());

                if (player.Character == Character.Intern)
                {
                    PublicBoard.AddVictoryPointsToPlayer(2 * _dessertCards[player].Count, player, VictoryPointsSource.Dessert);
                }
            }
            PlayerBase[] ordered = _iceCreamDict.OrderByDescending(x => x.Value).Select(x => x.Key).ToArray();
            IEnumerable<PlayerBase> _tiedAsFirst = ordered.Where(y => _iceCreamDict[y] == _iceCreamDict[ordered[0]]);
            if (_tiedAsFirst.Count() == 1)
            {
                PublicBoard.AddVictoryPointsToPlayer(10, ordered[0], VictoryPointsSource.Dessert);
                IEnumerable<PlayerBase> _tiedAsSecond = ordered.Where(y => _iceCreamDict[y] == _iceCreamDict[ordered[1]]);
                foreach (PlayerBase tiedPlayer in _tiedAsSecond)
                {
                    PublicBoard.AddVictoryPointsToPlayer(4 / _tiedAsSecond.Count(), tiedPlayer, VictoryPointsSource.Dessert);
                }
            }
            else
            {
                foreach (PlayerBase tiedPlayer in _tiedAsFirst)
                {
                    PublicBoard.AddVictoryPointsToPlayer(14 / _tiedAsFirst.Count(), tiedPlayer, VictoryPointsSource.Dessert);
                }
            }
        }

        private void ScorePlayerBonus()
        {
            foreach (PlayerBonusCard playerBonus in this.PublicBoard.CurrentPlayerBonuses)
            {
                foreach (PlayerBase player in this.Players)
                {
                    if (playerBonus.HasCompletedForPlayer(player, PublicBoard))
                    {
                        this.PublicBoard.AddVictoryPointsToPlayer(playerBonus.Points, player, VictoryPointsSource.PlayerBonus);
                        if (player.Character == Character.WarehouseManager)
                        {
                            Restaurant restaurant = player.ChooseRestaurantToAdvanceTrack(PublicBoard);
                            if (this.PublicBoard.RestaurantTracks[restaurant].AdvancePlayer(player))
                            {
                                List<DessertCard> cards = new List<DessertCard>();
                                List<int> chosenCardIndex = player.ChooseDessert(this.PublicBoard, this.DessertBuffet.TakeChoices(this.PublicBoard.RestaurantTracks[restaurant].CardAmount), 1);
                                List<DessertCard> chosenCards = this.DessertBuffet.RemoveDessertAtIndexes(chosenCardIndex);
                                _dessertCards[player].AddRange(chosenCards);
                            }
                        }
                        if (player.Character == Character.Finance)
                        {
                            PublicBoard.AddCashToPlayer(4, player);
                        }
                    }
                }
            }

            foreach (PlayerBase player in this.Players)
            {
                if (player.Character == Character.ForeignAffairs)
                {
                    int count = PublicBoard.VisitedPlaces[player].Select(x => x.Value).Where(x => x is RestaurantPlace).Distinct().Count();
                    PublicBoard.AddVictoryPointsToPlayer(count.Fibonacci(), player, VictoryPointsSource.Character);
                }
            }
        }

        private void ScoreTeamBonus()
        {
            if (PublicBoard.CurrentTeamBonus.HasCompletedTeamBonus(this.PublicBoard))
            {
                PublicBoard.AddTeamScore(2);
                PublicBoard.TeamBonusDoneCount++;
                Players.ActionForCharacter(Character.HR, (player) => PublicBoard.AddVictoryPointsToPlayer(2, player, VictoryPointsSource.Character));
            }
            else
            {
                PublicBoard.AddTeamScore(-2);
            }
            PublicBoard.ClearUndesiredRestaurants();
        }

        private void ScorePreferencesAndLoyalty()
        {
            foreach (PlayerBase player in this.Players)
            {
                PublicBoard.SetUndesiredRestaurantOfTheWeek(player, _preferenceCards[player].Undesired);
                LoyaltyCard loyalty = _loyaltyCards[player];
                int visitsToLoyalty = 0;
                foreach (DayOfWeek day in Extensions.Weekdays)
                {
                    if (PublicBoard.VisitedPlaces[player][day] is RestaurantPlace && (PublicBoard.VisitedPlaces[player][day] as RestaurantPlace).Identifier == loyalty.Restaurant)
                    {
                        visitsToLoyalty++;
                    }
                    if (PublicBoard.RestaurantWithMajority(day) == _preferenceCards[player].FirstPreference && PublicBoard.IsPlayerInMajority(day, player))
                    {
                        PublicBoard.AddVictoryPointsToPlayer(_preferenceCards[player].FirstPreferenceBonus, player, VictoryPointsSource.PreferenceCard);
                    }
                    if (PublicBoard.RestaurantWithMajority(day) == _preferenceCards[player].SecondPreference && PublicBoard.IsPlayerInMajority(day, player))
                    {
                        PublicBoard.AddVictoryPointsToPlayer(_preferenceCards[player].SecondPreferenceBonus, player, VictoryPointsSource.PreferenceCard);
                    }
                    if (PublicBoard.Restaurants[_preferenceCards[player].Undesired].Visitors[day].Contains(player))
                    {
                        ApplyPunishment(_preferenceCards[player].Punishment, player);
                    }
                    if (PublicBoard.IsPlayerAlone(day, player.Left))
                    {
                        ApplyPunishment(_preferenceCards[player].SideBan, player.Left);
                    }
                    if (PublicBoard.IsPlayerAlone(day, player.Right))
                    {
                        ApplyPunishment(_preferenceCards[player].SideBan, player.Right);
                    }
                }

                PublicBoard.AddVictoryPointsToPlayer(loyalty.VictoryPoints[visitsToLoyalty], player, VictoryPointsSource.LoyaltyCard);

                switch (_loyaltyCards[player].Type)
                {
                    case LoyaltyType.VIP:
                        LoyaltyCardVIP cardVIP = _loyaltyCards[player] as LoyaltyCardVIP;
                        if (visitsToLoyalty >= 2)
                        {
                            List<int> desserts = player.ChooseDessert(PublicBoard, DessertBuffet.TakeChoices(cardVIP.DessertOptions), cardVIP.DessertTakeCount);
                            List<DessertCard> chosenCards = this.DessertBuffet.RemoveDessertAtIndexes(desserts);
                            _dessertCards[player].AddRange(chosenCards);
                        }
                        break;
                    case LoyaltyType.GOLD:
                        LoyaltyCardGOLD cardGOLD = _loyaltyCards[player] as LoyaltyCardGOLD;
                        if (visitsToLoyalty >= 3)
                        {
                            PublicBoard.AddCashToPlayer(cardGOLD.ExtraCash, player);
                        }
                        break;
                }
            }
        }

        private void ApplyPunishment(Punishment punishment, PlayerBase target)
        {
            switch (punishment.Type)
            {
                case PunishmentType.Cash:
                    if (PublicBoard.PlayerCash[target] < Math.Abs(punishment.Value))
                    {
                        PublicBoard.AddCashToPlayer(-PublicBoard.PlayerCash[target], target);
                    }
                    else
                    {
                        PublicBoard.AddCashToPlayer(punishment.Value, target);
                    }
                    break;
                case PunishmentType.VictoryPoints:
                    PublicBoard.AddVictoryPointsToPlayer(punishment.Value, target, VictoryPointsSource.Punishment);
                    break;
            }

        }

        private void ReadjustRestaurantPrices()
        {
            foreach (Restaurant restaurant in Extensions.Restaurants)
            {
                PublicBoard.Restaurants[restaurant].AdjustPrice(this.Players.Count());
            }
        }

        private void ScoreVPs(DayOfWeek day)
        {
            foreach (Restaurant restaurant in Extensions.Restaurants)
            {
                foreach (PlayerBase player in PublicBoard.Restaurants[restaurant].Visitors[day])
                {
                    foreach (FoodType food in PublicBoard.FavoriteFood[player])
                    {
                        if (PublicBoard.Restaurants[restaurant].Menu.Contains(food))
                        {
                            PublicBoard.AddVictoryPointsToPlayer(1, player, VictoryPointsSource.Food);
                        }
                    }
                }
            }

            Players.ActionForCharacter(Character.HR, player =>
            {
                if (PublicBoard.IsPlayerInMajority(day, player))
                {
                    if (player.ShouldSwitchCashForVPAndTP(PublicBoard, -1, 1, 0))
                    {
                        PublicBoard.AddCashToPlayer(-1, player);
                        PublicBoard.AddVictoryPointsToPlayer(1, player, VictoryPointsSource.Character);
                    }
                }
            });
            Players.ActionForCharacter(Character.Marketing, player =>
            {
                if (!PublicBoard.IsPlayerInMajority(day, player))
                {
                    if (player.ShouldSwitchCashForVPAndTP(PublicBoard, -3, 2, 1))
                    {
                        PublicBoard.AddCashToPlayer(-3, player);
                        PublicBoard.AddVictoryPointsToPlayer(2, player, VictoryPointsSource.Character);
                        PublicBoard.AddTeamScore(1);
                    }
                }
            });
            Players.ActionForCharacter(Character.Programmer, player =>
            {
                if (!PublicBoard.IsPlayerInMajority(day, player) && !PublicBoard.IsPlayerAlone(day, player))
                {
                    PublicBoard.AddVictoryPointsToPlayer(3, player, VictoryPointsSource.Character);
                }
            });
            Players.ActionForCharacter(Character.Environment, player =>
            {
                if (PublicBoard.VisitedPlaces[player][day].Menu.Contains(FoodType.Vegetarian))
                {
                    PublicBoard.AddVictoryPointsToPlayer(2, player, VictoryPointsSource.Character);
                }
            });
        }

        private void ScoreDailyModifiers(DayOfWeek day)
        {
            foreach (Restaurant restaurant in Extensions.Restaurants)
            {
                if (PublicBoard.RestaurantHasModifierForThisDay<OneTeamPointIfMajority>(restaurant, day) && PublicBoard.HasMajority(day))
                {
                    PublicBoard.AddTeamScore(1);
                }
                if (PublicBoard.RestaurantHasModifierForThisDay<OneVictoryPointBonus>(restaurant, day))
                {
                    foreach (PlayerBase player in PublicBoard.Restaurants[restaurant].Visitors[day])
                    {
                        PublicBoard.AddVictoryPointsToPlayer(1, player, VictoryPointsSource.DailyModifier);
                    }
                }

            }
        }

        private void ScoreTeamPoints(DayOfWeek day)
        {
            int net_score = 0;
            net_score += 1 * (PublicBoard.HasMajority(day) ? 1 : -1);
            if (PublicBoard.HasUnanimity(day))
            {
                net_score++;
            }
            if (PublicBoard.HasSomeoneAlone(day))
            {
                net_score--;
            }
            PublicBoard.AddTeamScore(net_score);
        }

        private void PayForLunchAndSetMarkers(DayOfWeek day)
        {
            foreach (PlayerBase player in Players)
            {
                int cost = 0;
                Place visitedPlace = PublicBoard.VisitedPlaces[player][day];
                if (visitedPlace is RestaurantPlace)
                {
                    RestaurantPlace restaurant = visitedPlace as RestaurantPlace;
                    if (PublicBoard.RestaurantHasModifierForThisDay<OneDollarIncrease>(restaurant.Identifier, day))
                    {
                        cost++;
                    }
                    if (PublicBoard.RestaurantHasModifierForThisDay<OneDollarDiscount>(restaurant.Identifier, day))
                    {
                        cost--;
                    }
                    cost += restaurant.Cost;
                    if (cost >= 4 && player.Character == Character.CEO)
                    {
                        cost--;
                    }
                }
                PublicBoard.AddCashToPlayer(-cost, player);
            };
        }

        private void AdvanceRestaurantTracks(DayOfWeek day)
        {
            foreach (PlayerBase player in this.Players)
            {
                Place place = PublicBoard.VisitedPlaces[player][day];
                if (place is RestaurantPlace &&
                    !PublicBoard.RestaurantHasModifierForThisDay<DoesNotAdvanceTrackPlus2VictoryPoints>((place as RestaurantPlace).Identifier, day))
                {
                    if (this.PublicBoard.RestaurantTracks[(place as RestaurantPlace).Identifier].AdvancePlayer(player))
                    {
                        List<DessertCard> cards = new List<DessertCard>();
                        List<int> chosenCardIndex = player.ChooseDessert(this.PublicBoard, this.DessertBuffet.TakeChoices(this.PublicBoard.RestaurantTracks[(place as RestaurantPlace).Identifier].CardAmount), 1);
                        List<DessertCard> chosenCards = this.DessertBuffet.RemoveDessertAtIndexes(chosenCardIndex);
                        _dessertCards[player].AddRange(chosenCards);
                    }
                }
                else if (place is RestaurantPlace && PublicBoard.RestaurantHasModifierForThisDay<DoesNotAdvanceTrackPlus2VictoryPoints>((place as RestaurantPlace).Identifier, day))
                {
                    PublicBoard.AddVictoryPointsToPlayer(2, player, VictoryPointsSource.DailyModifier);
                }
            }
        }

        private void ChooseRestaurant(DayOfWeek day)
        {
            List<PreferenceHistogram> last = null;
            for (int i = 0; i < 3; i++)
            {
                List<PreferenceHistogram> curr = new List<PreferenceHistogram>();
                foreach (PlayerBase player in this.Players)
                {
                    PreferenceHistogram pref = player.GetPreferenceHistogram(this.PublicBoard, i, last == null ? last : last.Where(x => x.Player != player));
                    pref.Player = player;
                    curr.Add(pref);
                }
                last = curr;
            }
            foreach (PreferenceHistogram pref in last)
            {
                Place choice = pref.Preferences.First(x => x.Value == pref.Preferences.Values.Max()).Key;
                if (!(choice is Home) && PublicBoard.RestaurantHasModifierForThisDay<Closed>((choice as RestaurantPlace).Identifier, day))
                {
                    throw new InvalidRestaurantException();
                }
                if (!(choice is Home))
                {
                    int cost = choice.Cost;
                    cost += PublicBoard.RestaurantHasModifierForThisDay<OneDollarIncrease>((choice as RestaurantPlace).Identifier, day) ? 1 : 0;
                    cost += PublicBoard.RestaurantHasModifierForThisDay<OneDollarDiscount>((choice as RestaurantPlace).Identifier, day) ? -1 : 0;
                    if (cost > PublicBoard.PlayerCash[pref.Player])
                    {
                        choice = PublicBoard.Home;
                    }
                }
                this.PublicBoard.VisitPlace(pref.Player, day, choice);
            }
        }

        private void ChooseRestaurantPreferences()
        {
            foreach (PlayerBase player in this.Players)
            {
                _preferenceCards.Add(player, player.AskPreferences(this.PublicBoard));
            }
            foreach (PlayerBase player in this.Players)
            {
                _loyaltyCards.Add(player, player.AskLoyalty(this.PublicBoard));
            }
        }

        private void ChooseAFavoriteMeal()
        {
            foreach (PlayerBase player in this.Players)
            {
                PublicBoard.SetFavoriteFoodForPlayer(player, player.AskFavoriteFood(this.PublicBoard));
            }
        }

        private void DealCardsToPlayers()
        {
            FoodDeck.Recreate();
            LoyaltyDeck.Recreate();
            PreferencesDeck.Recreate();

            foreach (PlayerBase player in this.Players)
            {
                player.SignalNewWeek(this.PublicBoard);
            }

            for (int i = 0; i < 3; i++)
            {
                foreach (PlayerBase player in this.Players)
                {
                    player.GiveFoodCard(this.FoodDeck.Draw());
                }
            }
            for (int i = 0; i < 3; i++)
            {
                foreach (PlayerBase player in this.Players)
                {
                    player.GiveLoyaltyCard(this.LoyaltyDeck.Draw());
                }
            }
            for (int i = 0; i < 4; i++)
            {
                foreach (PlayerBase player in this.Players)
                {
                    player.GivePreferenceCard(this.PreferencesDeck.Draw());
                }
            }
        }

        private void RevealDailyModifiers()
        {
            foreach (Restaurant restaurant in Extensions.Restaurants)
            {
                PublicBoard.Restaurants[restaurant].SetDailyModifier(this.RestaurantDailyModifierDeck.Draw());
            }
        }

        private void RevealTeamObjective()
        {
            if (!this.Players.ActionForCharacter(Character.CEO, ceo =>
            {
                PublicBoard.CurrentTeamBonus = ceo.ChooseOneTeamBonus(this.PublicBoard, this.TeamBonusDeck.Draw(), this.TeamBonusDeck.Draw());
            }))
            {
                PublicBoard.CurrentTeamBonus = this.TeamBonusDeck.Draw();
            }
        }

        private void CreateRestaurantMenu()
        {
            List<FoodType> pool = new List<FoodType>();

            foreach (FoodType type in Extensions.FoodTypes)
            {
                pool.Add(type);
                pool.Add(type);
            }

            pool.Shuffle();

            int i = 0;
            foreach (Restaurant restaurant in Extensions.Restaurants)
            {
                RestaurantPlace place = PublicBoard.Restaurants[restaurant];
                place.ClearRestaurant();
                place.AddFoodToMenu(place.BaseFood);
                if (restaurant == Restaurant.Russo)
                {
                    place.AddFoodToMenu(pool[i++]);
                }
                else if (restaurant == Restaurant.JoeAndLeos)
                {
                    place.AddFoodToMenu(pool[i++]);
                    place.AddFoodToMenu(pool[i++]);
                    place.AddFoodToMenu(pool[i++]);
                }
                else
                {
                    place.AddFoodToMenu(pool[i++]);
                    place.AddFoodToMenu(pool[i++]);
                }
            }
        }

        private void RevealPlayerWeeklyObjectives(int turn_index)
        {
            List<PlayerBonusCard> list = new List<PlayerBonusCard>
            {
                this.PlayerBonusDeck.Draw()
            };
            if (turn_index >= 2)
            {
                list.Add(this.PlayerBonusDeck.Draw());
            }
            if (turn_index >= 3)
            {
                list.Add(this.PlayerBonusDeck.Draw());
            }
            PublicBoard.SetNewPlayerBonuses(list);
        }
    }
}
