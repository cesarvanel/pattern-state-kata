using LoanCase.Domain.Enum;
using LoanCase.Domain.Vo;

namespace LoanCase.Domain.States;

public sealed class UnderReviewState : LoanCaseState
{
    public UnderReviewState(LoanCase loanCase) : base(loanCase)
    {
    }

    public override LoanCaseStatus Status => LoanCaseStatus.UnderReview;

    public override void Reject(RejectionDetails rejectionDetails)
    {
        _loanCase.RejectionDetails = rejectionDetails;
        _loanCase.TransitionTo(new RejectedState(_loanCase));
    }

    public override void SendToCommittee()
    {
        _loanCase.TransitionTo(new CommitteeReviewState(_loanCase));
    }
}
