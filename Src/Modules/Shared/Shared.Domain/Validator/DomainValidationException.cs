using Shared.Domain.Exceptions;

namespace Shared.Domain.Validator;

public sealed class DomainValidationException(IEnumerable<string> errors)
    : DomainException(string.Join("; ", errors))
{
    public IReadOnlyCollection<string> Errors { get; } = [.. errors];
}