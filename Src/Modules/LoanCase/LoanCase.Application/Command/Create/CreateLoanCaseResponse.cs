namespace LoanCase.Application.Command.Create;

public class CreateLoanCaseResponse
{
    public bool IsCreated { get; set; } = false;
    public string Message { get; set; } = string.Empty;
}