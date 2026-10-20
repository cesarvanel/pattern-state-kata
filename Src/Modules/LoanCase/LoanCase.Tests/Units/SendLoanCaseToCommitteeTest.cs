using LoanCase.Application.Command.SendToCommittee;
using LoanCase.Domain.Enum;
using LoanCase.Domain.Snapshot;
using LoanCase.Tests.Builder;
using LoanCase.Tests.Repository;

namespace LoanCase.Tests.Units;

public class SendLoanCaseToCommitteeTest
{
    private readonly LoanCaseGiven _given;
    private readonly Guid _loanCaseId = Guid.NewGuid();
    private readonly SendLoanCaseToCommitteeHandler _handler;
    private readonly InMemoryLoanRepository _loanRepository;
    private readonly InMemoryLoanCaseSnapshotRepository _snapshotRepository;

    public SendLoanCaseToCommitteeTest()
    {
        _loanRepository = new InMemoryLoanRepository();
        _snapshotRepository = new InMemoryLoanCaseSnapshotRepository();
        _handler = new SendLoanCaseToCommitteeHandler(
            _loanRepository,
            _snapshotRepository);
        _given = new LoanCaseGiven(_loanRepository, _snapshotRepository);
    }

    [Fact]
    public async Task SendToCommittee_When_UnderReview_Should_Transition_To_CommitteeReview()
    {
        await _given.AtStatusAsync(_loanCaseId, LoanCaseStatus.UnderReview);

        SendLoanCaseToCommitteeCommand command = new(_loanCaseId);
        SendLoanCaseToCommitteeResponse result =
            await _handler.HandleAsync(command, CancellationToken.None);

        Assert.True(result.IsSendToCommittee, result.Message);

        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);
        Assert.Equal(LoanCaseStatus.CommitteeReview, loanCase.Status);
        Assert.Equal(2, loanCase.Version);

        Assert.Equal(2, _snapshotRepository.StateSnapshots.Count);

        LoanCaseStateSnapshot lastSnapshot = _snapshotRepository.StateSnapshots.Last();
        Assert.Equal(loanCase.Id, lastSnapshot.LoanCaseId);
        Assert.Equal(LoanCaseStatus.CommitteeReview, lastSnapshot.Status);
        Assert.Equal(2, lastSnapshot.Version);
    }

    [Theory]
    [InlineData(LoanCaseStatus.Draft)]
    [InlineData(LoanCaseStatus.CommitteeReview)]
    [InlineData(LoanCaseStatus.Approved)]
    [InlineData(LoanCaseStatus.Rejected)]
    [InlineData(LoanCaseStatus.Closed)]
    public async Task SendToCommittee_Fails_When_LoanCase_State_Is_Not_UnderReview(
        LoanCaseStatus status)
    {
        await _given.AtStatusAsync(_loanCaseId, status);
        int snapshotCountBefore = _snapshotRepository.StateSnapshots.Count;

        SendLoanCaseToCommitteeCommand command = new(_loanCaseId);
        SendLoanCaseToCommitteeResponse result =
            await _handler.HandleAsync(command, CancellationToken.None);

        Assert.False(result.IsSendToCommittee, result.Message);
        Assert.NotEmpty(result.Message);

        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);
        Assert.Equal(status, loanCase.Status);
        Assert.Equal(1, loanCase.Version);
        Assert.Equal(snapshotCountBefore, _snapshotRepository.StateSnapshots.Count);
    }
}
