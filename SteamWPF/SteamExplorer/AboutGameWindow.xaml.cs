using System.Windows;
using GamesLibrary;
using UsersLibrary;
using System.Globalization;

namespace SteamExplorer
{
    /// <summary>
    /// Interaction logic for AboutGameWindow.xaml
    /// </summary>
    public partial class AboutGameWindow : Window
    {
        private readonly Account? _currentUser;
        private readonly Game _game;
        public AboutGameWindow(Game game, Account? currentUser = null)
        {
            InitializeComponent();
            DataContext = game;
            _currentUser = currentUser;
            _game = game;
            BuyGameButton.IsEnabled = _currentUser != null;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BuyGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            var price = _game.Price![1..];
            if (double.TryParse(price, NumberStyles.Any, CultureInfo.InvariantCulture, out var priceForGame))
            {
                if (_currentUser!.OwnsGame(_game))
                {
                    _ = System.Windows.MessageBox.Show("Hru už vlastníš",
                        "Upozornenie", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (_currentUser.Wallet < priceForGame)

                {
                    _ = System.Windows.MessageBox.Show("Nemáš dostatok financií na kúpenie hry",
                        "Upozornenie", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _ = System.Windows.MessageBox.Show("Hra bola úspešne pridaná do tvojej knižnice",
                        "Oznámenie", MessageBoxButton.OK, MessageBoxImage.Information);
                    _currentUser.BuyGame(_game, priceForGame);
                }
            }
            else if (_game.IsForFree)
            {
                if (_currentUser!.OwnsGame(_game))
                {
                    _ = System.Windows.MessageBox.Show("Hru už vlastníš",
                        "Upozornenie", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _ = System.Windows.MessageBox.Show("Hra bola úspešne pridaná do tvojej knižnice",
                        "Oznámenie", MessageBoxButton.OK, MessageBoxImage.Information);
                    _currentUser.BuyGame(_game, priceForGame);
                }
            }
        }
    }
}
