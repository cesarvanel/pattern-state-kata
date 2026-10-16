using LoanCase.Domain.Enum;

namespace LoanCase.Domain.States;

public sealed class RejectedState : LoanCaseState
{
    public override LoanCaseStatus Status => LoanCaseStatus.Rejected;
}