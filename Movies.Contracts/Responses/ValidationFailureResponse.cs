namespace Movies.Contracts.Responses;

public sealed record ValidationFailureResponse(IEnumerable<ValidationFailureMessage> Errors);

public sealed record ValidationFailureMessage(string PropertyName, string ErrorMessage);