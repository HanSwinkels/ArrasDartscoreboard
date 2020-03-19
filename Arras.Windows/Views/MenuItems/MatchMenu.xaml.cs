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

            SetsRadioButton.IsChecked = true;

            playersListView.ItemsSource = playersList;
            playersList.Add(new NormalPlayer(""));
        }

        /// <summary>
        /// Start a match given the provided parameters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartMatch_Click(object sender, RoutedEventArgs e)
        {
            // First check if the match settings are valid
            if (!this.AreNamesUnique() || !this.AreLevelsSelected() || playersList.Count == 0)
                return;

            // Create a frame that displays the MatchPage.
            var frame = new Frame();
            frame.Navigate(typeof(MatchPage), CreateMatchService());

            var matchWindow = Window.Current;
            matchWindow.Content = frame;
            matchWindow.Activate();
        }

        #region Validation
        /// <summary>
        /// Checks whether the names in the player list are unique.
        /// </summary>
        /// <returns>True when all names in the player list are unique.</returns>
        private bool AreNamesUnique()
        {
            var names = playersList.Select(x => x.Name);
            if (names.Distinct().Count() == names.Count())
            {
                ErrorTextNoUniqueNames.Visibility = Visibility.Collapsed;
                return true;
            }

            ErrorTextNoUniqueNames.Visibility = Visibility.Visible;
            return false;
        }

        /// <summary>
        /// Check whether for all bots a level is selected.
        /// </summary>
        /// <returns>True when for all bots a level is selected.</returns>
        private bool AreLevelsSelected()
        {
            var botPlayers = playersList.Where(x => x.PlayerType == PlayerType.Bot);
            foreach (BotPlayer bot in botPlayers)
            {
                if (bot.Level == BotLevel.None)
                {
                    ErrorTextNoLevel.Visibility = Visibility.Visible;
                }
                return false;
            }

            ErrorTextNoLevel.Visibility = Visibility.Collapsed;
            return true;
        }
        #endregion


        /// <summary>
        /// Generates a class with a information regarding a match.
        /// </summary>
        /// <returns></returns>
        private MatchService CreateMatchService()
        {
            var iSuddenDeath = SuddenDeathOnRadioButton.IsChecked.GetValueOrDefault(false);
            var isSets = SetsRadioButton.IsChecked.GetValueOrDefault(false);

            var numSets = Parse(TextBoxSets.Text);
            var numLegs = Parse(TextBoxLegs.Text);
            var format = Parse(TextBoxFormat.Text);

            var players = playersListView.Items.Cast<Player>().ToList();

            return isSets
                ? new MatchService(new StandardMatch(StandardMatchType.Sets, numSets.ToMaybe(), numLegs, format,
                    players))
                : new MatchService(new StandardMatch(StandardMatchType.Legs, Maybe<int>.Nothing, numLegs, format,
                    players));
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

            playersList.Add(new NormalPlayer(""));

            this.PlayersCountChanged();


            ErrorTextNoPlayers.Visibility = Visibility.Collapsed;
        }

        private void PlayersCountChanged()
        {
            if (playersList.Count == 0)
            {
                // Show text to indicate that this is invalid
                ErrorTextNoPlayers.Visibility = Visibility.Visible;
            } 

            if (playersList.Count <= 2)
            {
                SetsRadioButton.IsEnabled = true;
                SuddenDeathOnRadioButton.IsEnabled = true;
            }
            else if (playersListView.Items.Count > 2)
            {
                SetsRadioButton.IsChecked = false;
                LegsRadioButton.IsChecked = true;

                SuddenDeathOnRadioButton.IsChecked = false;
                SuddenDeathOffRadioButton.IsChecked = true;

                SetsRadioButton.IsEnabled = false;
                SuddenDeathOnRadioButton.IsEnabled = false;

            }
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
            playersList.Add(new BotPlayer(random.Next(100000).ToString(), BotLevel.None));

            this.PlayersCountChanged();

            ErrorTextNoPlayers.Visibility = Visibility.Collapsed;
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

            this.PlayersCountChanged();
        }

        /// <summary>
        /// Event for when the radio buttons of sets/legs are checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetsLegs_OnChecked(object sender, RoutedEventArgs e)
        {
            var radioBtnText = ((RadioButton)sender).Content.ToString();
            if (radioBtnText == "SETS")
            {
                SetsNumberGrid.Visibility = Visibility.Visible;
                LegsNumberGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                SetsNumberGrid.Visibility = Visibility.Collapsed;
                LegsNumberGrid.Visibility = Visibility.Visible;
            }

        }
    }
}