using LoanCase.Domain.Port.Out;
using LoanCase.Domain.Services;

using Shared.Domain.Exceptions;

namespace LoanCase.Application.Command.Create;

public class CreateLoanCaseHandler(
    ILoanCaseRepository loanCaseRepository,
    IGenerateLoanCaseNumberService generateLoanCaseNumberService)
{
    public async Task<CreateLoanCaseResponse> HandleAsync(CreateLoanCaseCommand command)
    {
        CreateLoanCaseResponse response = new();
        Domain.LoanCase loanCase;

        try
        {
            string loanCaseNumber = await generateLoanCaseNumberService.Execute();
            loanCase = Domain.LoanCase.Create(command.Id, command.BorrowerId, loanCaseNumber,
                createdAt: new DateTime(), amount: command.Amount, attachedDocuments: command.AttachedDocuments);
        }
        catch (DomainException e)
        {
            response.Message = e.Message;
            return response;
        }

        await loanCaseRepository.AddAsync(loanCase);
        response.IsCreated = true;

        return await Task.FromResult(response);
    }
}