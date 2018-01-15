using BotSample;
using ItsLunchTimeCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ItsLunchTime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<Character, int> points = new Dictionary<Character, int>();
            Dictionary<Character, int> matches = new Dictionary<Character, int>();
            Dictionary<Character, Dictionary<VictoryPointsSource, int>> split = new Dictionary<Character, Dictionary<VictoryPointsSource, int>>();
            int[] bonusTeam = new int[5];
            int[] teamScore = new int[Game.MAX_TEAM_SCORE + 1];
            int majorityCount = 0;

            foreach (Character c in Extensions.CharacterTypes)
            {
                points.Add(c, 0);
                matches.Add(c, 0);
                split.Add(c, new Dictionary<VictoryPointsSource, int>());
                foreach (VictoryPointsSource v in Enum.GetValues(typeof(VictoryPointsSource)))
                {
                    split[c].Add(v, 0);
                }
            }

            for (int i = 0; i < 10000; i++)
            {
                IEnumerable<Character> characters = Extensions.CharacterTypes.Scramble();
                List<PlayerBase> players = new List<PlayerBase>();
                for (int j = 0; j < 6; j++)
                {
                    players.Add(new SampleBot(characters.ElementAt(j)));
                }
                Game game = new Game(players, DifficultyLevel.Easy);
                teamScore[game.PublicBoard.TeamScore]++;
                if (game.Win)
                {
                    majorityCount += game.MajorityCount;
                    bonusTeam[game.PublicBoard.TeamBonusDoneCount]++;
                    foreach (PlayerBase player in players)
                    {
                        points[player.Character] += game.PublicBoard.PlayerScores[player];
                        matches[player.Character]++;
                        foreach (VictoryPointsSource v in Enum.GetValues(typeof(VictoryPointsSource)))
                        {
                            split[player.Character][v] += game.PublicBoard.SeparatedScores[player][v];
                        }
                    }
                }
            }

            Console.WriteLine("Won games: {0} - team bonus: {1}", bonusTeam.Sum(), string.Join(", ", bonusTeam));
            Console.WriteLine("Majorities: {1}    Team Score distribution: {0}", string.Join(", ", teamScore), majorityCount/ bonusTeam.Sum());
            foreach (Character c in points.OrderByDescending(x => x.Value / matches[x.Key]).Select(x => x.Key))
            {
                Dictionary<VictoryPointsSource, int> dict = new Dictionary<VictoryPointsSource, int>();
                foreach (VictoryPointsSource v in Enum.GetValues(typeof(VictoryPointsSource)))
                {
                    dict.Add(v, split[c][v] / matches[c]);
                }
                Console.WriteLine("{0,-18}: {1,-3} ({2})", c, points[c] / matches[c], string.Join(" ", dict.Select(x => string.Format("{0}={1,-3}", x.Key, x.Value))));
            }

        }
    }
}
