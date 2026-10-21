using LoanCase.Domain.Port.Out;
using LoanCase.Domain.Snapshot;

namespace LoanCase.Application.Services;

public sealed class LoanCaseSnapshotService(ILoanCaseSnapshotRepository snapshotRepository)
    : ILoanCaseSnapshotService
{
    public async Task ExecuteAsync(Domain.LoanCase loanCase,
        CancellationToken ct = default,
        Guid? createdBy = null,
        Guid? rolledBackFrom = null)
    {
        LoanCaseStateSnapshot snapshot =
            loanCase.SaveState(Guid.NewGuid(), DateTime.UtcNow, createdBy, rolledBackFrom);

        await snapshotRepository.AddAsync(snapshot, ct);
    }
}
