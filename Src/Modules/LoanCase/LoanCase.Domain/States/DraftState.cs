using LoanCase.Domain.Enum;

namespace LoanCase.Domain.States;

public sealed class DraftState : LoanCaseState
{
    public override LoanCaseStatus Status => LoanCaseStatus.Draft;

    public override void Submit()
    {
        _loanCase.TransitionTo(new UnderReviewState());
    }
}