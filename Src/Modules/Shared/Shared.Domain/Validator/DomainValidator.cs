namespace Shared.Domain.Validator;

public sealed class DomainValidator<T>
{
    private readonly List<string> _errors = [];

    public IReadOnlyCollection<string> Errors => _errors;
    public bool IsValid => _errors.Count == 0;

    public DomainValidator<T> Ensure(bool condition, string errorMessage)
    {
        if (!condition)
        {
            _errors.Add(errorMessage);
        }

        return this;
    }

    public void ThrowIfInvalid()
    {
        if (_errors.Count > 0)
        {
            throw new DomainValidationException(_errors);
        }
    }
}