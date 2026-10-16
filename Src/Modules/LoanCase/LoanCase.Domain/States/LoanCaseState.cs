using LoanCase.Domain.Enum;
using LoanCase.Domain.Exceptions;

namespace LoanCase.Domain.States;

public abstract class LoanCaseState
{
    protected LoanCase _loanCase;

    public abstract LoanCaseStatus Status { get; }

    internal void SetLoanCase(LoanCase loanCase)
    {
        _loanCase = loanCase;
    }


    public virtual void Submit()
    {
        DefaultStateImplementation();
    }

    public virtual void SendToCommittee()
    {
        DefaultStateImplementation();
    }


    public virtual void Reject()
    {
        DefaultStateImplementation();
    }

    public virtual void Approve()
    {
        DefaultStateImplementation();
    }

    private static void DefaultStateImplementation()
    {
        throw new InvalidLoanCaseStateActionException(
            "Action non autorisée");
    }
}