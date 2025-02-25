using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers
{
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesRepository _repository;

        public MoviesController(IMoviesRepository repository)
        {
            _repository = repository;
        }

        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var movie = await _repository.GetMovieAsync(id);

            if (movie is null)
                return NotFound();

            return Ok(movie.MapToResponse());
        }

        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetMoviesAsync()
        {
            var movies = await _repository.GetMoviesAsync();

            return Ok(movies.MapToResponse());
        }

        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> CreateMovieAsync([FromBody] CreateMovieRequest request)
        {
            var movie = request.MapToMovie();

            bool result = await _repository.CreateMovieAsync(movie);

            if (!result)
                return BadRequest();

            return CreatedAtAction(nameof(Get), new { id = movie.Id }, movie.MapToResponse());
        }

        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> UpdateMovieAsync([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
        {
            var movie = request.MapToMovie(id);

            bool result = await _repository.UpdateMovieAsync(movie);

            if (!result)
                return NotFound();

            return Ok(movie);
        }

        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> DeleteMovieAsync([FromRoute] Guid id)
        {
            bool result = await _repository.DeleteMovieAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}