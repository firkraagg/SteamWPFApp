using System.ComponentModel;

namespace GamesLibrary
{
    public class Game
    {
        public required string Title { get; set; }
        public string? Genre { get; set; }
        public string? Price { get; set; }
        public required string Released { get; set; }
        public required bool IsForFree { get; set; }
        public int? Metascore { get; set; }
    }
}
