using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Validators;

public class MovieValidator : AbstractValidator<Movie>
{
    private readonly IMoviesRepository _repository;
    
    public MovieValidator(IMoviesRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository, nameof(repository));
        
        _repository = repository;
        
        RuleFor(m => m.Title)
            .NotEmpty();
        
        RuleFor(m => m.Description)
            .NotEmpty();
        
        RuleFor(m => m.Genres)
            .NotEmpty();
        
        RuleFor(m => m.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(m => m.Slug)
            .MustAsync(IsValidSlugAsync)
            .WithMessage("Movie already exists.");
    }

    private async Task<bool> IsValidSlugAsync(Movie movie, string slug, CancellationToken cancellationToken = default)
    {
        var existingMovie = await _repository.GetBySlugAsync(slug, cancellationToken);
        
        return existingMovie is null || existingMovie.Id.Equals(movie.Id);
    }
}