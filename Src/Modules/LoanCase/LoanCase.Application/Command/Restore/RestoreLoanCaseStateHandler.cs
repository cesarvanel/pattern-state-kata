using LoanCase.Application.Services;
using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Port.Out;
using LoanCase.Domain.Snapshot;

using Shared.Application.Command;
using Shared.Domain.Exceptions;

namespace LoanCase.Application.Command.Restore;

public class RestoreLoanCaseStateHandler(
    ILoanCaseRepository loanCaseRepository,
    ILoanCaseSnapshotService snapshotService)
    : ICommandHandler<RestoreLoanCaseStateCommand, RestoreLoanCaseStateResponse>
{
    public async Task<RestoreLoanCaseStateResponse> HandleAsync(RestoreLoanCaseStateCommand command,
        CancellationToken ct = default)
    {
        RestoreLoanCaseStateResponse response = new();
        Domain.LoanCase? loanCase;
        LoanCaseStateSnapshot restoredEntry;

        try
        {
            loanCase = await loanCaseRepository.GetByIdAsync(command.LoanCaseId, ct) ??
                       throw new LoanCaseNotFoundException(command.LoanCaseId.ToString());

            restoredEntry = loanCase.Restore(command.Version, DateTime.UtcNow, command.CreatedBy);
        }
        catch (DomainException e)
        {
            response.Message = e.Message;
            return response;
        }

        await snapshotService.SaveAsync(restoredEntry, ct);

        await loanCaseRepository.UpdateAsync(loanCase, ct);

        response.IsRestored = true;
        response.RestoredVersion = restoredEntry.Version;

        return response;
    }
}
