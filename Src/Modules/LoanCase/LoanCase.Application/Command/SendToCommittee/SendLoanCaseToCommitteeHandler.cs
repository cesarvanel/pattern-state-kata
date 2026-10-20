using LoanCase.Application.Services;
using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Port.Out;

using Shared.Domain.Exceptions;

namespace LoanCase.Application.Command.SendToCommittee;

public sealed class SendLoanCaseToCommitteeHandler(
    ILoanCaseRepository loanCaseRepository,
    ILoanCaseSnapshotRepository snapshotRepository)
{
    public async Task<SendLoanCaseToCommitteeResponse> HandleAsync(
        SendLoanCaseToCommitteeCommand command,
        CancellationToken ct = default)
    {
        SendLoanCaseToCommitteeResponse response = new();

        Domain.LoanCase? loanCase;

        try
        {
            loanCase = await loanCaseRepository.GetByIdAsync(command.LoanCaseId, ct) ??
                       throw new LoanCaseNotFoundException(command.LoanCaseId.ToString());
            loanCase.SendToCommittee();
        }
        catch (DomainException e)
        {
            response.Message = e.Message;
            return response;
        }

        await new LoanCaseSnapshotService(snapshotRepository).ExecuteAsync(loanCase, ct);

        await loanCaseRepository.UpdateAsync(loanCase, ct);
        response.IsSendToCommittee = true;

        return response;
    }
}