using LoanCase.Application.Services;
using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Port.Out;

using Shared.Application.Command;
using Shared.Domain.Exceptions;

namespace LoanCase.Application.Command.Submit;

public sealed class SubmitLoanCaseHandler(
    ILoanCaseRepository loanCaseRepository,
    ILoanCaseSnapshotRepository snapshotRepository)
    : ICommandHandler<SubmitLoanCaseCommand, SubmitLoanCaseResponse>
{
    public async Task<SubmitLoanCaseResponse> HandleAsync(SubmitLoanCaseCommand command,
        CancellationToken ct = default)
    {
        SubmitLoanCaseResponse response = new();

        Domain.LoanCase? loanCase;

        try
        {
            loanCase = await loanCaseRepository.GetByIdAsync(command.LoanCaseId, ct) ??
                       throw new LoanCaseNotFoundException(command.LoanCaseId.ToString());
            loanCase.Submit();
        }
        catch (DomainException e)
        {
            response.Message = e.Message;
            return response;
        }

        await new LoanCaseSnapshotService(snapshotRepository).ExecuteAsync(loanCase, ct);
        await loanCaseRepository.UpdateAsync(loanCase, ct);

        response.IsSubmitted = true;

        return response;
    }
}