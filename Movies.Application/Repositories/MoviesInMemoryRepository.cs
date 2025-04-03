using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public class MoviesInMemoryRepository : IMoviesRepository
    {
        private readonly List<Movie> _movies = [];

        public async Task<Movie?> GetMovieBySlugAsync(string slug)
        {
            return await Task.FromResult(_movies.SingleOrDefault(x => x.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase)));
        }

        public Task<bool> CreateMovieAsync(Movie movie)
        {
            _movies.Add(movie);

            return Task.FromResult(true);
        }

        public Task<bool> DeleteMovieAsync(Guid id)
        {
            var movieIndex = _movies.FindIndex(movie => movie.Id == id);

            if (movieIndex < 0)
                return Task.FromResult(false);

            _movies.RemoveAt(movieIndex);

            return Task.FromResult(true);
        }

        public Task<bool> ExistsAsync(Guid id) => 
            Task.FromResult(_movies.Any(x => x.Id.Equals(id)));

        public Task<Movie?> GetMovieAsync(Guid id) =>
            Task.FromResult(_movies.SingleOrDefault(movie => movie.Id == id));

        public Task<IEnumerable<Movie>> GetMoviesAsync() =>
            Task.FromResult(_movies.AsEnumerable());

        public Task<bool> UpdateMovieAsync(Movie movie)
        {
            var existingMovieIndex = _movies.FindIndex(m => m.Id == movie.Id);

            if (existingMovieIndex < 0)
                return Task.FromResult(false);

            _movies[existingMovieIndex] = movie;

            return Task.FromResult(true);
        }
    }
}
