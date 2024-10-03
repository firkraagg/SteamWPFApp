using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using SteamExplorer.Properties;
using Brushes = System.Windows.Media.Brushes;

namespace SteamExplorer
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public System.Windows.Media.FontFamily Font { get; set; } = new System.Windows.Media.FontFamily("Comic Sans Ms");
        public SolidColorBrush? RowBackground { get; set; } = Brushes.White;
        public SolidColorBrush? AlternativeRowBackground { get; set; }

        private readonly DataGrid? _gamesDataGrid;

        public SettingsWindow(DataGrid? gamesDataGrid = null)
        {
            InitializeComponent();
            DataContext = this;
            DarkThemeCheckBox.IsChecked = Properties.Settings.Default.DarkModeEnabled;
            FixedSizeOfWindow.IsChecked = Properties.Settings.Default.FixedSizeOfWindow;
            InitializeComboBoxesItems();
            _gamesDataGrid = gamesDataGrid;

            UpdateMainWindow();
        }


        private void InitializeComboBoxesItems()
        {
            var fontFamilyName = Properties.Settings.Default.FontFamilyName;
            foreach (FontFamily fontFamily in FontsComboBox.Items)
            {
                if (!fontFamily.Source.Equals(fontFamilyName)) continue;
                FontsComboBox.SelectedItem = fontFamily;
                Loaded += (sender, e) =>
                {
                    FontsComboBox.Text = fontFamilyName;
                };

                break;
            }

            var rowBackgroundText = Properties.Settings.Default.RowBackground;
            foreach (string rb in RowBackgroundComboBox.Items)
            {
                if (!rb.Equals(rowBackgroundText)) continue;
                RowBackgroundComboBox.SelectedItem = rb;
                Loaded += (sender, e) =>
                {
                    RowBackgroundComboBox.Text = rowBackgroundText;
                };
                break;
            }

            var alternativeRowBackgroundText = Properties.Settings.Default.AlternativeRowBackground;
            foreach (string altRowBackground in AlternativeRowBackgroundComboBox.Items)
            {
                if (!altRowBackground.Equals(alternativeRowBackgroundText)) continue;
                AlternativeRowBackgroundComboBox.SelectedItem = altRowBackground;
                Loaded += (sender, e) =>
                {
                    AlternativeRowBackgroundComboBox.Text = alternativeRowBackgroundText;
                };
                break;
            }

            var alternativeRowBackground = Properties.Settings.Default.AlternativeRowBackground;
            if (!string.IsNullOrEmpty(alternativeRowBackground))
            {
                AlternativeRowBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(alternativeRowBackground));
            }
        }
        private void DarkThemeCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DarkModeEnabled = DarkThemeCheckBox.IsChecked == true;
            Properties.Settings.Default.Save();

            UpdateMainWindow();
        }

        private void FixedSizeOfWindow_OnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.FixedSizeOfWindow = FixedSizeOfWindow.IsChecked == true;
            Properties.Settings.Default.Save();

            FontsComboBox.Text = Font.Source;
            UpdateMainWindow();
        }

        private void FontsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontsComboBox.SelectedItem == null) return;
            var fontFamilyName = ((FontFamily)FontsComboBox.SelectedItem).Source;
            Font = new System.Windows.Media.FontFamily(fontFamilyName);

            Properties.Settings.Default.FontFamilyName = fontFamilyName;
            Properties.Settings.Default.Save();

            FontsComboBox.Text = fontFamilyName;
            UpdateMainWindow();
        }

        private void RowBackgroundComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RowBackgroundComboBox.SelectedItem == null) return;
            var selectedColorName = (string)RowBackgroundComboBox.SelectedItem;
            var selectedColor = (Color)ColorConverter.ConvertFromString(selectedColorName);
            RowBackground = new SolidColorBrush(selectedColor); 

            Properties.Settings.Default.RowBackground = selectedColorName;
            Properties.Settings.Default.Save();

            UpdateMainWindow();
        }

        private void AlternativeRowBackgroundComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlternativeRowBackgroundComboBox.SelectedItem != null)
            {
                var selectedColorName = (string)AlternativeRowBackgroundComboBox.SelectedItem;
                var selectedColor = (Color)ColorConverter.ConvertFromString(selectedColorName);
                AlternativeRowBackground = new SolidColorBrush(selectedColor);

                Properties.Settings.Default.AlternativeRowBackground = selectedColorName;
                Properties.Settings.Default.Save();

                UpdateMainWindow();
            }
        }

        public void UpdateMainWindow()
        {
            if (DarkThemeCheckBox.IsChecked == true)
            {
                Application.Current.MainWindow!.Background = Brushes.DimGray;
                Application.Current.MainWindow.Foreground = Brushes.White;
                Background = Brushes.DimGray;
                Foreground = Brushes.White;
            }
            else
            {
                Application.Current.MainWindow!.Background = Brushes.White;
                Application.Current.MainWindow.Foreground = Brushes.Black;
                Background = Brushes.White;
                Foreground = Brushes.Black;
            }

            if (FixedSizeOfWindow.IsChecked == true)
            {
                ResizeMode = ResizeMode.NoResize;
                Application.Current.MainWindow.ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                ResizeMode = ResizeMode.CanResize;
                Application.Current.MainWindow.ResizeMode = ResizeMode.CanResize;
            }

            if (_gamesDataGrid != null)
            {
                _gamesDataGrid.Background = DarkThemeCheckBox.IsChecked == true ? System.Windows.Media.Brushes.DarkGray : System.Windows.Media.Brushes.Azure;

                _gamesDataGrid.RowBackground = RowBackground;
                if (AlternativeRowBackground != null)
                {
                    _gamesDataGrid.AlternatingRowBackground = AlternativeRowBackground;
                }
            }

            Application.Current.MainWindow.FontFamily = Font;
            FontFamily = Font;
        }

        private void SettingsWindow_OnClosing(object? sender, CancelEventArgs e)
        {
            e.Cancel = true; 
            Visibility = Visibility.Collapsed;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
