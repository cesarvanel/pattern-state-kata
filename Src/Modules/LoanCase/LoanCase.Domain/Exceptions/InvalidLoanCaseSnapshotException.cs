using Shared.Domain.Exceptions;

namespace LoanCase.Domain.Exceptions;

public sealed class InvalidLoanCaseSnapshotException(string reason) : DomainException(reason);