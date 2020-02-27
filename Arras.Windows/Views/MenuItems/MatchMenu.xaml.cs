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
    using System.Runtime.CompilerServices;
    using Business;
    using Common.Match;
    using Common.Player;
    using Functional.Maybe;
    using MatchItems;
    using static System.Int32;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MatchMenu : Page
    {
        public MatchMenu()
        {
            this.InitializeComponent();
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

            var players = new List<Player>();
            players.Add(new Player(NamePlayerOne.Text, PlayerType.Normal));
            players.Add(new Player(NamePlayerTwo.Text, PlayerType.Normal));

            return isSets ? 
                new MatchService(new StandardMatch(StandardMatchType.Sets, numSets.ToMaybe(), numLegs, format, players)) : 
                new MatchService(new StandardMatch(StandardMatchType.Legs, Maybe<int>.Nothing, numLegs, format, players));
        }
    }
}
