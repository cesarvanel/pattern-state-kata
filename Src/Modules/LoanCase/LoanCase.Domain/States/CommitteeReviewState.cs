using LoanCase.Domain.Enum;

namespace LoanCase.Domain.States;

public sealed class CommitteeReviewState : LoanCaseState
{
    public override LoanCaseStatus Status => LoanCaseStatus.CommitteeReview;

    public override void Approve()
    {
        _loanCase.TransitionTo(new ApprovedState());
    }
}