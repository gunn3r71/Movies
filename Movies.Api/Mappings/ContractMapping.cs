using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mappings
{
    public static class ContractMapping
    {
        public static Movie MapToMovie(this CreateMovieRequest request)
        {
            var movie = new Movie
            {
                Description = request.Description,
                Title = request.Title,
                YearOfRelease = request.YearOfRelease
            };

            movie.Genres.AddRange(request.Genres);

            return movie;
        }

        public static MovieResponse MapToResponse(this Movie movie)
        {
            return new MovieResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                YearOfRelease = movie.YearOfRelease,
                Slug = movie.Slug,
                UserRating = movie.UserRating,
                Rating = movie.Rating,
                Genres = movie.Genres
            };
        }

        public static MoviesResponse MapToResponse(this IEnumerable<Movie> movies)
        {
            return new MoviesResponse() 
            {
                Items = movies.Select(x => x.MapToResponse()) 
            };
        }

        public static Movie MapToMovie(this UpdateMovieRequest request, Guid id)
        {
            var movie = new Movie(id)
            {
                Description = request.Description,
                Title = request.Title,
                YearOfRelease = request.YearOfRelease
            };

            movie.Genres.AddRange(request.Genres);

            return movie;
        }

        public static MovieRatingResponse MapToResponse(this MovieRating rating)
        {
            return new MovieRatingResponse()
            {
                Rating = rating.Rating,
                Slug = rating.Slug,
                MovieId = rating.MovieId
            };
        }

        public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> ratings) => 
            ratings.Select(MapToResponse);
    }
}
