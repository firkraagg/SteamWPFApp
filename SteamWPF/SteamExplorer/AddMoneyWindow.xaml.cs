using System.Windows;
using UsersLibrary;

namespace SteamExplorer
{
    /// <summary>
    /// Interaction logic for AddMoneyWindow.xaml
    /// </summary>
    public partial class AddMoneyWindow : Window
    {
        private readonly Account? _currentUser;
        public AddMoneyWindow(Account? currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddMoneyButton5_OnClick(object sender, RoutedEventArgs e)
        {
            _currentUser!.Wallet += 5;
            ShowTextInfo();
        }

        private void AddMoneyButton10_OnClick(object sender, RoutedEventArgs e)
        {
            _currentUser!.Wallet += 10;
            ShowTextInfo();
        }

        private void AddMoneyButton20_OnClick(object sender, RoutedEventArgs e)
        {
            _currentUser!.Wallet += 20;
            ShowTextInfo();
        }

        private void AddMoneyButton30_OnClick(object sender, RoutedEventArgs e)
        {
            _currentUser!.Wallet += 30;
            ShowTextInfo();
        }

        private void ShowTextInfo()
        {
            InfoTextBlock.Text = $"Peniaze boli pridané. Aktuálny zostatok: {_currentUser!.Wallet} €";
            InfoTextBlock.Visibility = Visibility.Visible;
        }
    }
}
