namespace LoanCase.Application.Command.Restore;

public class RestoreLoanCaseStateResponse
{
    public bool IsRestored { get; set; } = false;
    public int RestoredVersion { get; set; }
    public string Message { get; set; } = string.Empty;
}