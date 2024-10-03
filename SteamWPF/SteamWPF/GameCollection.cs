using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Globalization;

namespace GamesLibrary
{
    public class GameCollection : ObservableCollection<Game>
    {
        public static GameCollection? LoadGamesFromJsonFile(string path)
        {
            try
            {
                StreamReader streamReader = new(path);
                GameCollection games = [];
                string json = streamReader.ReadToEnd();
                List<Game> gamesList = JsonConvert.DeserializeObject<List<Game>>(json)!;
                HashSet<string> addedTitles = [];
                foreach (var game in gamesList)
                {
                    string processedGenre = game.Genre!.Replace("[u'", "").Replace("']", "").Replace("u'", "").Replace("'", "").Replace("[", "");
                    game.Genre = processedGenre;
                    game.IsForFree = game.Price == "Free to Play";

                    if (addedTitles.Add(game.Title))
                    {
                        games.Add(game);
                    }
                }

                streamReader.Close();
                return games;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Nemôžeš načítať zo súboru: " + e.Message);
                throw;
            }
        }

        public void SaveGamesToJson(FileInfo jsonFile)
        {
            try
            {
                string json = JsonConvert.SerializeObject(this);
                File.WriteAllText(jsonFile.FullName, json);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Nemôžeš uložiť do súboru: " + e.Message);
                throw;
            }
        }

        public IEnumerable<string> GetGenres()
        {
            SortedSet<string> genres = [];
            foreach (var game in this)
            {
                if (!string.IsNullOrEmpty(game.Genre))
                {
                    string[] genresArray = game.Genre!.Split(", ");
                    foreach (var s in genresArray)
                    {
                        string genre = s.Trim();
                        genres.Add(genre);
                    }
                }
            }
            return genres;
        }

        public IEnumerable<int> GetReleaseYears()
        {
            SortedSet<int> years = [];
            foreach (var game in this)
            {
                years.Add(int.Parse(game.Released[^4..]));
            }
            return years;
        }

        public GameArray SearchGames(string? title = null, string? genre = null, string? releaseYear = null, int? minPrice = null, int? maxPrice = null)
        {
            var filteredGames = this.Where(game =>
            {
                bool titleMatch = title == null || game.Title.Contains(title, StringComparison.OrdinalIgnoreCase);
                bool genreMatch = genre == null || game.Genre?.Split(", ").Any(g => g.Equals(genre, StringComparison.OrdinalIgnoreCase)) == true;
                bool releaseYearMatch = releaseYear == null || game.Released.Contains(releaseYear, StringComparison.OrdinalIgnoreCase) || game.Released.EndsWith(releaseYear, StringComparison.OrdinalIgnoreCase);

                bool isPriceValid = float.TryParse(game.Price![1..], NumberStyles.Any, CultureInfo.InvariantCulture, out var price);
                bool minPriceMatch = minPrice == null || game.Price != null && isPriceValid && price >= minPrice;
                bool maxPriceMatch = maxPrice == null || game.Price == "Free to Play" || game.Price != null && isPriceValid && price <= maxPrice;

                return titleMatch && genreMatch && releaseYearMatch && minPriceMatch && maxPriceMatch;
            }).ToArray();

            return new GameArray(filteredGames);
        }
    }
}
