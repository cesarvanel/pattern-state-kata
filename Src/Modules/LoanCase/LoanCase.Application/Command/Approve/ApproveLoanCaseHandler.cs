using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Port.Out;

using Shared.Domain.Exceptions;

namespace LoanCase.Application.Command.Approve;

public sealed class ApproveLoanCaseHandler(ILoanCaseRepository loanCaseRepository)
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

        response.IsApproved = true;
        await loanCaseRepository.UpdateAsync(loanCase, ct);

        return response;
    }
}