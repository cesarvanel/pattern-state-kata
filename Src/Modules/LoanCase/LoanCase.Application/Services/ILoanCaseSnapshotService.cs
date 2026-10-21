namespace LoanCase.Application.Services;

public interface ILoanCaseSnapshotService
{
    Task ExecuteAsync(Domain.LoanCase loanCase,
        Guid? createdBy = null,
        Guid? rolledBackFrom = null,
        CancellationToken ct = default);

    Task SaveAsync(Domain.Snapshot.LoanCaseStateSnapshot entry, CancellationToken ct = default);
}
