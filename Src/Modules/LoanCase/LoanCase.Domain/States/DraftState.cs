namespace LoanCase.Domain.States;

public sealed class DraftState : LoanCaseState
{
    public override void Submit(LoanCase loanCase)
    {
        // 1. Ici, tu pourras rajouter tes règles métier pour passer à l'état suivant (ex: vérifier que les documents sont bien là)

        // 2. On change l'état du contexte vers le prochain état (SubmittedState)
        loanCase.TransitionTo(new SubmittedState());
    }
}