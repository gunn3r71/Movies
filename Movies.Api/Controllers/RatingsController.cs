using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mappings;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[Authorize]
[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingsController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [HttpPost(ApiEndpoints.Movies.Rate)]
    public async Task<IActionResult> RateMovieAsync([FromRoute] Guid id, [FromBody] RateMovieRequest request, CancellationToken cancellationToken = default)
    {
        var userId = HttpContext.GetUserId();
        
        if (userId is null)
            return Unauthorized();
        
        var result = await _ratingService.RateMovieAsync(id, userId.Value, request.Rating, cancellationToken);
        
        return result ? NoContent() : NotFound();
    }
    
    [HttpDelete(ApiEndpoints.Movies.Rate)]
    public async Task<IActionResult> DeleteRatingAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var userId = HttpContext.GetUserId();
        
        if (userId is null)
            return Unauthorized();
        
        var result = await _ratingService.DeleteRatingAsync(id, userId.Value, cancellationToken);
        
        return result ? NoContent() : BadRequest();
    }

    [HttpGet(ApiEndpoints.Ratings.GetUserRating)]
    public async Task<IActionResult> GetUserRatingsAsync(CancellationToken cancellationToken = default)
    {
        var userId = HttpContext.GetUserId();

        if (userId is null)
            return Unauthorized();

        var result = await _ratingService.GetRatingsByUserAsync(userId.Value, cancellationToken);

        return Ok(result.MapToResponse());
    }
}