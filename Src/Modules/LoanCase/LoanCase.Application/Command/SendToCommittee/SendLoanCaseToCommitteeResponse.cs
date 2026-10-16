namespace LoanCase.Application.Command.SendToCommittee;

public class SendLoanCaseToCommitteeResponse
{
    public bool IsSendToCommittee { get; set; } = false;
    public string Message { get; set; } = string.Empty;
}