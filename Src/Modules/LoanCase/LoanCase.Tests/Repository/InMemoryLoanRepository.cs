using LoanCase.Domain.Port.Out;

namespace LoanCase.Tests.Repository;

public class InMemoryLoanRepository : ILoanCaseRepository
{
    public Dictionary<Guid, Domain.LoanCase> LoanCases = new();

    public Task AddAsync(Domain.LoanCase loanCase)
    {
        LoanCases.Add(loanCase.Id, loanCase);
        return Task.CompletedTask;
    }
}