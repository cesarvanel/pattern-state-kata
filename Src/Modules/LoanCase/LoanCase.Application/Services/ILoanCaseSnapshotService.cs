namespace LoanCase.Application.Services;

public interface ILoanCaseSnapshotService
{
    Task ExecuteAsync(Domain.LoanCase loanCase,
        CancellationToken ct = default,
        Guid? createdBy = null,
        Guid? rolledBackFrom = null);
}
