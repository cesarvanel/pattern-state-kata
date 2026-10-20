using LoanCase.Application.Command.Close;
using LoanCase.Domain.Enum;
using LoanCase.Domain.Snapshot;
using LoanCase.Tests.Builder;
using LoanCase.Tests.Repository;

namespace LoanCase.Tests.Units;

public class CloseLoanCaseTest
{
    private readonly LoanCaseGiven _given;
    private readonly CloseLoanCaseHandler _handler;
    private readonly Guid _loanCaseId = Guid.NewGuid();
    private readonly InMemoryLoanRepository _loanRepository;
    private readonly InMemoryLoanCaseSnapshotRepository _snapshotRepository;

    public CloseLoanCaseTest()
    {
        _loanRepository = new InMemoryLoanRepository();
        _snapshotRepository = new InMemoryLoanCaseSnapshotRepository();
        _handler = new CloseLoanCaseHandler(_loanRepository, _snapshotRepository);
        _given = new LoanCaseGiven(_loanRepository, _snapshotRepository);
    }

    [Fact]
    public async Task Close_When_Approved_Should_Transition_To_Closed()
    {
        await _given.AtStatusAsync(_loanCaseId, LoanCaseStatus.Approved);

        CloseLoanCaseResponse result =
            await _handler.HandleAsync(new CloseLoanCaseCommand(_loanCaseId));

        Assert.True(result.IsClosed, result.Message);

        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);
        Assert.Equal(LoanCaseStatus.Closed, loanCase.Status);
        Assert.Equal(2, loanCase.Version);

        Assert.Equal(2, _snapshotRepository.StateSnapshots.Count);

        LoanCaseStateSnapshot lastSnapshot = _snapshotRepository.StateSnapshots.Last();
        Assert.Equal(loanCase.Id, lastSnapshot.LoanCaseId);
        Assert.Equal(LoanCaseStatus.Closed, lastSnapshot.Status);
        Assert.Equal(2, lastSnapshot.Version);
    }

    [Theory]
    [InlineData(LoanCaseStatus.Draft)]
    [InlineData(LoanCaseStatus.UnderReview)]
    [InlineData(LoanCaseStatus.CommitteeReview)]
    [InlineData(LoanCaseStatus.Rejected)]
    [InlineData(LoanCaseStatus.Closed)]
    public async Task Close_Fails_When_LoanCase_State_Is_Not_Approved(
        LoanCaseStatus status)
    {
        await _given.AtStatusAsync(_loanCaseId, status);

        CloseLoanCaseResponse result =
            await _handler.HandleAsync(new CloseLoanCaseCommand(_loanCaseId));

        Assert.False(result.IsClosed, result.Message);
        Assert.NotEmpty(result.Message);

        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);
        Assert.Equal(status, loanCase.Status);
        Assert.Equal(1, loanCase.Version);
        Assert.Single(_snapshotRepository.StateSnapshots);
    }
}
