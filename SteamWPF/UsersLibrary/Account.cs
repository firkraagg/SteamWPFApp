using GamesLibrary;
using System.ComponentModel;

namespace UsersLibrary
{
    public class Account(string email, string password) : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string Email { get; set; } = email;
        public string Password { get; set; } = password;
        public string? PhoneNumber { get; set; }
        public Uri? ImageUri { get; set; }

        private double _wallet;

        public double Wallet
        {
            get => _wallet;
            set
            {
                _wallet = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wallet)));
            }
        }
        public List<Game>? Games { get; set; } = [];

        public void BuyGame(Game game, double price)
        {
            Wallet -= price;
            Wallet = Math.Round(Wallet, 2);
            Games!.Add(game);
        }

        public bool OwnsGame(Game game)
        {
            foreach (var g in Games!)
            {
                if (g.Title.Equals(game.Title))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
