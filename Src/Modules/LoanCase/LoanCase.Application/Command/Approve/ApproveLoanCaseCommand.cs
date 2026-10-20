using Shared.Application.Command;

namespace LoanCase.Application.Command.Approve;

public record ApproveLoanCaseCommand(Guid LoanCaseId)
    : ICommand<ApproveLoanCaseResponse>;
