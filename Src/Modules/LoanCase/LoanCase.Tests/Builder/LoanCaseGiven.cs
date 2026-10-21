using LoanCase.Domain.Enum;
using LoanCase.Tests.Repository;

namespace LoanCase.Tests.Builder;

public sealed class LoanCaseGiven(
    InMemoryLoanRepository loanRepository,
    InMemoryLoanCaseSnapshotRepository snapshotRepository)
{
    public async Task<Domain.LoanCase> AtStatusAsync(Guid loanCaseId,
        LoanCaseStatus status)
    {
        Domain.LoanCase loanCase = new LoanCaseSut()
            .WithLoanCaseId(loanCaseId)
            .WithStatus(status)
            .Build();

        await loanRepository.AddAsync(loanCase);
        await snapshotRepository.AddAsync(loanCase.RecordState(DateTime.UtcNow));

        return loanCase;
    }
}
