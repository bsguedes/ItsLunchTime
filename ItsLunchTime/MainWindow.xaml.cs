using BotSample;
using ItsLunchTimeCore;
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
            IEnumerable<Character> characters = Extensions.CharacterTypes.Scramble();
            PlayerBase playerA = new SampleBot(characters.ElementAt(0));
            PlayerBase playerB = new SampleBot(characters.ElementAt(1));
            PlayerBase playerC = new SampleBot(characters.ElementAt(2));
            List<PlayerBase> players = new List<PlayerBase>() { playerA, playerB, playerC };
            Game game = new Game(players, DifficultyLevel.Medium);
        }
    }
}
