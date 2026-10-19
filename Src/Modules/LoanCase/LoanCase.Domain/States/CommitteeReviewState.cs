using LoanCase.Domain.Enum;
using LoanCase.Domain.Vo;

namespace LoanCase.Domain.States;

public sealed class CommitteeReviewState : LoanCaseState
{
    public CommitteeReviewState(LoanCase loanCase) : base(loanCase)
    {
    }

    public override LoanCaseStatus Status => LoanCaseStatus.CommitteeReview;

    public override void Approve()
    {
        _loanCase.TransitionTo(new ApprovedState(_loanCase));
    }

    public override void Reject(RejectionDetails rejectionDetails)
    {
        _loanCase.RejectionDetails = rejectionDetails;
        _loanCase.TransitionTo(new RejectedState(_loanCase));
    }
}
