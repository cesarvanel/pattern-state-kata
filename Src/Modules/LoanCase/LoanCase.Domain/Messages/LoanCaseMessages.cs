using LoanCase.Domain.Enum;
using LoanCase.Domain.Exceptions;

namespace LoanCase.Domain.Messages;

public static class LoanCaseMessages
{
    public const string EmptyAttachedDocument = "Les documents joints ne peuvent pas être vides";
    public const string LoanCaseNotFound = "Ce dossier n'existe pas ";
    public const string UnknownLoanCaseStatus = "Statut du dossier de prêt inconnu";

    public const string LoanCaseIdMismatch =
        "L'identifiant du dossier de prêt fourni ne correspond pas à l'identifiant attendu.";

    public static string ActionNotAllowedInState(string action, LoanCaseStatus status)
    {
        return $"L'action '{action}' n'est pas autorisée dans l'état '{status}'.";
    }

    public static string RequiredAttachmentDocumentType(AttachedDocumentType attachedDocumentType)
    {
        return
            $"La pièce jointe {attachedDocumentType.ToFrenchLabel()} est manquante dans les documents.";
    }
}

public static class AttachedDocumentTypeExtensions
{
    public static string ToFrenchLabel(this AttachedDocumentType type)
    {
        return type switch
        {
            AttachedDocumentType.IdentityCard => "Carte d'identité",
            AttachedDocumentType.Payslip => "Bulletin de paie",
            AttachedDocumentType.ProofOfAddress => "Justificatif de domicile",
            AttachedDocumentType.TaxReturn => "Déclaration fiscale",
            _ => throw new InvalidAttachmentTypeException()
        };
    }
}