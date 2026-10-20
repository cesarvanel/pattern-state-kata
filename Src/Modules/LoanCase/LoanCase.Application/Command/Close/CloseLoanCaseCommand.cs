using Shared.Application.Command;

namespace LoanCase.Application.Command.Close;

public record CloseLoanCaseCommand(Guid LoanCaseId)
    : ICommand<CloseLoanCaseResponse>;
