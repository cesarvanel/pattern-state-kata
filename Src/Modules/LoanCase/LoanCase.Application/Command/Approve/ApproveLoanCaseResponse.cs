namespace LoanCase.Application.Command.Approve;

public class ApproveLoanCaseResponse
{
    public bool IsApproved { get; set; } = false;
    public string Message { get; set; } = string.Empty;
}