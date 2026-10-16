using Shared.Domain.Exceptions;

namespace LoanCase.Domain.Exceptions;

public sealed class InvalidLoanCaseStateActionException(string reason) : DomainException(reason);