using LoanCase.Domain.Enum;

namespace LoanCase.Domain.States;

public sealed class DraftState : LoanCaseState
{
    public DraftState(LoanCase loanCase) : base(loanCase)
    {
    }

    public override LoanCaseStatus Status => LoanCaseStatus.Draft;

    public override void Submit()
    {
        _loanCase.TransitionTo(new UnderReviewState(_loanCase));
    }
}
