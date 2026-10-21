using System.Collections.Immutable;

using LoanCase.Domain.Enum;
using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Messages;
using LoanCase.Domain.Snapshot;
using LoanCase.Tests.Builder;

namespace LoanCase.Tests.Units;

public class LoanCaseStateHistoryTest
{
    private readonly Guid _loanCaseId = Guid.NewGuid();

    [Fact]
    public void RecordState_Should_Append_Entries_With_A_Continuous_Sequence()
    {
        Domain.LoanCase loanCase = new LoanCaseSut().WithLoanCaseId(_loanCaseId).Build();

        LoanCaseStateSnapshot first = loanCase.RecordState(DateTime.UtcNow);
        LoanCaseStateSnapshot second = loanCase.RecordState(DateTime.UtcNow);
        LoanCaseStateSnapshot third = loanCase.RecordState(DateTime.UtcNow);

        Assert.Equal(1, first.Version);
        Assert.Equal(2, second.Version);
        Assert.Equal(3, third.Version);
        Assert.Equal(3, loanCase.Version);
        Assert.Equal(3, loanCase.StateHistory.Count);
    }

    [Fact]
    public void RecordState_Should_Capture_The_Current_State_Of_The_LoanCase()
    {
        Domain.LoanCase loanCase = new LoanCaseSut().WithLoanCaseId(_loanCaseId).Build();

        loanCase.Submit();
        LoanCaseStateSnapshot entry = loanCase.RecordState(DateTime.UtcNow);

        Assert.Equal(LoanCaseStatus.UnderReview, entry.Status);
        Assert.Equal(loanCase.Number, entry.Number);
        Assert.Equal(loanCase.Amount.Value, entry.Amount);
        Assert.Equal(loanCase.BorrowerId, entry.BorrowerId);
        Assert.Equal(loanCase.AttachedDocuments.Count, entry.AttachedDocuments.Length);
    }

    [Fact]
    public void Restore_Should_Append_The_Rollback_To_The_History_And_Keep_The_Trace()
    {
        Domain.LoanCase loanCase = new LoanCaseSut().WithLoanCaseId(_loanCaseId).Build();

        LoanCaseStateSnapshot draftEntry = loanCase.RecordState(DateTime.UtcNow);
        loanCase.Submit();
        LoanCaseStateSnapshot underReviewEntry = loanCase.RecordState(DateTime.UtcNow);

        LoanCaseStateSnapshot restoredEntry = loanCase.Restore(draftEntry.Version,
            DateTime.UtcNow,
            Guid.NewGuid());

        Assert.Equal(3, restoredEntry.Version);
        Assert.Equal(LoanCaseStatus.Draft, restoredEntry.Status);
        Assert.Equal(draftEntry.Id, restoredEntry.RolledBackFrom);

        Assert.Equal(3, loanCase.StateHistory.Count);
        Assert.Equal(LoanCaseStatus.Draft, loanCase.Status);
        Assert.Equal(3, loanCase.Version);

        Assert.Equal(draftEntry.Version, loanCase.StateHistory.First().Version);
        Assert.Equal(underReviewEntry.Version, loanCase.StateHistory.Last(x => x != restoredEntry).Version);
    }

    [Fact]
    public void Restore_Should_Fail_When_The_Target_Version_Does_Not_Exist()
    {
        Domain.LoanCase loanCase = new LoanCaseSut().WithLoanCaseId(_loanCaseId).Build();

        loanCase.RecordState(DateTime.UtcNow);

        LoanCaseSnapshotNotFoundException exception =
            Assert.Throws<LoanCaseSnapshotNotFoundException>(() =>
                loanCase.Restore(99, DateTime.UtcNow, Guid.NewGuid()));

        Assert.Contains(LoanCaseMessages.LoanCaseSnapshotNotFound, exception.Message);
        Assert.Equal(LoanCaseStatus.Draft, loanCase.Status);
        Assert.Single(loanCase.StateHistory);
    }

    [Fact]
    public void Rehydrate_Should_Fail_When_The_History_Has_A_Sequence_Gap()
    {
        LoanCaseStateSnapshot entry = new(Guid.NewGuid(),
            _loanCaseId,
            Guid.NewGuid(),
            LoanCaseStatus.Draft,
            "DOSSIER-2026-0001",
            1000m,
            DateTime.UtcNow,
            DateTime.UtcNow,
            null,
            2,
            null,
            ImmutableArray<Domain.Vo.AttachedDocument>.Empty,
            null);

        InvalidLoanCaseSnapshotException exception = Assert.Throws<InvalidLoanCaseSnapshotException>(
            () => Domain.LoanCase.Rehydrate(_loanCaseId,
                Guid.NewGuid(),
                "DOSSIER-2026-0001",
                DateTime.UtcNow,
                new Domain.Vo.Amount(1000m),
                [],
                LoanCaseStatus.Draft,
                null,
                [entry]));

        Assert.Contains("séquence", exception.Message);
    }

    [Fact]
    public void Rehydrate_Should_Fail_When_The_Current_Status_Does_Not_Match_The_Last_Entry()
    {
        LoanCaseStateSnapshot entry = new(Guid.NewGuid(),
            _loanCaseId,
            Guid.NewGuid(),
            LoanCaseStatus.Draft,
            "DOSSIER-2026-0001",
            1000m,
            DateTime.UtcNow,
            DateTime.UtcNow,
            null,
            1,
            null,
            ImmutableArray<Domain.Vo.AttachedDocument>.Empty,
            null);

        InvalidLoanCaseSnapshotException exception = Assert.Throws<InvalidLoanCaseSnapshotException>(
            () => Domain.LoanCase.Rehydrate(_loanCaseId,
                Guid.NewGuid(),
                "DOSSIER-2026-0001",
                DateTime.UtcNow,
                new Domain.Vo.Amount(1000m),
                [],
                LoanCaseStatus.UnderReview,
                null,
                [entry]));

        Assert.Contains("dernière version enregistrée", exception.Message);
    }

    [Fact]
    public void Rehydrate_Should_Fail_When_The_History_Belongs_To_Another_LoanCase()
    {
        LoanCaseStateSnapshot entry = new(Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            LoanCaseStatus.Draft,
            "DOSSIER-2026-0001",
            1000m,
            DateTime.UtcNow,
            DateTime.UtcNow,
            null,
            1,
            null,
            ImmutableArray<Domain.Vo.AttachedDocument>.Empty,
            null);

        Assert.Throws<InvalidLoanCaseSnapshotException>(() => Domain.LoanCase.Rehydrate(_loanCaseId,
            Guid.NewGuid(),
            "DOSSIER-2026-0001",
            DateTime.UtcNow,
            new Domain.Vo.Amount(1000m),
            [],
            LoanCaseStatus.Draft,
            null,
            [entry]));
    }
}
