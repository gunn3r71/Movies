namespace Movies.Contracts.Responses;

public sealed class MovieRatingResponse
{
    public required Guid MovieId { get; init; }
    public required string Slug { get; init; }
    public required int Rating { get; init; }
}