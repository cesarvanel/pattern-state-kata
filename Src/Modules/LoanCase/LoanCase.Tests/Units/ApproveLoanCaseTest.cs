using LoanCase.Application.Command.Approve;
using LoanCase.Domain.Enum;
using LoanCase.Domain.Snapshot;
using LoanCase.Tests.Builder;
using LoanCase.Tests.Repository;

namespace LoanCase.Tests.Units;

public class ApproveLoanCaseTest
{
    private readonly LoanCaseGiven _given;
    private readonly ApproveLoanCaseHandler _handler;
    private readonly Guid _loanCaseId = Guid.NewGuid();
    private readonly InMemoryLoanRepository _loanRepository;
    private readonly InMemoryLoanCaseSnapshotRepository _snapshotRepository;

    public ApproveLoanCaseTest()
    {
        _loanRepository = new InMemoryLoanRepository();
        _snapshotRepository = new InMemoryLoanCaseSnapshotRepository();
        _handler = new ApproveLoanCaseHandler(_loanRepository, _snapshotRepository);
        _given = new LoanCaseGiven(_loanRepository, _snapshotRepository);
    }

    [Fact]
    public async Task Approve_When_CommitteeReview_Should_Transition_To_Approved()
    {
        await _given.AtStatusAsync(_loanCaseId, LoanCaseStatus.CommitteeReview);

        ApprovedLoanCaseCommand command = new(_loanCaseId);
        ApproveLoanCaseResponse result = await _handler.HandleAsync(command);

        Assert.True(result.IsApproved, result.Message);

        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);
        Assert.Equal(LoanCaseStatus.Approved, loanCase.Status);
        Assert.Equal(2, loanCase.Version);

        Assert.Equal(2, _snapshotRepository.StateSnapshots.Count);

        LoanCaseStateSnapshot lastSnapshot = _snapshotRepository.StateSnapshots.Last();
        Assert.Equal(loanCase.Id, lastSnapshot.LoanCaseId);
        Assert.Equal(LoanCaseStatus.Approved, lastSnapshot.Status);
        Assert.Equal(2, lastSnapshot.Version);
    }

    [Theory]
    [InlineData(LoanCaseStatus.Draft)]
    [InlineData(LoanCaseStatus.UnderReview)]
    [InlineData(LoanCaseStatus.Approved)]
    [InlineData(LoanCaseStatus.Rejected)]
    [InlineData(LoanCaseStatus.Closed)]
    public async Task Approve_Fails_When_LoanCase_State_Is_Not_CommitteeReview(
        LoanCaseStatus status)
    {
        await _given.AtStatusAsync(_loanCaseId, status);

        ApprovedLoanCaseCommand command = new(_loanCaseId);
        ApproveLoanCaseResponse result = await _handler.HandleAsync(command);

        Assert.False(result.IsApproved, result.Message);
        Assert.NotEmpty(result.Message);

        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);
        Assert.Equal(status, loanCase.Status);
        Assert.Equal(1, loanCase.Version);
        Assert.Single(_snapshotRepository.StateSnapshots);
    }
}
