using LoanCase.Domain.Enum;

namespace LoanCase.Domain.States;

public sealed class ApprovedState : LoanCaseState
{
    public override LoanCaseStatus Status => LoanCaseStatus.Approved;
}