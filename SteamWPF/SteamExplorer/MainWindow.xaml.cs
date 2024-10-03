using GamesLibrary;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using UsersLibrary;

namespace SteamExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private GameCollection? _games;
        private readonly SettingsWindow _settingsWindow;
        private readonly AccountDatabase _accountDatabase = AccountDatabase.LoadFromJson()!;
        private Account? _currentUser;

        private string _wallet = "";

        public string Wallet
        {
            get => _wallet;
            set
            {
                _wallet = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wallet)));
            }
        }

        private bool _isMoreAboutGameButtonEnabled = false;
        private Game? _selectedGame;
        public bool IsMoreAboutGameButtonEnabled
        {
            get => _isMoreAboutGameButtonEnabled;
            set
            {
                _isMoreAboutGameButtonEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMoreAboutGameButtonEnabled)));
            }
        }

        private bool _hasNoAccount;

        public bool HasNoAccount
        {
            get => _hasNoAccount;
            set
            {
                _hasNoAccount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasNoAccount)));
            }
        }

        private bool _isLoggedIn;

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoggedIn)));
            }
        }

        private bool _isFindButtonEnabled = false;

        public bool IsFindButtonEnabled
        {
            get => _isFindButtonEnabled;
            set
            {
                _isFindButtonEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFindButtonEnabled)));
            }
        }
        private bool _isResetButtonEnabled = false;

        public bool IsResetButtonEnabled
        {
            get => _isResetButtonEnabled;
            set
            {
                _isResetButtonEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsResetButtonEnabled)));
            }
        }
        private int _gamesCount = 0;

        public event PropertyChangedEventHandler? PropertyChanged;
        public int GamesCount
        {
            get => _gamesCount;
            set
            {
                _gamesCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GamesCount)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GamesCountFormatted)));
            }
        }
        public string GamesCountFormatted => $"Počet hier: {GamesCount}";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            _settingsWindow = new SettingsWindow(GamesDataGrid);
            _settingsWindow.UpdateMainWindow();
            HasNoAccount = true;
        }

        private void CloseMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var messageBox = System.Windows.MessageBox.Show("Si si istý, že chceš aplikáciu ukončiť?",
                "Koniec", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (messageBox == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void OpenFile_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFile.Cursor = System.Windows.Input.Cursors.ArrowCD;
            OpenFileDialog openFile = new()
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            };
            if (openFile.ShowDialog() != true) return;
            var filePath = openFile.FileName;
            _ = new FileInfo(filePath);
            _games = GameCollection.LoadGamesFromJsonFile("games.json")!;
            GamesCount = _games.Count;
            GamesDataGrid.ItemsSource = _games;
            _settingsWindow.UpdateMainWindow();
            PopulateComboBoxes();
            if (_gamesCount > 0)
            {
                IsFindButtonEnabled = true;
            }
        }

        private void PopulateComboBoxes()
        {
            GenreComboBox.Items.Clear();
            ReleaseYearComboBox.Items.Clear();

            foreach (var genre in _games!.GetGenres())
            {
                GenreComboBox.Items.Add(genre);
            }

            foreach (var releaseYear in _games.GetReleaseYears())
            {
                ReleaseYearComboBox.Items.Add(releaseYear);
            }
        }

        private void FilterGamesButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!MyRegex().IsMatch(PriceFromTextBox.Text) || !MyRegex1().IsMatch(PriceToTextBox.Text))
            {
                MessageBox.Show("Cena musí byť celé číslo!", "Upozornenie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_games != null)
            {
                var gameName = NameTextBox.Text;
                var genre = GenreComboBox.SelectedItem?.ToString();
                var releaseYear = ReleaseYearComboBox.SelectedItem?.ToString();

                if (int.TryParse(PriceFromTextBox.Text, out var priceFrom) && int.TryParse(PriceToTextBox.Text, out var priceTo))
                {
                    var filteredList = _games!.SearchGames(gameName, genre, releaseYear, priceFrom, priceTo);
                    GamesDataGrid.ItemsSource = filteredList.Games;
                    GamesCount = filteredList.Games.Length;
                }
                else if (int.TryParse(PriceFromTextBox.Text, out var minPrice))
                {
                    var filteredList = _games!.SearchGames(gameName, genre, releaseYear, minPrice);
                    GamesDataGrid.ItemsSource = filteredList.Games;
                    GamesCount = filteredList.Games.Length;
                }
                else if (int.TryParse(PriceToTextBox.Text, out var maxPrice))
                {
                    var filteredList = _games!.SearchGames(gameName, genre, releaseYear, maxPrice: maxPrice);
                    GamesDataGrid.ItemsSource = filteredList.Games;
                    GamesCount = filteredList.Games.Length;
                }
                else
                {
                    var filteredList = _games!.SearchGames(gameName, genre, releaseYear);
                    GamesDataGrid.ItemsSource = filteredList.Games;
                    GamesCount = filteredList.Games.Length;
                }
            }
        }

        private void ResetFiltersButton_OnClick(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = "";
            PriceFromTextBox.Text = "";
            PriceToTextBox.Text = "";
            GenreComboBox.SelectedItem = null;
            ReleaseYearComboBox.SelectedItem = null;
            GamesDataGrid.ItemsSource = _games;
            IsResetButtonEnabled = false;
            GamesCount = _games!.Count;
        }

        private void MoreAboutGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            AboutGameWindow aboutGameWindow = new(_selectedGame!, _currentUser)
            {
                Owner = this,
                FontFamily = FontFamily,
                ResizeMode = ResizeMode.NoResize,
                Background = Background,
                Foreground = Foreground,
        };

            aboutGameWindow.ShowDialog();
        }

        private void AboutAuthorMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            InformationWindow informationWindow = new("Images/creator.png", "O autorovi", "Meno: Daniel Zuzčák", "Študijná skupina: 5ZYI23")
            {
                Owner = this,
                FontFamily = FontFamily,
                ResizeMode = ResizeMode,
            };
            informationWindow.ShowDialog();
        }

        private void AboutApplicationMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            InformationWindow informationWindow = new("Images/app.jpg", "O aplikácií", "Táto aplikácia slúži na vyhľadávanie a filtrovanie počítačových hier.", "Okrem tohto umožňuje používateľom prihlásiť sa a 'nakupovať ľubovoľné hry'.")
            {
                ResizeMode = ResizeMode,
                FontFamily = FontFamily,
                Owner = this,
            };
            informationWindow.ShowDialog();
        }

        private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckFilterControl();
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFilterControl();
        }

        private void CheckFilterControl()
        {
            if (!string.IsNullOrEmpty(NameTextBox.Text) || !string.IsNullOrEmpty(PriceFromTextBox.Text) || !string.IsNullOrEmpty(PriceToTextBox.Text) ||
                GenreComboBox.SelectedItem != null || ReleaseYearComboBox.SelectedItem != null)
            {
                IsResetButtonEnabled = true;
            }
            else
            {
                IsResetButtonEnabled = false;
            }
        }

        private void SettingsMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            _settingsWindow.Visibility = Visibility.Visible;
        }

        private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
        {
            _accountDatabase.SaveToJson(new FileInfo("accounts.json"));
            Application.Current.Shutdown();
        }

        private void GamesDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GamesDataGrid.SelectedItem == null)
            {
                IsMoreAboutGameButtonEnabled = false;
            }
            else
            {
                _selectedGame = GamesDataGrid.SelectedItem as Game;
                IsMoreAboutGameButtonEnabled = true;
            }
        }

        private void LogInMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            LogInWindow logInWindow = new(_accountDatabase)
            {
                Owner = this,
                FontFamily = FontFamily,
                ResizeMode = ResizeMode.NoResize,
                Background = Background,
                Foreground = Foreground
            };
            logInWindow.ShowDialog();
            _currentUser = logInWindow.CurrentUser;
            if (_currentUser == null) return;
            IsLoggedIn = true;
            HasNoAccount = false;
            WalletMenuItem.Visibility = Visibility.Visible;
            WalletNumberMenuItem.Visibility = Visibility.Visible;
            _currentUser.PropertyChanged += UpdateWallet!;
            UpdateWallet(this, null);
        }
        private void UpdateWallet(object sender, PropertyChangedEventArgs? e)
        {
            Wallet = $"${_currentUser!.Wallet:F2}";
        }

        private void AccountSettingsMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            AccountSettingsWindow accountSettingsWindow = new(_currentUser!, _accountDatabase)
            {
                Owner = this,
                FontFamily = FontFamily,
                ResizeMode = ResizeMode,
                Background = Background,
                Foreground = Foreground
            };
            accountSettingsWindow.ShowDialog();
        }

        private void LogOutMenuItem_OnClickMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            _ = System.Windows.MessageBox.Show("Bol si odhlásený",
                "Odhlásenie", MessageBoxButton.OK, MessageBoxImage.Information);
            IsLoggedIn = false;
            HasNoAccount = true;
            _currentUser = null;
            WalletMenuItem.Visibility = Visibility.Collapsed;
            WalletNumberMenuItem.Visibility = Visibility.Collapsed;
        }

        private void WalletMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            AddMoneyWindow addMoneyWindow = new(_currentUser!)
            {
                Owner = this,
                FontFamily = FontFamily,
                ResizeMode = ResizeMode.NoResize,
                Background = Background,
                Foreground = Foreground
            };
            addMoneyWindow.ShowDialog();
        }

        private void AccountOwnedGamesMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            OwnedGamesWindow ownedGamesWindow = new(_currentUser!, _settingsWindow)
            {
                Owner = this,
                FontFamily = FontFamily,
                ResizeMode = ResizeMode,
                Background = Background,
                Foreground = Foreground
            };
            ownedGamesWindow.ShowDialog();
        }

        [GeneratedRegex(@"^[0-9]*$")]
        private static partial Regex MyRegex();
        [GeneratedRegex(@"^[0-9]*$")]
        private static partial Regex MyRegex1();
    }
}