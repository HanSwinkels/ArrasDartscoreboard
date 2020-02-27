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
    public sealed partial class MenuCard : UserControl
    {
        public MenuCard()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty TopPrimaryTextProperty = DependencyProperty.Register("TopPrimaryText", typeof(string), typeof(MenuCard), null);
        public static readonly DependencyProperty TopSecondaryTextProperty = DependencyProperty.Register("TopSecondaryText", typeof(string), typeof(MenuCard), null);
        public static readonly DependencyProperty BottomPrimaryTextProperty = DependencyProperty.Register("BottomPrimaryText", typeof(string), typeof(MenuCard), null);
        public static readonly DependencyProperty BottomSecondaryTextProperty = DependencyProperty.Register("BottomSecondaryText", typeof(string), typeof(MenuCard), null);

        public string TopPrimaryText
        {
            get => (string)GetValue(TopPrimaryTextProperty);
            set => SetValue(TopPrimaryTextProperty, value);
        }

        public string TopSecondaryText
        {
            get => (string)GetValue(TopSecondaryTextProperty);
            set => SetValue(TopSecondaryTextProperty, value);
        }

        public string BottomPrimaryText
        {
            get => (string)GetValue(BottomPrimaryTextProperty);
            set => SetValue(BottomPrimaryTextProperty, value);
        }

        public string BottomSecondaryText
        {
            get => (string)GetValue(BottomSecondaryTextProperty);
            set => SetValue(BottomSecondaryTextProperty, value);
        }
    }
}
