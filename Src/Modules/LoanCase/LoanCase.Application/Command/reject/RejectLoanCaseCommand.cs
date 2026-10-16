using Shared.Domain.Vo;

namespace LoanCase.Application.Command.reject;

public record RejectLoanCaseCommand(
    Guid LoanCaseId,
    string RejectReason,
    DocumentFile RejectDocument);