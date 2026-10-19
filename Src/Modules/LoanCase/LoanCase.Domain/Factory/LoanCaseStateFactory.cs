using LoanCase.Domain.Enum;
using LoanCase.Domain.Messages;
using LoanCase.Domain.States;

namespace LoanCase.Domain.Factory;

public static class LoanCaseStateFactory
{
    public static LoanCaseState FromStatus(LoanCase loanCase, LoanCaseStatus status)
    {
        return status switch
        {
            LoanCaseStatus.Draft => new DraftState(loanCase),
            LoanCaseStatus.UnderReview => new UnderReviewState(loanCase),
            LoanCaseStatus.CommitteeReview => new CommitteeReviewState(loanCase),
            LoanCaseStatus.Approved => new ApprovedState(loanCase),
            LoanCaseStatus.Rejected => new RejectedState(loanCase),
            LoanCaseStatus.Closed => new ClosedState(loanCase),
            _ => throw new ArgumentOutOfRangeException(nameof(status),
                status,
                LoanCaseMessages.UnknownLoanCaseStatus)
        };
    }
}