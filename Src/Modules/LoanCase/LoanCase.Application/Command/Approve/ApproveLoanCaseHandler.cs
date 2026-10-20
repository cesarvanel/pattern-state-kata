using LoanCase.Application.Services;
using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Port.Out;

using Shared.Domain.Exceptions;

namespace LoanCase.Application.Command.Approve;

public sealed class ApproveLoanCaseHandler(
    ILoanCaseRepository loanCaseRepository,
    ILoanCaseSnapshotRepository snapshotRepository)
{
    public async Task<ApproveLoanCaseResponse> HandleAsync(ApprovedLoanCaseCommand command,
        CancellationToken ct = default)
    {
        ApproveLoanCaseResponse response = new();

        Domain.LoanCase? loanCase;
        try
        {
            loanCase = await loanCaseRepository.GetByIdAsync(command.LoanCaseId, ct) ??
                       throw new LoanCaseNotFoundException(command.LoanCaseId.ToString());

            loanCase.Approve();
        }
        catch (DomainException e)
        {
            response.Message = e.Message;
            return response;
        }

        await new LoanCaseSnapshotService(snapshotRepository).ExecuteAsync(loanCase, ct);
        await loanCaseRepository.UpdateAsync(loanCase, ct);
        response.IsApproved = true;

        return response;
    }
}
