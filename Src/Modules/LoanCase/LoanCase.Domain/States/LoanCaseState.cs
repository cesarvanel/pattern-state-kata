namespace LoanCase.Domain.States;

public abstract class LoanCaseState
{
    public virtual void Submit(LoanCase loanCase)
    {
        throw new InvalidOperationException("Le dossier ne peut pas être soumis depuis son état actuel.");
    }
}