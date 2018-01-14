using BotAgressive;
using BotSample;
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
            List<PlayerBase> players = new List<PlayerBase>() { new SampleBot(), new SampleBot(), new SampleBot() };
            Game game = new Game(players, DifficultyLevel.Medium);
        }
    }
}
