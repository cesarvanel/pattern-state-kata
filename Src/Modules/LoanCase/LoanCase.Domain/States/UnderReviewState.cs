using LoanCase.Domain.Enum;

namespace LoanCase.Domain.States;

public sealed class UnderReviewState : LoanCaseState
{
    public override LoanCaseStatus Status => LoanCaseStatus.UnderReview;
}