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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Arras.Windows.Views.ReuseableComponents
{
    public sealed partial class Keyboard : UserControl
    {
        public Keyboard()
        {
            this.InitializeComponent();
        }

        #region KeyboardClicks

        /// <summary>
        /// Handles taps on the on screen keyboard numbers. Shows the entered number in the textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyboardScore_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var clickedItem = sender as KeyboardItem;

            if(ScoreInputBox.Text.Length < 3)
                ScoreInputBox.Text += clickedItem.Text;
        }

        /// <summary>
        /// Handles taps on the clear button. Clears the text of the ScoreInputBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyboardClear_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ScoreInputBox.Text = "";    
        }
        
        #endregion

    }
}
