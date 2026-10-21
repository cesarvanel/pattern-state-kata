using LoanCase.Application.Command.Submit;
using LoanCase.Domain.Enum;
using LoanCase.Domain.Snapshot;
using LoanCase.Tests.Builder;
using LoanCase.Tests.Repository;

namespace LoanCase.Tests.Units;

public class SubmitLoanCaseTest
{
    private readonly SubmitLoanCaseHandler _handler;
    private readonly Guid _loanCaseId = Guid.NewGuid();
    private readonly InMemoryLoanRepository _loanRepository;
    private readonly InMemoryLoanCaseSnapshotRepository _snapshotRepository;

    public SubmitLoanCaseTest()
    {
        LoanCaseTestContext context = new();
        _loanRepository = context.LoanCaseRepository;
        _snapshotRepository = context.SnapshotRepository;
        _handler = context.SubmitHandler;
    }


    [Fact]
    public async Task Submit_Should_Transit_LoanCase_From_Draft_To_UnderReview()
    {
        await BuildSut();

        SubmitLoanCaseCommand command = new(_loanCaseId);

        SubmitLoanCaseResponse result =
            await _handler.HandleAsync(command);

        Assert.True(result.IsSubmitted, result.Message);
        Domain.LoanCase loanCase = Assert.Single(_loanRepository.LoanCases.Values);

        Assert.Equal(LoanCaseStatus.UnderReview, loanCase.Status);

        Assert.NotEmpty(_snapshotRepository.StateSnapshots);

        LoanCaseStateSnapshot? existedSnapshot =
            _snapshotRepository.StateSnapshots.Last();

        Assert.NotNull(existedSnapshot);

        Assert.Equal(loanCase.Id, existedSnapshot.LoanCaseId);

        Assert.Equal(2, existedSnapshot.Version);
        Assert.Equal(2, loanCase.Version);
        Assert.Equal(LoanCaseStatus.UnderReview, existedSnapshot.Status);
    }

    [Fact]
    public async Task Submit_Fails_When_LoanCase_State_Is_Not_Draft()
    {
        await BuildSutWithUnderReview();

        SubmitLoanCaseCommand command = new(_loanCaseId);
        SubmitLoanCaseResponse result =
            await _handler.HandleAsync(command);

        Assert.False(result.IsSubmitted, result.Message);
    }

    private async Task BuildSut()
    {
        Domain.LoanCase loanCase = new LoanCaseSut().WithLoanCaseId(_loanCaseId).Build();

        await _loanRepository.AddAsync(loanCase);
        LoanCaseStateSnapshot snapshot = loanCase.RecordState(DateTime.UtcNow);
        await _snapshotRepository.AddAsync(snapshot, CancellationToken.None);
    }

    private async Task BuildSutWithUnderReview()
    {
        Domain.LoanCase loanCase = new LoanCaseSut()
            .WithLoanCaseId(_loanCaseId)
            .WithStatus(LoanCaseStatus.UnderReview)
            .Build();

        await _loanRepository.AddAsync(loanCase);
    }
}