using LoanCase.Domain.Port.Out;
using LoanCase.Domain.Snapshot;
using LoanCase.Domain.Vo;

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
        Domain.LoanCase? loanCase = LoanCases.GetValueOrDefault(loanCaseId);

        return Task.FromResult<Domain.LoanCase?>(loanCase is null ? null : Rehydrate(loanCase));
    }

    public Task UpdateAsync(Domain.LoanCase loanCase, CancellationToken ct = default)
    {
        LoanCases[loanCase.Id] = loanCase;
        return Task.CompletedTask;
    }

    private static Domain.LoanCase Rehydrate(Domain.LoanCase loanCase)
    {
        IReadOnlyList<LoanCaseStateSnapshot> history = [.. loanCase.StateHistory];

        if (history.Count == 0)
        {
            return Domain.LoanCase.Rehydrate(loanCase.Id,
                loanCase.BorrowerId,
                loanCase.Number,
                loanCase.CreatedAt,
                loanCase.Amount,
                [.. loanCase.AttachedDocuments],
                loanCase.Status,
                loanCase.RejectionDetails,
                history);
        }

        LoanCaseStateSnapshot lastEntry = history[^1];

        return Domain.LoanCase.Rehydrate(lastEntry.LoanCaseId,
            lastEntry.BorrowerId,
            lastEntry.Number,
            lastEntry.CreatedAt,
            new Amount(lastEntry.Amount),
            [.. lastEntry.AttachedDocuments],
            lastEntry.Status,
            lastEntry.RejectionDetails,
            history);
    }
}
