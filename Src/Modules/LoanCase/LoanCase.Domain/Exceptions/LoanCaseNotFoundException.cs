using LoanCase.Domain.Messages;

using Shared.Domain.Exceptions;

namespace LoanCase.Domain.Exceptions;

public class LoanCaseNotFoundException(string field)
    : DomainException($"{LoanCaseMessages.LoanCaseNotFound} {field}");