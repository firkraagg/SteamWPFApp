using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UsersLibrary;
using static Microsoft.VisualStudio.Services.Graph.GraphResourceIds;

namespace SteamExplorer
{
    /// <summary>
    /// Interaction logic for AccountSettingsWindow.xaml
    /// </summary>
    public partial class AccountSettingsWindow : Window
    {
        private readonly Account? _account;
        private readonly AccountDatabase? _accountDatabase;
        private string? _imagePath;
        public AccountSettingsWindow(Account? account, AccountDatabase? accountDatabase)
        {
            InitializeComponent();
            _account = account;
            _accountDatabase = accountDatabase;
            DataContext = _account;
            if (_account == null || _account.ImageUri == null) return;
            var image = new Image()
            
            {
                Source = new BitmapImage(_account.ImageUri),
                Width = 75,
                Height = 75,
                Margin = new Thickness(5)
            };
            
            UploadImageButton.Content = image;
        }

        private void UploadImageButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg"
            };

            if (openFileDialog.ShowDialog() != true) return;

            _imagePath = openFileDialog.FileName;
            var image = new Image()
            {
                Source = new BitmapImage(new Uri(openFileDialog.FileName)),
                Width = 75,
                Height = 75,
                Margin = new Thickness(5)
            };
            UploadImageButton.Content = image;
        }

        private void AccountSaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Account? foundAccount = null;
            if (_account != null)
            {
                foundAccount = _accountDatabase!.FindAccount(_account!);
                if (foundAccount == _account)
                {
                    foundAccount = null;
                }
            }
            
            if (!MyRegex().IsMatch(AccountNameTextBox.Text) || !MyRegex1().IsMatch(AccountSurnameTextBox.Text))
            {
                ErrorMessageTextBlock.Text = "Meno a priezvisko môže obsahovať len písmená!";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
            else if (foundAccount != null)
            {
                 ErrorMessageTextBlock.Text = "Účet s týmto e-mailom neexistuje!";
                 ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
            else if (!MyRegex2().IsMatch(AccoutEmailTextBox.Text))
            {
                ErrorMessageTextBlock.Text = "Zlý formát e-mailovej adresy";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
            else if (AccountPasswordTextBox.Text.Length < 8)
            {
                ErrorMessageTextBlock.Text = "Heslo musí obsahovať najmenej 8 znakov!";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
            else if (!MyRegex3().IsMatch(AccountPhoneNumberTextBox.Text) || !MyRegex4().IsMatch(AccountPhoneNumberTextBox.Text))
            {
                ErrorMessageTextBlock.Text = "Telefónne číslo musí byť vo formáte '+421 999 999 999' alebo '0909 999 999'!";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                _account!.Name = AccountNameTextBox.Text;
                _account.Surname = AccountSurnameTextBox.Text;
                _account.Password = AccountPasswordTextBox.Text;
                _account.Email = AccoutEmailTextBox.Text;
                _account.PhoneNumber = AccountPhoneNumberTextBox.Text;

                AccountNameTextBox.Text = _account!.Name;
                AccountSurnameTextBox.Text = _account.Surname;
                AccountPasswordTextBox.Text = _account.Password;
                AccoutEmailTextBox.Text = _account.Email;
                AccountPhoneNumberTextBox.Text = _account.PhoneNumber;
                UploadImageButton.Content = _account.ImageUri;

                if (_imagePath != null)
                {
                    _account.ImageUri = new Uri(_imagePath);
                    _imagePath = null;
                }

                ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
                DialogResult = true;
                Close();
            }
        }

        private void AccountCancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        [GeneratedRegex(@"^[A-Za-z]*$")]
        private static partial Regex MyRegex();
        [GeneratedRegex(@"^[A-Za-z]*$")]
        private static partial Regex MyRegex1();
        [GeneratedRegex(@"^[0-9+]*$")]
        private static partial Regex MyRegex3();
        [GeneratedRegex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{1,3}$")]
        private static partial Regex MyRegex2();
        [GeneratedRegex(@"^[0-9+]*$")]
        private static partial Regex MyRegex4();
    }
}
