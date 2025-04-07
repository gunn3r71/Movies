namespace Movies.Contracts.Requests
{
    public sealed class CreateMovieRequest
    {
        public required string Title { get; init; }
        public required string Description { get; init; }
        public required int YearOfRelease { get; init; }
        public required IEnumerable<string> Genres { get; init; } = [];
    }
}
