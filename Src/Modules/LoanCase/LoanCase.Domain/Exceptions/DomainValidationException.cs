namespace LoanCase.Domain.Exceptions;

public sealed class DomainValidationException(IEnumerable<string> errors) 
    : Exception(string.Join("; ", errors))
{
    public IReadOnlyCollection<string> Errors { get; } = [.. errors];
}