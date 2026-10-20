namespace LoanCase.Application.Command.Reject;

public class RejectLoanCaseResponse
{
    public bool IsRejected { get; set; } = false;
    public string Message { get; set; } = string.Empty;
}