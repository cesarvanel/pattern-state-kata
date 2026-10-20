using LoanCase.Application.Services;
using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Port.Out;
using LoanCase.Domain.Vo;

using Shared.Domain.Exceptions;

namespace LoanCase.Application.Command.reject;

public class RejectLoanCaseHandler(
    ILoanCaseRepository loanCaseRepository,
    ILoanCaseSnapshotRepository snapshotRepository)
{
    public async Task<RejectLoanCaseResponse> HandleAsync(RejectLoanCaseCommand command,
        CancellationToken ct = default)
    {
        RejectLoanCaseResponse response = new();

        Domain.LoanCase? loanCase;

        try
        {
            loanCase = await loanCaseRepository.GetByIdAsync(command.LoanCaseId, ct) ??
                       throw new LoanCaseNotFoundException(command.LoanCaseId.ToString());

            RejectionDetails rejectionDetails = new(command.RejectReason, command.RejectDocument);

            loanCase.Reject(rejectionDetails);
        }
        catch (DomainException e)
        {
            response.Message = e.Message;
            return response;
        }

        await new LoanCaseSnapshotService(snapshotRepository).ExecuteAsync(loanCase, ct);
        await loanCaseRepository.UpdateAsync(loanCase, ct);
        response.IsRejected = true;

        return response;
    }
}
