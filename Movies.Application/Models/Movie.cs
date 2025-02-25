namespace Movies.Application.Models
{
    public class Movie
    {
        public Movie()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required int YearOfRelease { get; set; }
    }
}
