using LoanCase.Domain.Services;

namespace LoanCase.Tests.Repository;

public class InMemoryGenerateLoanCaseNumberService(int? counter) : IGenerateLoanCaseNumberService
{
    private int _counter = counter ?? 0;

    public Task<string> Execute()
    {
        int nextNumber = Interlocked.Increment(ref _counter);

        string caseNumber = $"DOSSIER-{DateTime.UtcNow.Year}-{nextNumber:D4}";

        return Task.FromResult(caseNumber);
    }

    public void ResetCounter()
    {
        Interlocked.Exchange(ref _counter, 0);
    }
}