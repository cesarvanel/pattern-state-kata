namespace LoanCase.Domain.Enum;

public enum LoanCaseStatus
{
    Draft, // En cours de constitution par le client (modifiable, ajout/suppression de pièces)
    UnderReview, // En cours d'analyse des risques par un gestionnaire/analyste
    Rejected, // 
    CommitteeReview, // En attente de vote par le comité de crédit (documents scellés)
    Approved, // Accepté par le comité (en attente de l'offre signée et du décaissement)
    Closed // Clôturé (fonds décaissés) ou Refusé (fin de vie définitive)
}