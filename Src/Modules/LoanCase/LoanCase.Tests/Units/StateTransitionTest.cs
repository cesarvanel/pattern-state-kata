using LoanCase.Domain.Enum;
using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Vo;
using LoanCase.Tests.Builder;

using Shared.Domain.Vo;

namespace LoanCase.Tests.Units;

public class StateTransitionTest
{
    [Fact]
    public void Submit_From_Draft_Should_Transition_To_UnderReview()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.Draft);

        loanCase.Submit();

        Assert.Equal(LoanCaseStatus.UnderReview, loanCase.Status);
    }

    [Fact]
    public void Reject_From_Draft_Should_Throw_And_Not_Mutate_LoanCase()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.Draft);

        Assert.Throws<InvalidLoanCaseStateActionException>(() =>
            loanCase.Reject(CreateRejectionDetails()));

        Assert.Equal(LoanCaseStatus.Draft, loanCase.Status);
        Assert.Null(loanCase.RejectionDetails);
    }

    [Fact]
    public void SendToCommittee_From_Draft_Should_Throw()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.Draft);

        Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.SendToCommittee());
        Assert.Equal(LoanCaseStatus.Draft, loanCase.Status);
    }

    [Fact]
    public void Approve_From_Draft_Should_Throw_With_Contextual_Message()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.Draft);

        InvalidLoanCaseStateActionException exception =
            Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.Approve());

        Assert.Contains(nameof(Domain.LoanCase.Approve), exception.Message);
        Assert.Contains(nameof(LoanCaseStatus.Draft), exception.Message);
        Assert.Equal(LoanCaseStatus.Draft, loanCase.Status);
    }

    [Fact]
    public void Close_From_Draft_Should_Throw()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.Draft);

        Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.Close());
        Assert.Equal(LoanCaseStatus.Draft, loanCase.Status);
    }

    [Fact]
    public void SendToCommittee_From_UnderReview_Should_Transition_To_CommitteeReview()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.UnderReview);

        loanCase.SendToCommittee();

        Assert.Equal(LoanCaseStatus.CommitteeReview, loanCase.Status);
    }

    [Fact]
    public void Reject_From_UnderReview_Should_Transition_To_Rejected_With_Details()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.UnderReview);
        RejectionDetails rejectionDetails = CreateRejectionDetails();

        loanCase.Reject(rejectionDetails);

        Assert.Equal(LoanCaseStatus.Rejected, loanCase.Status);
        Assert.Equal(rejectionDetails, loanCase.RejectionDetails);
    }

    [Fact]
    public void Submit_From_UnderReview_Should_Throw()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.UnderReview);

        Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.Submit());
        Assert.Equal(LoanCaseStatus.UnderReview, loanCase.Status);
    }

    [Fact]
    public void Approve_From_UnderReview_Should_Throw()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.UnderReview);

        Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.Approve());
        Assert.Equal(LoanCaseStatus.UnderReview, loanCase.Status);
    }

    [Fact]
    public void Close_From_UnderReview_Should_Throw()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.UnderReview);

        Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.Close());
        Assert.Equal(LoanCaseStatus.UnderReview, loanCase.Status);
    }

    [Fact]
    public void Approve_From_CommitteeReview_Should_Transition_To_Approved()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.CommitteeReview);

        loanCase.Approve();

        Assert.Equal(LoanCaseStatus.Approved, loanCase.Status);
    }

    [Fact]
    public void Reject_From_CommitteeReview_Should_Transition_To_Rejected_With_Details()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.CommitteeReview);
        RejectionDetails rejectionDetails = CreateRejectionDetails();

        loanCase.Reject(rejectionDetails);

        Assert.Equal(LoanCaseStatus.Rejected, loanCase.Status);
        Assert.Equal(rejectionDetails, loanCase.RejectionDetails);
    }

    [Fact]
    public void Submit_From_CommitteeReview_Should_Throw()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.CommitteeReview);

        Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.Submit());
        Assert.Equal(LoanCaseStatus.CommitteeReview, loanCase.Status);
    }

    [Fact]
    public void SendToCommittee_From_CommitteeReview_Should_Throw()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.CommitteeReview);

        Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.SendToCommittee());
        Assert.Equal(LoanCaseStatus.CommitteeReview, loanCase.Status);
    }

    [Fact]
    public void Close_From_CommitteeReview_Should_Throw()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.CommitteeReview);

        Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.Close());
        Assert.Equal(LoanCaseStatus.CommitteeReview, loanCase.Status);
    }

    [Fact]
    public void Close_From_Approved_Should_Transition_To_Closed()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.Approved);

        loanCase.Close();

        Assert.Equal(LoanCaseStatus.Closed, loanCase.Status);
    }

    [Fact]
    public void Submit_From_Approved_Should_Throw()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.Approved);

        Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.Submit());
        Assert.Equal(LoanCaseStatus.Approved, loanCase.Status);
    }

    [Fact]
    public void SendToCommittee_From_Approved_Should_Throw()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.Approved);

        Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.SendToCommittee());
        Assert.Equal(LoanCaseStatus.Approved, loanCase.Status);
    }

    [Fact]
    public void Reject_From_Approved_Should_Throw_And_Not_Mutate_RejectionDetails()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.Approved);

        Assert.Throws<InvalidLoanCaseStateActionException>(() =>
            loanCase.Reject(CreateRejectionDetails()));

        Assert.Equal(LoanCaseStatus.Approved, loanCase.Status);
        Assert.Null(loanCase.RejectionDetails);
    }

    [Fact]
    public void Approve_From_Approved_Should_Throw()
    {
        Domain.LoanCase loanCase = Build(LoanCaseStatus.Approved);

        Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.Approve());
        Assert.Equal(LoanCaseStatus.Approved, loanCase.Status);
    }

    [Theory]
    [InlineData(LoanCaseStatus.Rejected)]
    [InlineData(LoanCaseStatus.Closed)]
    public void All_Actions_From_Terminal_State_Should_Throw(LoanCaseStatus status)
    {
        Domain.LoanCase loanCase = Build(status);

        Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.Submit());
        Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.SendToCommittee());
        Assert.Throws<InvalidLoanCaseStateActionException>(() =>
            loanCase.Reject(CreateRejectionDetails()));
        Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.Approve());
        Assert.Throws<InvalidLoanCaseStateActionException>(() => loanCase.Close());

        Assert.Equal(status, loanCase.Status);
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
