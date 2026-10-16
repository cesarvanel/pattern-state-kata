using LoanCase.Domain.Enum;

namespace LoanCase.Domain.States;

public sealed class UnderReviewState : LoanCaseState
{
    public override LoanCaseStatus Status => LoanCaseStatus.UnderReview;


    public override void Reject()
    {
        _loanCase.TransitionTo(new RejectedState());
    }

    public override void SendToCommittee()
    {
        _loanCase.TransitionTo(new CommitteeReviewState());
    }
}