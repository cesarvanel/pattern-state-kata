using LoanCase.Domain.Messages;

using Shared.Domain.Exceptions;

namespace LoanCase.Domain.Exceptions;

public class LoanCaseSnapshotNotFoundException(string field)
    : DomainException($"{LoanCaseMessages.LoanCaseSnapshotNotFound} {field}");