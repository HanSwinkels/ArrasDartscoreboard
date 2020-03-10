using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Arras.Windows.Views.MenuItems
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;
    using Business;
    using Common.Match;
    using Common.Player;
    using Functional.Maybe;
    using MatchItems;
    using Microsoft.Toolkit.Uwp.UI.Extensions;
    using static System.Int32;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MatchMenu : Page
    {
        /// <summary>
        /// A list 
        /// </summary>
        private ObservableCollection<Player> playersList = new ObservableCollection<Player>();

        /// <summary>
        /// Initializes the page.
        /// </summary>
        public MatchMenu()
        {
            this.InitializeComponent();
            playersListView.ItemsSource = playersList;
            playersList.Add(new NormalPlayer("", PlayerType.Normal));
        }

        /// <summary>
        /// Start a match given the provided parameters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartMatch_Click(object sender, RoutedEventArgs e)
        {
            // Create a frame that displays the MatchPage.
            var frame = new Frame();
            frame.Navigate(typeof(MatchPage), CreateMatchService());

            var matchWindow = Window.Current;
            matchWindow.Content = frame;
            matchWindow.Activate();
        }

        /// <summary>
        /// Generates a class with a information regarding a match.
        /// </summary>
        /// <returns></returns>
        private MatchService CreateMatchService()
        {
            var iSuddenDeath = SwitchSuddenDeath.IsOn;
            var isSets = SwitchSets.IsOn;

            var numSets = Parse(TextBoxSets.Text);
            var numLegs = Parse(TextBoxLegs.Text);
            var format = Parse(TextBoxFormat.Text);

            var players = playersListView.Items.Cast<Player>().ToList();

            return isSets ? 
                new MatchService(new StandardMatch(StandardMatchType.Sets, numSets.ToMaybe(), numLegs, format, players)) : 
                new MatchService(new StandardMatch(StandardMatchType.Legs, Maybe<int>.Nothing, numLegs, format, players));
        }

        /// <summary>
        /// Adds a player to the list of players for the match.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (playersListView.Items.Count > 7)
                return;

            playersList.Add(new NormalPlayer("", PlayerType.Normal));
        }

        /// <summary>
        /// Adds a bot player to the list of players for the match.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBot_Click(object sender, RoutedEventArgs e)
        {
            if (playersListView.Items.Count > 7)
                return;

            var random = new Random();
            playersList.Add(new BotPlayer(random.Next(100000).ToString(), BotLevel.One));
        }

        /// <summary>
        /// Deletes a player from the list view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeletePlayer_Click(object sender, RoutedEventArgs e)
        {
            var deleteButton = sender as Button;
            var item = deleteButton.DataContext as Player;
            playersList.Remove(item);
        }
    }
    
}
