namespace LoanCase.Domain.Services;

public interface IGenerateLoanCaseNumberService
{
    Task<string> ExecuteAsync();
}