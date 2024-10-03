using System.Windows;
using System.Windows.Media.Imaging;

namespace SteamExplorer
{
    /// <summary>
    /// Interaction logic for InformationWindow.xaml
    /// </summary>
    public partial class InformationWindow : Window
    {
        public InformationWindow()
        {
            InitializeComponent();
        }

        public InformationWindow(string imageSource, string title, string firstText, string secondText)
        {
            InitializeComponent();
            GridImage.Source = new BitmapImage(new Uri(imageSource, UriKind.RelativeOrAbsolute));
            FirstTextBlock.Text = firstText;
            SecondTextBlock.Text = secondText;
            Title = title;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
