using System.Windows;
using UsersLibrary;

namespace SteamExplorer
{
    /// <summary>
    /// Interaction logic for OwnedGamesWindow.xaml
    /// </summary>
    public partial class OwnedGamesWindow : Window
    {
        public OwnedGamesWindow(Account currentUser, SettingsWindow settings)
        {
            InitializeComponent();
            DataContext = this;
            ListOfOwnedGames.ItemsSource = currentUser.Games;
            ListOfOwnedGames.Background = settings.DarkThemeCheckBox.IsChecked == true ? System.Windows.Media.Brushes.DarkGray : System.Windows.Media.Brushes.Azure;
        }
    }
}
