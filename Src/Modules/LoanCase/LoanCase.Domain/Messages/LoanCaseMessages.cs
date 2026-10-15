using LoanCase.Domain.Enum;
using LoanCase.Domain.Exceptions;

namespace LoanCase.Domain.Messages;

public static class LoanCaseMessages
{
    public const string EmptyAttachedDocument = "Les documents joints ne peuvent pas être vides";

    public static string RequiredAttachmentDocumentType(AttachedDocumentType attachedDocumentType)
    {
        return $"La pièce jointe {attachedDocumentType.ToFrenchLabel()} est manquante dans les documents.";
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