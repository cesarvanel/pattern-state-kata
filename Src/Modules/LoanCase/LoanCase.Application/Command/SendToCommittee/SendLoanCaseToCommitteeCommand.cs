using Shared.Application.Command;

namespace LoanCase.Application.Command.SendToCommittee;

public record SendLoanCaseToCommitteeCommand(Guid LoanCaseId)
    : ICommand<SendLoanCaseToCommitteeResponse>;
