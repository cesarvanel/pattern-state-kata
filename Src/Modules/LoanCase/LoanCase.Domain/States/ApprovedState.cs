using LoanCase.Domain.Enum;

namespace LoanCase.Domain.States;

public sealed class ApprovedState : LoanCaseState
{
    public ApprovedState(LoanCase loanCase) : base(loanCase)
    {
    }

    public override LoanCaseStatus Status => LoanCaseStatus.Approved;

    public override void Close()
    {
        _loanCase.TransitionTo(new ClosedState(_loanCase));
    }
}
