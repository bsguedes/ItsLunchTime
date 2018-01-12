using BotAgressive;
using ItsLunchTimeCore;
using System.Collections.Generic;
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
            List<Player> players = new List<Player>() { new AgressiveBot(), new AgressiveBot(), new AgressiveBot() };
            Game game = new Game(players, DifficultyLevel.Easy);
        }
    }
}
