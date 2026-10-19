using LoanCase.Domain.Enum;

namespace LoanCase.Domain.States;

public sealed class RejectedState : LoanCaseState
{
    public RejectedState(LoanCase loanCase) : base(loanCase)
    {
    }

    public override LoanCaseStatus Status => LoanCaseStatus.Rejected;
}
