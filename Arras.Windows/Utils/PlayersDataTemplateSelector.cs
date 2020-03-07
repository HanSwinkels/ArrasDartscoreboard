namespace Arras.Windows.Utils
{
    using Common.Player;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;

    public class PlayersDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PlayerTemplate { get; set; }
        public DataTemplate BotPlayerTemplate { get; set; }
        public DataTemplate YourselfPlayerTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (item)
            {
                case BotPlayer _:
                    return BotPlayerTemplate;
                case Player _:
                    return PlayerTemplate;
                default:
                    return base.SelectTemplateCore(item);
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}