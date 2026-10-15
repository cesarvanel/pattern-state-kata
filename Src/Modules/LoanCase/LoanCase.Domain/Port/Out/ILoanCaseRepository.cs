namespace LoanCase.Domain.Port.Out;

public interface ILoanCaseRepository
{
    Task AddAsync(LoanCase loanCase);
}