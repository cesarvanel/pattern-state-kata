using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Port.Out;

using Shared.Domain.Exceptions;

namespace LoanCase.Application.Command.SendToCommittee;

public sealed class SendLoanCaseToCommitteeHandler(ILoanCaseRepository loanCaseRepository)
{
    public async Task<SendLoanCaseToCommitteeResponse> HandlAsync(
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

        await loanCaseRepository.UpdateAsync(loanCase, ct);
        response.IsSendToCommittee = true;

        return response;
    }
}