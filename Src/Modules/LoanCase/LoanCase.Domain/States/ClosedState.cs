using LoanCase.Domain.Enum;

namespace LoanCase.Domain.States;

public sealed class ClosedState : LoanCaseState
{
    public ClosedState(LoanCase loanCase) : base(loanCase)
    {
    }

    public override LoanCaseStatus Status => LoanCaseStatus.Closed;
}
