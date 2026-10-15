using LoanCase.Domain.Port.Out;

namespace LoanCase.Application.Command.Create;

public class CreateLoanCaseHandler(ILoanCaseRepository loanCaseRepository)
{
    public async Task<CreateLoanCaseResponse> HandleAsync(CreateLoanCaseCommand command)
    {
        CreateLoanCaseResponse response = new();
        Domain.LoanCase loanCase = Domain.LoanCase.Create(command.Id, command.BorrowerId, "DOSSIER-1-20026",
            createdAt: new DateTime(), amount: command.Amount, attachedDocuments: command.AttachedDocuments);

        await loanCaseRepository.AddAsync(loanCase);
        response.IsCreated = true;

        return await Task.FromResult(response);
    }
}