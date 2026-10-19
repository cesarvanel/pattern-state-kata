using LoanCase.Domain.Enum;
using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Messages;
using LoanCase.Domain.Vo;

namespace LoanCase.Domain.States;

public abstract class LoanCaseState
{
    protected readonly LoanCase _loanCase;

    protected LoanCaseState(LoanCase loanCase)
    {
        _loanCase = loanCase;
    }

    public abstract LoanCaseStatus Status { get; }

    public virtual void Submit()
    {
        ActionNotAllowed(nameof(Submit));
    }

    public virtual void SendToCommittee()
    {
        ActionNotAllowed(nameof(SendToCommittee));
    }

    public virtual void Reject(RejectionDetails rejectionDetails)
    {
        ActionNotAllowed(nameof(Reject));
    }

    public virtual void Approve()
    {
        ActionNotAllowed(nameof(Approve));
    }

    public virtual void Close()
    {
        ActionNotAllowed(nameof(Close));
    }

    private void ActionNotAllowed(string action)
    {
        throw new InvalidLoanCaseStateActionException(
            LoanCaseMessages.ActionNotAllowedInState(action, Status));
    }
}
