using System;
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

namespace Arras.Windows.Views.MatchItems
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Business;
    using Common.Match;
    using Common.Player;
    using global::Windows.UI.Text;
    using Microsoft.Toolkit.Uwp.UI.Extensions;
    using Newtonsoft.Json;
    using ReuseableComponents;
    using Button = global::Windows.UI.Xaml.Controls.Button;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MatchPage : Page
    {
        /// <summary>
        /// Input textbox where the user enters their scores.
        /// </summary>
        private TextBox scoreInputBox;

        /// <summary>
        /// The matchservice containing all information and methods for a match.
        /// </summary>
        private MatchService matchService;

        /// <summary>
        /// List containing the <see cref="MatchScoreItem"/> of all players.
        /// </summary>
        private readonly ObservableCollection<MatchScoreItem> MatchScoreItems = new ObservableCollection<MatchScoreItem>();

        /// <summary>
        /// List containing the <see cref="MatchStatsItem"/> of all players.
        /// </summary>
        private readonly ObservableCollection<MatchStatsItem> MatchStatsItems = new ObservableCollection<MatchStatsItem>();

        /// <summary>
        /// Initializes this class.
        /// </summary>
        public MatchPage()
        {
            this.InitializeComponent();

            this.SetUpKeyboard();
        }

        /// <summary>
        /// Handles OnNavigatedTo event.
        /// </summary>
        /// <param name="e">Arguments passed to this page including parameter.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.matchService = e.Parameter as MatchService;

            if (this.matchService == null)
                throw new Exception("Parameter not of type MatchService");

            this.matchService.InitializeGame();
            this.matchService.SetHasStarted();

            var numPlayers = this.matchService.StandardMatch.Players.Count();

            this.MatchScoreItems.Add(MatchScoreItemOne);
            this.MatchStatsItems.Add(MatchStatsItemOne);

            if (numPlayers == 2)
            {
                this.MatchScoreItems.Add(MatchScoreItemTwo);
                this.MatchStatsItems.Add(MatchStatsItemTwo);
            }


            this.UpdateScoreItems();

            // Initialize values.
            this.SetCurrentTurn();
            this.SetTurnStartLeg();
        }

        /// <summary>
        /// Initializes and updates the score item of both players
        /// </summary>
        private void UpdateScoreItems()
        {
            for (var i = 0; i < MatchScoreItems.Count; i++)
            {
                MatchScoreItems[i].DataContext = this.matchService.playerServices[i].GetPlayerScoreItem(this.matchService.StandardMatch);
            }
        }

        /// <summary>
        /// Initializes and updates the stats items of both players
        /// </summary>
        private void UpdateStatsItems()
        {
            for (var i = 0; i < MatchStatsItems.Count; i++)
            {
                MatchStatsItems[i].DataContext = this.matchService.playerServices[i].GetAllStats(this.matchService.StandardMatch);
            }
        }

        #region Keyboard

        /// <summary>
        /// Sets up the click events of the nested buttons inside the keyboard
        /// </summary>
        private void SetUpKeyboard()
        {
            scoreInputBox = Keyboard.FindChildByName("ScoreInputBox") as TextBox;
            var undoButton = Keyboard.FindChildByName("KeyboardUndo") as Button;
            var enterButton = Keyboard.FindChildByName("KeyboardEnter") as Button;
            var numberButtons = Keyboard.FindChildren<KeyboardItem>();

            foreach (var button in numberButtons)
            {
                button.RightTapped += Number_RightTapped;
            }

            undoButton.Click += new RoutedEventHandler(KeyboardUndo_Click);
            enterButton.Click += new RoutedEventHandler(KeyboardEnter_Click);

            var endLegButtons = MatchEndLeg.FindChildren<Button>();

            foreach (var button in endLegButtons)
            {
                button.Click += EndLeg_Click;
            }
        }

        /// <summary>
        /// Click event of the number of darts that were used to finish the leg.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EndLeg_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            EndLegGrid.Visibility = Visibility.Collapsed;

            this.matchService.EndLeg(int.Parse(button.Content.ToString()));
            this.UpdateScoreItems();
            this.SetCurrentTurn();
            this.SetTurnStartLeg();
            this.UpdateStatsItems();
        }

        /// <summary>
        /// Right click event on a number of a keyboard, which will enter the secondary score on the item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Number_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var keyboardItem = sender as KeyboardItem;
            scoreInputBox.Text = keyboardItem.SecondaryText;

            KeyboardEnter_Click(sender, e);
        }

        /// <summary>
        /// Undoes the last entered score and updates all stats accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyboardUndo_Click(object sender, RoutedEventArgs e)
        {
            this.matchService.UndoTurn();
            this.UpdateScoreItems();

            this.SetCurrentTurn();
            this.SetTurnStartLeg();
            this.UpdateStatsItems();

            Console.WriteLine("UNDO");
        }

        /// <summary>
        /// Confirms the entered score and updates everything accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void KeyboardEnter_Click(object sender, RoutedEventArgs e)
        {
            var player = matchService.GetTurn();

            // When no input is entered yet, just return.
            if (scoreInputBox.Text == "")
                return;

            var score = matchService.EnterScore(int.Parse(scoreInputBox.Text), player);
            scoreInputBox.Text = "";

            switch (score)
            {
                case ScoreValidationType.EndsLeg:
                    EndLegGrid.Visibility = Visibility.Visible;
                    break;
                case ScoreValidationType.Valid:
                    break;
                case ScoreValidationType.Invalid:
                    break;
                default:
                    break;
            }

            this.SetCurrentTurn();
            this.UpdateScoreItems();
            this.UpdateStatsItems();

            player = this.matchService.GetTurn();
            if (player.GetType() == typeof(BotPlayer))
            {
                await Task.Delay(500);

                // TODO pass on the remaining score, so the bot never returns an invalid score.
                scoreInputBox.Text = this.matchService.playerServices.First(x => x.Player == player).GenerateScore().ToString();

                // Sleep for one second, to ensure a clear overview of the score from the bot, for the user.
                await Task.Delay(500);

                KeyboardEnter_Click(sender, e);
            }
            Console.WriteLine("ENTER");
        }

        private void sleep()
        {

        }
        
        #endregion

        #region Turn Methods

        /// <summary>
        /// Sets the visibility of the ellipse showcasing which player started the current leg.
        /// </summary>
        private void SetTurnStartLeg()
        {
            var legStartedPlayer = this.matchService.GetStartingPlayer();
            var indexPlayer = this.matchService.StandardMatch.Players.IndexOf(legStartedPlayer);

            var turnGrids = new List<Grid>
            {
                MatchScoreItemOne.FindChildByName("TurnGrid") as Grid,
                MatchScoreItemTwo.FindChildByName("TurnGrid") as Grid
            };

            for (var i = 0; i < turnGrids.Count; i++)
            {
                turnGrids[i].Visibility = i == indexPlayer ? Visibility.Visible : Visibility.Collapsed;
            }

        }

        /// <summary>
        /// Sets the boldness of the scores for both player to indicate whose turn it is.
        /// </summary>
        private void SetCurrentTurn()
        {
            var currentPlayer = this.matchService.GetTurn();
            var indexPlayer = this.matchService.StandardMatch.Players.IndexOf(currentPlayer);

            var scoreText = new List<TextBlock>
            {
                MatchScoreItemOne.FindChildByName("ScoreText") as TextBlock,
                MatchScoreItemTwo.FindChildByName("ScoreText") as TextBlock
            };

            for (var i = 0; i < scoreText.Count; i++)
            {
                scoreText[i].FontWeight = i == indexPlayer ? FontWeights.Bold : FontWeights.Normal;
            }
        }

        #endregion
    }


}
