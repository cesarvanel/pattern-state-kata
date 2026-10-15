namespace LoanCase.Domain.Port.Out;

public interface ILoanCaseRepository
{
    Task AddAsync(LoanCase loanCase);

    Task<bool> ExistsByNumberAsync(string loanCaseNumber);
}