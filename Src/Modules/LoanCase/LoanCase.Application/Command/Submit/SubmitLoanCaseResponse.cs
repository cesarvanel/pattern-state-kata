namespace LoanCase.Application.Command.Submit;

public class SubmitLoanCaseResponse
{
    public bool IsSubmitted { get; set; } = false;
    public string Message { get; set; } = string.Empty;
}