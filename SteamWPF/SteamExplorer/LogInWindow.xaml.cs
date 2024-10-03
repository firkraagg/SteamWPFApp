using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using UsersLibrary;

namespace SteamExplorer
{
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _areButtonsEnabled;
        public Account? CurrentUser;
        private readonly AccountDatabase _accounts;

        public bool AreButtonsEnabled
        {
            get => _areButtonsEnabled;
            set
            {
                _areButtonsEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AreButtonsEnabled)));
            }
        }
        public LogInWindow(AccountDatabase accountDatabase)
        {
            InitializeComponent();
            _accounts = accountDatabase;
            DataContext = this;
        }

        private void LogInButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!MyRegex1().IsMatch(EmailTextBox.Text))
            {
                ErrorMessageTextBlock.Text = "Zlý formát e-mailovej adresy";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
                LoginAsAnUser(_accounts, EmailTextBox.Text, PasswordTextBox.Text);
            }
        }

        private void RegisterButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!MyRegex().IsMatch(EmailTextBox.Text))
            {
                ErrorMessageTextBlock.Text = "Zlý formát e-mailovej adresy";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
            else if (PasswordTextBox.Text.Length < 8)
            {
                ErrorMessageTextBlock.Text = "Heslo musí obsahovať najmenej 8 znakov!";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
                AddAccount(_accounts, EmailTextBox.Text, PasswordTextBox.Text);
            }
        }

        private void LoginAsAnUser(AccountDatabase accountDatabase, string email, string password)
        {
            var account = new Account(email, password);
            var foundAccount = accountDatabase.FindAccount(account);
            if (accountDatabase.FindAccount(account) == null)
            {
                ErrorMessageTextBlock.Text = "Účet s týmto e-mailom neexistuje!";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            } 
            else if (accountDatabase.FindAccount(account) != null)
            {
                if (foundAccount!.Password != password)
                {
                    ErrorMessageTextBlock.Text = "Zadali ste zlé heslo!";
                    ErrorMessageTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    CurrentUser = foundAccount;
                    Close();
                }
            }
        }

        private void AddAccount(AccountDatabase accountDatabase, string email, string password)
        {
            var account = new Account(email, password);
            if (!accountDatabase.AddAccount(account))
            {
                ErrorMessageTextBlock.Text = "Účet s touto e-mailovou adresou už existuje!";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
                _ = System.Windows.MessageBox.Show("Bol si zaregistrovaný",
                    "Registrácia úspešná", MessageBoxButton.OK, MessageBoxImage.Information);
                EmailTextBox.Text = "";
                PasswordTextBox.Text = "";
            }
        }

        private void EmailAndPasswordTextBoxes_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (EmailTextBox.Text.Length > 0 && PasswordTextBox.Text.Length > 0)
            {
                AreButtonsEnabled = true;
            }
            else
            {
                AreButtonsEnabled = false;
            }
        }

        [GeneratedRegex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{1,3}$")]
        private static partial Regex MyRegex();
        [GeneratedRegex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{1,3}$")]
        private static partial Regex MyRegex1();
    }
}
