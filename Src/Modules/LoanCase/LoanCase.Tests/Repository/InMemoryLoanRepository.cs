using LoanCase.Domain.Port.Out;

namespace LoanCase.Tests.Repository;

public class InMemoryLoanRepository : ILoanCaseRepository
{
    public readonly Dictionary<Guid, Domain.LoanCase> LoanCases = new();

    public Task AddAsync(Domain.LoanCase loanCase, CancellationToken ct = default)
    {
        LoanCases.Add(loanCase.Id, loanCase);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsByNumberAsync(string loanCaseNumber, CancellationToken ct = default)
    {
        bool result = LoanCases.Values.Any(loanCase =>
            string.Equals(loanCase.Number, loanCaseNumber, StringComparison.Ordinal));
        return Task.FromResult(result);
    }

    public Task<Domain.LoanCase?> GetByIdAsync(Guid loanCaseId, CancellationToken ct = default)
    {
        return Task.FromResult(LoanCases.GetValueOrDefault(loanCaseId));
    }

    public Task UpdateAsync(Domain.LoanCase loanCase, CancellationToken ct = default)
    {
        LoanCases[loanCase.Id] = loanCase;
        return Task.CompletedTask;
    }
}