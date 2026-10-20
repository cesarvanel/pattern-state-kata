using LoanCase.Application.Services;
using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Port.Out;
using LoanCase.Domain.Snapshot;

using Shared.Domain.Exceptions;

namespace LoanCase.Application.Command.Restore;

public class RestoreLoanCaseStateHandler(
    ILoanCaseRepository loanCaseRepository,
    ILoanCaseSnapshotRepository loanCaseSnapshotRepository)
{
    public async Task<RestoreLoanCaseStateResponse> HandleAsync(RestoreLoanCaseStateCommand command,
        CancellationToken ct = default)
    {
        RestoreLoanCaseStateResponse response = new();
        Domain.LoanCase? loanCase;
        Guid restoredFromSnapshotId;

        try
        {
            loanCase = await loanCaseRepository.GetByIdAsync(command.LoanCaseId, ct) ??
                       throw new LoanCaseNotFoundException(command.LoanCaseId.ToString());

            LoanCaseStateSnapshot loanCaseSnapshot =
                await loanCaseSnapshotRepository.GetByVersionAsync(command.LoanCaseId,
                    command.Version,
                    ct) ?? throw new LoanCaseSnapshotNotFoundException(
                    $"{command.LoanCaseId}:{command.Version}");
            loanCase.Restore(loanCaseSnapshot);
            restoredFromSnapshotId = loanCaseSnapshot.Id;
        }
        catch (DomainException e)
        {
            response.Message = e.Message;
            return response;
        }

        await new LoanCaseSnapshotService(loanCaseSnapshotRepository).ExecuteAsync(loanCase,
            ct,
            command.CreatedBy,
            restoredFromSnapshotId);

        await loanCaseRepository.UpdateAsync(loanCase, ct);

        response.IsRestored = true;
        response.RestoredVersion = loanCase.Version;

        return response;
    }
}