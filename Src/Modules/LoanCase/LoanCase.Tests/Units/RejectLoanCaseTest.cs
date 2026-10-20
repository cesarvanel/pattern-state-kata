using LoanCase.Application.Command.reject;
using LoanCase.Domain.Enum;
using LoanCase.Domain.Vo;
using LoanCase.Tests.Builder;
using LoanCase.Tests.Repository;

using Shared.Domain.Vo;

namespace LoanCase.Tests.Units;

public class RejectLoanCaseTest
{
    private readonly LoanCaseGiven _given;
    private readonly Guid _loanCaseId = Guid.NewGuid();
    private readonly InMemoryLoanRepository _loanRepository;
    private readonly RejectLoanCaseHandler _handler;
    private readonly InMemoryLoanCaseSnapshotRepository _snapshotRepository;

    private const string RejectReason = "Document non conforme";

    public RejectLoanCaseTest()
    {
        _loanRepository = new InMemoryLoanRepository();
        _snapshotRepository = new InMemoryLoanCaseSnapshotRepository();
        _handler = new RejectLoanCaseHandler(_loanRepository);
        _given = new LoanCaseGiven(_loanRepository, _snapshotRepository);
    }

    [Theory]
    [InlineData(LoanCaseStatus.UnderReview)]
    [InlineData(LoanCaseStatus.CommitteeReview)]
    public async Task Reject_When_Allowed_State_Should_Transition_To_Rejected(
        LoanCaseStatus status)
    {
        await _given.AtStatusAsync(_loanCaseId, status);

        RejectLoanCaseResponse response = await _handler.HandleAsync(CreateCommand(),
            CancellationToken.None);

        Assert.True(response.IsRejected, response.Message);

        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);
        Assert.Equal(LoanCaseStatus.Rejected, loanCase.Status);

        RejectionDetails? rejectionDetails = loanCase.RejectionDetails;
        Assert.NotNull(rejectionDetails);
        Assert.Equal(RejectReason, rejectionDetails.RejectReason);
        Assert.NotNull(rejectionDetails.DocumentFile);
    }

    [Theory]
    [InlineData(LoanCaseStatus.Draft)]
    [InlineData(LoanCaseStatus.Approved)]
    [InlineData(LoanCaseStatus.Rejected)]
    [InlineData(LoanCaseStatus.Closed)]
    public async Task Reject_Fails_When_LoanCase_State_Is_Not_Allowed(
        LoanCaseStatus status)
    {
        await _given.AtStatusAsync(_loanCaseId, status);

        int snapshotCountBefore = _snapshotRepository.StateSnapshots.Count;
        int versionBefore = await VersionOfLoanCase();
        RejectionDetails? rejectionDetailsBefore =
            await RejectionDetailsOfLoanCase();

        RejectLoanCaseResponse response = await _handler.HandleAsync(CreateCommand(),
            CancellationToken.None);

        Assert.False(response.IsRejected, response.Message);
        Assert.NotEmpty(response.Message);

        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);
        Assert.Equal(status, loanCase.Status);
        Assert.Equal(versionBefore, loanCase.Version);
        Assert.Equal(rejectionDetailsBefore, loanCase.RejectionDetails);
        Assert.Equal(snapshotCountBefore, _snapshotRepository.StateSnapshots.Count);
    }

    private RejectLoanCaseCommand CreateCommand()
    {
        return new RejectLoanCaseCommand(_loanCaseId,
            RejectReason,
            DocumentFile.Create("Documents/rejected", 150, ".pdf"));
    }

    private async Task<int> VersionOfLoanCase()
    {
        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);
        return loanCase.Version;
    }

    private async Task<RejectionDetails?> RejectionDetailsOfLoanCase()
    {
        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);
        return loanCase.RejectionDetails;
    }
}
