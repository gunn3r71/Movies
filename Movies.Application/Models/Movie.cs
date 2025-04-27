using System.Text.RegularExpressions;

namespace Movies.Application.Models
{
    public partial class Movie
    {
        public Movie()
        {
            Id = Guid.NewGuid();
        }

        public Movie(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public string Slug => GenerateSlug();
        public required int YearOfRelease { get; set; }
        public byte? UserRating { get; set; }
        public float? Rating { get; set; }
        public List<string> Genres { get; private set; } = [];

        private string GenerateSlug()
        {
            var sluggedTitle = SlugRegex().Replace(Title, string.Empty)
                .Replace(' ', '-')
                .ToLower();

            return $"{sluggedTitle}-{YearOfRelease}";
        }

        [GeneratedRegex(@"[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 10)]
        private static partial Regex SlugRegex();
    }
}
