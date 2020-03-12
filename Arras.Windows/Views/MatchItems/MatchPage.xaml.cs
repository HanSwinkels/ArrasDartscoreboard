using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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
    using ReuseableComponents;

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
        /// The number of players in the current match.
        /// </summary>
        private int numPlayers;

        /// <summary>
        /// List containing the <see cref="MatchScoreItem"/> of all players.
        /// </summary>
        private readonly List<UserControl> MatchScoreItems = new List<UserControl>();

        /// <summary>
        /// List containing the <see cref="MatchStatsItem"/> of all players.
        /// </summary>
        private readonly List<MatchStatsItem> MatchStatsItems = new List<MatchStatsItem>();

        /// <summary>
        /// List of PlayerScoreItems used to update the players list, when <see cref="numPlayers"/> is greater than 2.
        /// </summary>
        private ObservableCollection<PlayerScoreItem> PlayerScoreItems = new ObservableCollection<PlayerScoreItem>();

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

            numPlayers = this.matchService.StandardMatch.Players.Count();

            this.matchService.InitializeGame();
            this.matchService.SetHasStarted();

            this.SetStatsScoreItems();

            this.UpdateScoreItems();

            if (numPlayers > 2)
                PlayersListView.ItemsSource = PlayerScoreItems;

            // Initialize values.
            this.SetCurrentTurn();
            this.SetTurnStartLeg();

            var player = this.matchService.GetTurn();

            if (player.PlayerType == PlayerType.Bot)
                EnterBotScore(player);
        }

        /// <summary>
        /// Initialize the stats and score items, depending on the number of players.
        /// </summary>
        private void SetStatsScoreItems()
        {
            switch (numPlayers)
            {
                case 1:
                    this.MatchScoreItems.Add(MatchScoreItemWide);
                    this.MatchStatsItems.Add(MatchStatsItemOne);

                    TopRowMultiplePlayer.Visibility = Visibility.Visible;
                    TopRowTwoPlayer.Visibility = Visibility.Collapsed;
                    ScoreListView.Visibility = Visibility.Visible;
                    PlayersListView.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    TopRowMultiplePlayer.Visibility = Visibility.Collapsed;
                    TopRowTwoPlayer.Visibility = Visibility.Visible;

                    this.MatchScoreItems.Add(MatchScoreItemOne);
                    this.MatchStatsItems.Add(MatchStatsItemOne);
                    this.MatchScoreItems.Add(MatchScoreItemTwo);
                    this.MatchStatsItems.Add(MatchStatsItemTwo);
                    break;
                default:
                    TopRowMultiplePlayer.Visibility = Visibility.Visible;
                    TopRowTwoPlayer.Visibility = Visibility.Collapsed;
                    ScoreListView.Visibility = Visibility.Collapsed;
                    PlayersListView.Visibility = Visibility.Visible;

                    this.MatchScoreItems.Add(MatchScoreItemWide);
                    this.MatchStatsItems.Add(MatchStatsItemOne);
                    break;
            }
        }

        /// <summary>
        /// Enter the score for the bot player
        /// </summary>
        /// <param name="player">A bot player for which to enter the score.</param>
        /// <returns></returns>
        private async Task EnterBotScore(Player player)
        {
            if (player.PlayerType == PlayerType.Bot)
                throw new Exception("This player is not of type bot");

            await Task.Delay(500);

            var playerService = this.matchService.PlayerServices.First(x => x.Player == player);
            scoreInputBox.Text = playerService
                .GenerateScore(playerService.GetRemainingScore(this.matchService.StandardMatch)).ToString();

            await Task.Delay(500);

            KeyboardEnter_Click(null, null);
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

            // If the legs is won by the bot, automatically enter 3 darts finish.
            if (player.PlayerType == PlayerType.Bot && score == ScoreValidationType.EndsLeg)
            {
                EndLegGrid.Visibility = Visibility.Collapsed;
                this.matchService.EndLeg(3);
            }

            this.SetCurrentTurn();
            this.UpdateScoreItems();
            this.UpdateStatsItems();

            // When the current player at turn is a bot
            player = this.matchService.GetTurn();

            if (player.PlayerType == PlayerType.Bot)
                await EnterBotScore(player);

            Console.WriteLine("ENTER");
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

        #region Update

        /// <summary>
        /// Initializes and updates the score item of all players
        /// </summary>
        private void UpdateScoreItems()
        {
            switch (numPlayers)
            {
                case 1:
                    this.UpdateScoreItems1();
                    break;
                case 2:
                    this.UpdateScoreItems2();
                    break;
                default:
                    this.UpdateScoreItemsMore();
                    break;
            }
        }

        /// <summary>
        /// Updates the top row items of the match page, for single player.
        /// </summary>
        private void UpdateScoreItems1()
        {
            var currentScoreItem = this.matchService.PlayerServices.First().GetPlayerScoreItem(this.matchService.StandardMatch);
            MatchScoreItems.First().DataContext = currentScoreItem;

            ScoreListView.ItemsSource = this.matchService.GetCurrentLeg().LegByPlayers.First().Scores;
        }

        /// <summary>
        /// Updates the top row items of the match page, for two players.
        /// </summary>
        private void UpdateScoreItems2()
        {
            for (var i = 0; i < MatchScoreItems.Count; i++)
            {
                MatchScoreItems[i].DataContext = this.matchService.PlayerServices[i]
                    .GetPlayerScoreItem(this.matchService.StandardMatch);
            }
        }

        /// <summary>
        /// Updates the top row items of the match page, for > 2 players.
        /// </summary>
        private void UpdateScoreItemsMore()
        {
            // When there are more than two players, update the score item to show the current player.
            // And then update the listview holding the list of players.
            var player = this.matchService.GetTurn();
            var playerService = this.matchService.PlayerServices.First(x => x.Player == player);

            var currentScoreItem = playerService.GetPlayerScoreItem(this.matchService.StandardMatch);
            MatchScoreItems.First().DataContext = currentScoreItem;

            var scoreItems = this.matchService.PlayerServices
                .Select(x => x.GetPlayerScoreItem(this.matchService.StandardMatch)).ToList();

            // When no items are in the list, first fill the list. Otherwise update values.
            if (PlayerScoreItems.Count == 0)
            {
                for (int i = 0; i < scoreItems.Count(); i++)
                {
                    PlayerScoreItems.Add(scoreItems[i]);
                }
            }
            else
            {
                for (int i = 0; i < scoreItems.Count(); i++)
                {
                    PlayerScoreItems[i] = scoreItems[i];
                }
            }

        }

        /// <summary>
        /// Initializes and updates the stats items of all players
        /// </summary>
        private void UpdateStatsItems()
        {
            if (numPlayers == 2)
            {
                for (var i = 0; i < MatchStatsItems.Count; i++)
                {
                    MatchStatsItems[i].DataContext = this.matchService.PlayerServices[i]
                        .GetAllStats(this.matchService.StandardMatch);
                }
            }
            else
            {
                var player = this.matchService.GetTurn();
                MatchStatsItems.First().DataContext = this.matchService.PlayerServices.First(x => x.Player == player)
                    .GetAllStats(this.matchService.StandardMatch);
            }
        }

        #endregion
    }
}