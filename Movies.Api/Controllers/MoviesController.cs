using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesService _service;

        public MoviesController(IMoviesService service)
        {
            _service = service;
        }

        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute] string identifier, CancellationToken cancellationToken = default)
        {
            var movie = Guid.TryParse(identifier, out var id) 
                ? await _service.GetByIdAsync(id, cancellationToken)
                : await _service.GetBySlugAsync(identifier, cancellationToken);

            if (movie is null)
                return NotFound();

            return Ok(movie.MapToResponse());
        }

        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetMoviesAsync(CancellationToken cancellationToken = default)
        {
            var movies = await _service.GetAsync(cancellationToken);

            return Ok(movies.MapToResponse());
        }

        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> CreateMovieAsync([FromBody] CreateMovieRequest request, CancellationToken cancellationToken = default)
        {
            var movie = request.MapToMovie();

            bool result = await _service.CreateAsync(movie, cancellationToken);

            if (!result)
                return BadRequest();

            return CreatedAtAction(nameof(Get), new { identifier = movie.Id.ToString() }, movie.MapToResponse());
        }

        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> UpdateMovieAsync([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken cancellationToken = default)
        {
            var movie = request.MapToMovie(id);

            var result = await _service.UpdateAsync(movie, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(movie);
        }

        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> DeleteMovieAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            bool result = await _service.DeleteAsync(id, cancellationToken);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}