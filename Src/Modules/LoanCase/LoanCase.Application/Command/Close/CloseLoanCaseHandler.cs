using LoanCase.Application.Services;
using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Port.Out;

using Shared.Application.Command;
using Shared.Domain.Exceptions;

namespace LoanCase.Application.Command.Close;

public sealed class CloseLoanCaseHandler(
    ILoanCaseRepository loanCaseRepository,
    ILoanCaseSnapshotRepository snapshotRepository)
    : ICommandHandler<CloseLoanCaseCommand, CloseLoanCaseResponse>
{
    public async Task<CloseLoanCaseResponse> HandleAsync(CloseLoanCaseCommand command,
        CancellationToken ct = default)
    {
        CloseLoanCaseResponse response = new();

        Domain.LoanCase? loanCase;
        try
        {
            loanCase = await loanCaseRepository.GetByIdAsync(command.LoanCaseId, ct) ??
                       throw new LoanCaseNotFoundException(command.LoanCaseId.ToString());

            loanCase.Close();
        }
        catch (DomainException e)
        {
            response.Message = e.Message;
            return response;
        }

        await new LoanCaseSnapshotService(snapshotRepository).ExecuteAsync(loanCase, ct);
        await loanCaseRepository.UpdateAsync(loanCase, ct);
        response.IsClosed = true;

        return response;
    }
}
