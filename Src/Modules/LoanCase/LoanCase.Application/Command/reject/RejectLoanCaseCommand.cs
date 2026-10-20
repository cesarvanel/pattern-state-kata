using Shared.Application.Command;
using Shared.Domain.Vo;

namespace LoanCase.Application.Command.Reject;

public record RejectLoanCaseCommand(
    Guid LoanCaseId,
    string RejectReason,
    DocumentFile RejectDocument) : ICommand<RejectLoanCaseResponse>;
