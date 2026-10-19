namespace LoanCase.Application.Command.Close;

public class CloseLoanCaseResponse
{
    public bool IsClosed { get; set; } = false;

    public string Message { get; set; } = string.Empty;
}