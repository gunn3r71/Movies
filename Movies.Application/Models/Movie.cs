namespace Movies.Application.Models
{
    public class Movie
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
        public required int YearOfRelease { get; set; }
        public List<string> Genres { get; private set; } = new();
    }
}
