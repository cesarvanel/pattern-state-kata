namespace LoanCase.Application.Command.reject;

public class RejectLoanCaseResponse
{
    public bool IsRejected { get; set; } = false;
    public string Message { get; set; } = string.Empty;
}