using LoanCase.Domain.Port.Out;

namespace LoanCase.Tests.Repository;

public class InMemoryLoanRepository : ILoanCaseRepository
{
    public readonly Dictionary<Guid, Domain.LoanCase> LoanCases = new();

    public Task AddAsync(Domain.LoanCase loanCase)
    {
        LoanCases.Add(loanCase.Id, loanCase);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsByNumberAsync(string loanCaseNumber)
    {
        bool result = LoanCases.Values.Any(loanCase =>
            string.Equals(loanCase.Number, loanCaseNumber, StringComparison.Ordinal));
        return Task.FromResult(result);
    }
}