using LoanCase.Domain.Port.Out;
using LoanCase.Domain.Snapshot;

namespace LoanCase.Tests.Repository;

public class InMemoryLoanCaseSnapshotRepository : ILoanCaseSnapshotRepository
{
    public readonly List<LoanCaseStateSnapshot> StateSnapshots = [];

    public Task AddAsync(LoanCaseStateSnapshot snapshot, CancellationToken ct = default)
    {
        StateSnapshots.Add(snapshot);
        return Task.CompletedTask;
    }

    public Task<LoanCaseStateSnapshot?> GetByVersionAsync(Guid loanCaseId,
        int version,
        CancellationToken ct = default)
    {
        return Task.FromResult(StateSnapshots.SingleOrDefault(s =>
            s.LoanCaseId == loanCaseId && s.Version == version));
    }
}