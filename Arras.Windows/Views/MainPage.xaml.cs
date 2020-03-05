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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Arras.Windows
{
    using Microsoft.Toolkit.Uwp.UI.Extensions;
    using Views.MenuItems;
    using Views.ReuseableComponents;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Initializing the mainpage. By default show the home menu.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            InfoFrame.Navigate(typeof(HomeMenu));
            
        }

        /// <summary>
        /// Handles a click on the items in the navigation view and navigates correspondingly.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments of the invoked item.</param>
        private void MenuItem_Clicked(object sender, TappedRoutedEventArgs args)
        {
            var menuItem = sender as MenuMainItem;
            var menuText = menuItem.FindChildByName("menuText") as TextBlock;

            switch (menuText.Text)
            {
                case "Home":
                    InfoFrame.Navigate(typeof(HomeMenu));
                    break;
                case "Match":
                    InfoFrame.Navigate(typeof(MatchMenu));
                    break;
                case "Cricket":
                    InfoFrame.Navigate(typeof(HomeMenu));
                    break;
                case "History":
                    InfoFrame.Navigate(typeof(HomeMenu));
                    break;
                case "Profiles":
                    InfoFrame.Navigate(typeof(HomeMenu));
                    break;
                case "Purchases":
                    InfoFrame.Navigate(typeof(HomeMenu));
                    break;
                case "About":
                    InfoFrame.Navigate(typeof(HomeMenu));
                    break;
                default: return;
            }
        }
    }
}
