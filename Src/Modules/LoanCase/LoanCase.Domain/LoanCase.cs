using LoanCase.Domain.Enum;
using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Messages;
using LoanCase.Domain.Vo;

namespace LoanCase.Domain;

public class LoanCase
{
    private List<AttachedDocument> _attachedDocuments = [];

    private LoanCase(Guid id, Guid borrowerId, string number, DateTime createdAt, Money amount,
        List<AttachedDocument> attachedDocuments)
    {
        Id = id;
        BorrowerId = borrowerId;
        Number = number;
        CreatedAt = createdAt;
        Amount = amount;
        _attachedDocuments = attachedDocuments;
    }

    public Guid Id { get; private set; }
    public Guid BorrowerId { get; private set; }
    public string Number { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Money Amount { get; private set; }


    public static LoanCase Create(Guid id, Guid borrowerId, string number, decimal amount, DateTime createdAt,
        List<AttachedDocument> attachedDocuments)
    {
        if (attachedDocuments.Count == 0)
        {
            throw new LoanCaseAttachedDocumentException(LoanCaseMessages.EmptyAttachedDocument);
        }

        List<AttachedDocumentType> allAttachedDocumentTypes =
            [.. attachedDocuments.Select(attachment => attachment.Type)];
        foreach (AttachedDocumentType attachedDocumentType in System.Enum.GetValues(typeof(AttachedDocumentType))
                     .Cast<AttachedDocumentType>())
        {
            if (!allAttachedDocumentTypes.Contains(attachedDocumentType))
            {
                throw new LoanCaseAttachedDocumentException(
                    LoanCaseMessages.RequiredAttachmentDocumentType(attachedDocumentType));
            }
        }

        return new LoanCase(id, borrowerId, number, createdAt, new Money(amount), attachedDocuments);
    }
}