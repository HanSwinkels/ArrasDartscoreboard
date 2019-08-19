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
    public sealed partial class MenuMainItem : UserControl
    {
        public MenuMainItem()
        {
            this.InitializeComponent();
        }



        public string MenuText
        {
            get { return (string)GetValue(MenuTextProperty); }
            set { SetValue(MenuTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MenuText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MenuTextProperty =
            DependencyProperty.Register("MenuText", typeof(string), typeof(MenuMainItem), new PropertyMetadata(0));


    }
}
