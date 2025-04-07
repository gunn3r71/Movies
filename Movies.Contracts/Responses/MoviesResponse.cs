namespace Movies.Contracts.Responses
{
    public sealed class MoviesResponse
    {
        public required IEnumerable<MovieResponse> Items { get; init; }
    }
}
