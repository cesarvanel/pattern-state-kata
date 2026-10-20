using LoanCase.Domain.Snapshot;

namespace LoanCase.Domain.Port.Out;

public interface ILoanCaseSnapshotRepository
{
    Task AddAsync(LoanCaseStateSnapshot snapshot, CancellationToken ct = default);

    Task<LoanCaseStateSnapshot?> GetByVersionAsync(Guid loanCaseId,
        int version,
        CancellationToken ct = default);
}