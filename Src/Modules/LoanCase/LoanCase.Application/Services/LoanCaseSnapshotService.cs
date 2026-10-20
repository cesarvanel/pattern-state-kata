using LoanCase.Domain.Port.Out;
using LoanCase.Domain.Snapshot;

namespace LoanCase.Application.Services;

public sealed class LoanCaseSnapshotService(ILoanCaseSnapshotRepository snapshotRepository)

{
    public async Task ExecuteAsync(Domain.LoanCase loanCase,
        CancellationToken ct,
        Guid? createdBy = null)
    {
        LoanCaseStateSnapshot snapshot =
            loanCase.SaveState(Guid.NewGuid(), DateTime.UtcNow, createdBy);

        await snapshotRepository.AddAsync(snapshot, ct);
    }
}