namespace LoanCase.Domain.Port.Out;

public interface ILoanCaseRepository
{
    Task AddAsync(LoanCase loanCase, CancellationToken ct = default);

    Task<bool> ExistsByNumberAsync(string loanCaseNumber, CancellationToken ct = default);
    Task<LoanCase?> GetByIdAsync(Guid loanCaseId, CancellationToken ct = default);
}