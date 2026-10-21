using LoanCase.Application.Command.Restore;
using LoanCase.Domain.Enum;
using LoanCase.Domain.Messages;
using LoanCase.Domain.Snapshot;
using LoanCase.Tests.Builder;
using LoanCase.Tests.Repository;

namespace LoanCase.Tests.Units;

public class RestoreLoanCaseStateTest
{
    private readonly Guid _createdBy = Guid.NewGuid();
    private readonly RestoreLoanCaseStateHandler _handler;
    private readonly Guid _loanCaseId = Guid.NewGuid();
    private readonly InMemoryLoanRepository _loanRepository;
    private readonly InMemoryLoanCaseSnapshotRepository _snapshotRepository;

    public RestoreLoanCaseStateTest()
    {
        LoanCaseTestContext context = new();
        _loanRepository = context.LoanCaseRepository;
        _snapshotRepository = context.SnapshotRepository;
        _handler = context.RestoreHandler;
    }

    [Fact]
    public async Task Restore_Should_Roll_Back_LoanCase_To_Requested_Version()
    {
        List<LoanCaseStateSnapshot> history = await BuildHistoryAsync();

        RestoreLoanCaseStateCommand command = new(_loanCaseId, _createdBy, 1);
        RestoreLoanCaseStateResponse response =
            await _handler.HandleAsync(command, CancellationToken.None);

        Assert.True(response.IsRestored, response.Message);
        Assert.Equal(4, response.RestoredVersion);

        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);
        Assert.Equal(LoanCaseStatus.Draft, loanCase.Status);
        Assert.Equal(4, loanCase.Version);

        Assert.Equal(4, _snapshotRepository.StateSnapshots.Count);

        LoanCaseStateSnapshot restoredSnapshot = _snapshotRepository.StateSnapshots.Last();
        Assert.Equal(loanCase.Id, restoredSnapshot.LoanCaseId);
        Assert.Equal(LoanCaseStatus.Draft, restoredSnapshot.Status);
        Assert.Equal(4, restoredSnapshot.Version);
        Assert.Equal(_createdBy, restoredSnapshot.CreatedBy);
        Assert.Equal(history[0].Id, restoredSnapshot.RolledBackFrom);
    }

    [Fact]
    public async Task Restore_To_Latest_Version_Should_Keep_State_And_Append_Snapshot()
    {
        List<LoanCaseStateSnapshot> history = await BuildHistoryAsync();

        RestoreLoanCaseStateCommand command = new(_loanCaseId, _createdBy, 3);
        RestoreLoanCaseStateResponse response =
            await _handler.HandleAsync(command, CancellationToken.None);

        Assert.True(response.IsRestored, response.Message);
        Assert.Equal(4, response.RestoredVersion);

        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);
        Assert.Equal(LoanCaseStatus.CommitteeReview, loanCase.Status);
        Assert.Equal(4, loanCase.Version);

        Assert.Equal(4, _snapshotRepository.StateSnapshots.Count);

        LoanCaseStateSnapshot restoredSnapshot = _snapshotRepository.StateSnapshots.Last();
        Assert.Equal(LoanCaseStatus.CommitteeReview, restoredSnapshot.Status);
        Assert.Equal(4, restoredSnapshot.Version);
        Assert.Equal(history[2].Id, restoredSnapshot.RolledBackFrom);
    }

    [Fact]
    public async Task Restore_Fails_When_LoanCase_Does_Not_Exist()
    {
        RestoreLoanCaseStateCommand command = new(Guid.NewGuid(), _createdBy, 1);
        RestoreLoanCaseStateResponse response =
            await _handler.HandleAsync(command, CancellationToken.None);

        Assert.False(response.IsRestored);
        Assert.Contains(LoanCaseMessages.LoanCaseNotFound, response.Message);

        Assert.Empty(_loanRepository.LoanCases);
        Assert.Empty(_snapshotRepository.StateSnapshots);
    }

    [Fact]
    public async Task Restore_Fails_When_Snapshot_Version_Does_Not_Exist()
    {
        await BuildHistoryAsync();

        RestoreLoanCaseStateCommand command = new(_loanCaseId, _createdBy, 99);
        RestoreLoanCaseStateResponse response =
            await _handler.HandleAsync(command, CancellationToken.None);

        Assert.False(response.IsRestored);
        Assert.Equal(0, response.RestoredVersion);
        Assert.Contains(LoanCaseMessages.LoanCaseSnapshotNotFound, response.Message);

        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);
        Assert.Equal(LoanCaseStatus.CommitteeReview, loanCase.Status);
        Assert.Equal(3, loanCase.Version);
        Assert.Equal(3, _snapshotRepository.StateSnapshots.Count);
    }

    [Fact]
    public async Task Restore_Fails_When_Current_State_Is_Closed()
    {
        await new LoanCaseGiven(_loanRepository, _snapshotRepository)
            .AtStatusAsync(_loanCaseId, LoanCaseStatus.Closed);

        RestoreLoanCaseStateCommand command = new(_loanCaseId, _createdBy, 1);
        RestoreLoanCaseStateResponse response =
            await _handler.HandleAsync(command, CancellationToken.None);

        Assert.False(response.IsRestored);
        Assert.Contains(nameof(Domain.LoanCase.Restore), response.Message);
        Assert.Contains(nameof(LoanCaseStatus.Closed), response.Message);

        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);
        Assert.Equal(LoanCaseStatus.Closed, loanCase.Status);
        Assert.Equal(1, loanCase.Version);
        Assert.Single(_snapshotRepository.StateSnapshots);
    }

    private async Task<List<LoanCaseStateSnapshot>> BuildHistoryAsync()
    {
        Domain.LoanCase loanCase = new LoanCaseSut().WithLoanCaseId(_loanCaseId).Build();

        LoanCaseStateSnapshot draftSnapshot = loanCase.RecordState(DateTime.UtcNow);

        loanCase.Submit();
        LoanCaseStateSnapshot underReviewSnapshot = loanCase.RecordState(DateTime.UtcNow);

        loanCase.SendToCommittee();
        LoanCaseStateSnapshot committeeReviewSnapshot = loanCase.RecordState(DateTime.UtcNow);

        await _loanRepository.AddAsync(loanCase);
        await _snapshotRepository.AddAsync(draftSnapshot, CancellationToken.None);
        await _snapshotRepository.AddAsync(underReviewSnapshot, CancellationToken.None);
        await _snapshotRepository.AddAsync(committeeReviewSnapshot, CancellationToken.None);

        return [draftSnapshot, underReviewSnapshot, committeeReviewSnapshot];
    }
}
