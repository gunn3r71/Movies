using FluentValidation;
using Movies.Contracts.Responses;

namespace Movies.Api.Middlewares;

public sealed class ValidationErrorMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationErrorMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var failureResult = new ValidationFailureResponse(ex.Errors.Select(x => 
                    new ValidationFailureMessage(x.PropertyName, x.ErrorMessage)));
            
            await context.Response.WriteAsJsonAsync(failureResult);
        }
    }
}