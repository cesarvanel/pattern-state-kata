using Shared.Application.Command;

namespace LoanCase.Application.Command.Submit;

public record SubmitLoanCaseCommand(Guid LoanCaseId)
    : ICommand<SubmitLoanCaseResponse>;
