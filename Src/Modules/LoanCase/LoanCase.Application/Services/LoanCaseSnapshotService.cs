using LoanCase.Domain.Port.Out;
using LoanCase.Domain.Snapshot;

namespace LoanCase.Application.Services;

public sealed class LoanCaseSnapshotService(ILoanCaseSnapshotRepository snapshotRepository)
    : ILoanCaseSnapshotService
{
    public async Task ExecuteAsync(Domain.LoanCase loanCase,
        Guid? createdBy = null,
        Guid? rolledBackFrom = null,
        CancellationToken ct = default)
    {
        LoanCaseStateSnapshot entry =
            loanCase.RecordState(DateTime.UtcNow, createdBy, rolledBackFrom);

        await SaveAsync(entry, ct);
    }

    public async Task SaveAsync(LoanCaseStateSnapshot entry, CancellationToken ct = default)
    {
        await snapshotRepository.AddAsync(entry, ct);
    }
}
