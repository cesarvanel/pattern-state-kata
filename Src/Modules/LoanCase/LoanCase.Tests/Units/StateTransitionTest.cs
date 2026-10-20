using LoanCase.Domain.Enum;
using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Vo;
using LoanCase.Tests.Builder;

using Shared.Domain.Vo;

namespace LoanCase.Tests.Units;

public class StateTransitionTest
{
    public enum LoanCaseAction
    {
        Submit,
        SendToCommittee,
        Reject,
        Approve,
        Close
    }

    private static readonly Dictionary<(LoanCaseStatus From, LoanCaseAction Action),
        LoanCaseStatus> AllowedTransitions = new()
    {
        [(LoanCaseStatus.Draft, LoanCaseAction.Submit)] = LoanCaseStatus.UnderReview,
        [(LoanCaseStatus.UnderReview, LoanCaseAction.SendToCommittee)] =
            LoanCaseStatus.CommitteeReview,
        [(LoanCaseStatus.UnderReview, LoanCaseAction.Reject)] = LoanCaseStatus.Rejected,
        [(LoanCaseStatus.CommitteeReview, LoanCaseAction.Approve)] = LoanCaseStatus.Approved,
        [(LoanCaseStatus.CommitteeReview, LoanCaseAction.Reject)] = LoanCaseStatus.Rejected,
        [(LoanCaseStatus.Approved, LoanCaseAction.Close)] = LoanCaseStatus.Closed
    };

    [Theory]
    [MemberData(nameof(AllowedTransitionCases))]
    public void Allowed_Transition_Should_Move_LoanCase_To_Expected_Status(
        LoanCaseStatus from,
        LoanCaseAction action,
        LoanCaseStatus expected)
    {
        Domain.LoanCase loanCase = Build(from);

        Apply(loanCase, action);

        Assert.Equal(expected, loanCase.Status);

        if (action == LoanCaseAction.Reject)
        {
            Assert.NotNull(loanCase.RejectionDetails);
        }
    }

    [Theory]
    [MemberData(nameof(ForbiddenTransitionCases))]
    public void Forbidden_Action_Should_Throw_And_Not_Mutate_LoanCase(
        LoanCaseStatus from,
        LoanCaseAction action)
    {
        Domain.LoanCase loanCase = Build(from);
        RejectionDetails? rejectionDetailsBefore = loanCase.RejectionDetails;
        int versionBefore = loanCase.Version;

        InvalidLoanCaseStateActionException exception =
            Assert.Throws<InvalidLoanCaseStateActionException>(() =>
                Apply(loanCase, action));

        Assert.NotEmpty(exception.Message);
        Assert.Equal(from, loanCase.Status);
        Assert.Equal(rejectionDetailsBefore, loanCase.RejectionDetails);
        Assert.Equal(versionBefore, loanCase.Version);
    }

    [Fact]
    public void Forbidden_Action_Should_Report_Action_And_State_In_Message()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.Draft);

        InvalidLoanCaseStateActionException exception =
            Assert.Throws<InvalidLoanCaseStateActionException>(() =>
                loanCase.Approve());

        Assert.Contains(nameof(Domain.LoanCase.Approve), exception.Message);
        Assert.Contains(nameof(LoanCaseStatus.Draft), exception.Message);
        Assert.Equal(LoanCaseStatus.Draft, loanCase.Status);
    }

    [Fact]
    public void Rehydrate_Should_Restore_LoanCase_With_Its_State_And_RejectionDetails()
    {
        Domain.LoanCase original = Build(LoanCaseStatus.Rejected);
        RejectionDetails? originalRejectionDetails = original.RejectionDetails;

        Domain.LoanCase rehydrated = Domain.LoanCase.Rehydrate(original.Id,
            original.BorrowerId,
            original.Number,
            original.CreatedAt,
            original.Amount,
            [.. original.AttachedDocuments],
            original.Status,
            originalRejectionDetails);

        Assert.Equal(original.Id, rehydrated.Id);
        Assert.Equal(LoanCaseStatus.Rejected, rehydrated.Status);
        Assert.NotNull(originalRejectionDetails);
        Assert.Equal(originalRejectionDetails, rehydrated.RejectionDetails);
        Assert.Equal(original.AttachedDocuments.Count, rehydrated.AttachedDocuments.Count);
    }

    public static IEnumerable<object[]> AllowedTransitionCases()
    {
        foreach (KeyValuePair<(LoanCaseStatus From, LoanCaseAction Action),
                     LoanCaseStatus> transition in AllowedTransitions)
        {
            yield return
            [
                transition.Key.From,
                transition.Key.Action,
                transition.Value
            ];
        }
    }

    public static IEnumerable<object[]> ForbiddenTransitionCases()
    {
        foreach (LoanCaseStatus status in Enum.GetValues<LoanCaseStatus>())
        {
            foreach (LoanCaseAction action in Enum.GetValues<LoanCaseAction>())
            {
                if (!AllowedTransitions.ContainsKey((status, action)))
                {
                    yield return [status, action];
                }
            }
        }
    }

    private static void Apply(Domain.LoanCase loanCase, LoanCaseAction action)
    {
        switch (action)
        {
            case LoanCaseAction.Submit:
                loanCase.Submit();
                break;

            case LoanCaseAction.SendToCommittee:
                loanCase.SendToCommittee();
                break;

            case LoanCaseAction.Reject:
                loanCase.Reject(CreateRejectionDetails());
                break;

            case LoanCaseAction.Approve:
                loanCase.Approve();
                break;

            case LoanCaseAction.Close:
                loanCase.Close();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }
    }

    private static Domain.LoanCase Build(LoanCaseStatus status)
    {
        return new LoanCaseSut().WithStatus(status).Build();
    }

    private static RejectionDetails CreateRejectionDetails()
    {
        return new RejectionDetails("Documents non conformes",
            DocumentFile.Create("Lettre_Rejet.pdf", 150, ".pdf"));
    }
}
