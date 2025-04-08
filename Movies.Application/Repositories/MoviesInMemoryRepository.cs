using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public class MoviesInMemoryService : IMoviesRepository
    {
        private readonly List<Movie> _movies = [];

        public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_movies.SingleOrDefault(x => x.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase)));
        }

        public Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
        {
            _movies.Add(movie);

            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var movieIndex = _movies.FindIndex(movie => movie.Id == id);

            if (movieIndex < 0)
                return Task.FromResult(false);

            _movies.RemoveAt(movieIndex);

            return Task.FromResult(true);
        }

        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) => 
            Task.FromResult(_movies.Any(x => x.Id.Equals(id)));

        public Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_movies.SingleOrDefault(movie => movie.Id == id));

        public Task<IEnumerable<Movie>> GetAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(_movies.AsEnumerable());

        public Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
        {
            var existingMovieIndex = _movies.FindIndex(m => m.Id == movie.Id);

            if (existingMovieIndex < 0)
                return Task.FromResult(false);

            _movies[existingMovieIndex] = movie;

            return Task.FromResult(true);
        }
    }
}
