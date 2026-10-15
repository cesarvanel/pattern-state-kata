using Shared.Domain.Exceptions;

namespace LoanCase.Domain.Exceptions;

public sealed class LoanCaseAttachedDocumentException(string message) : DomainException(message);