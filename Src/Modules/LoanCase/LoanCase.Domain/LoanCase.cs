using LoanCase.Domain.Enum;
using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Messages;
using LoanCase.Domain.States;
using LoanCase.Domain.Vo;

namespace LoanCase.Domain;

public class LoanCase
{
    private readonly List<AttachedDocument> _attachedDocuments;
    private LoanCaseState _state = null!;

    private LoanCase(Guid id,
        Guid borrowerId,
        string number,
        DateTime createdAt,
        Amount amount,
        RejectionDetails? rejectionDetails,
        List<AttachedDocument> attachedDocuments)
    {
        Id = id;
        BorrowerId = borrowerId;
        Number = number;
        CreatedAt = createdAt;
        Amount = amount;
        RejectionDetails = rejectionDetails;
        _attachedDocuments = [.. attachedDocuments];
    }

    public LoanCaseStatus Status => _state.Status;

    public Guid Id { get; private set; }
    public Guid BorrowerId { get; private set; }
    public string Number { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Amount Amount { get; private set; }
    public RejectionDetails? RejectionDetails { get; internal set; }

    public IReadOnlyCollection<AttachedDocument> AttachedDocuments =>
        _attachedDocuments.AsReadOnly();

    public static LoanCase Create(Guid id,
        Guid borrowerId,
        string number,
        decimal amount,
        DateTime createdAt,
        List<AttachedDocument> attachedDocuments)
    {
        if (attachedDocuments.Count == 0)
        {
            throw new LoanCaseAttachedDocumentException(LoanCaseMessages.EmptyAttachedDocument);
        }

        List<AttachedDocumentType> allAttachedDocumentTypes =
            [.. attachedDocuments.Select(attachment => attachment.Type)];
        foreach (AttachedDocumentType attachedDocumentType in System.Enum
                     .GetValues(typeof(AttachedDocumentType))
                     .Cast<AttachedDocumentType>())
        {
            if (!allAttachedDocumentTypes.Contains(attachedDocumentType))
            {
                throw new LoanCaseAttachedDocumentException(
                    LoanCaseMessages.RequiredAttachmentDocumentType(attachedDocumentType));
            }
        }

        LoanCase loanCase = new(id,
            borrowerId,
            number,
            createdAt,
            new Amount(amount),
            null,
            attachedDocuments);

        loanCase.TransitionTo(new DraftState(loanCase));

        return loanCase;
    }

    public static LoanCase Rehydrate(Guid id,
        Guid borrowerId,
        string number,
        DateTime createdAt,
        Amount amount,
        List<AttachedDocument> attachedDocuments,
        LoanCaseStatus status,
        RejectionDetails? rejectionDetails)
    {
        LoanCase loanCase = new(id,
            borrowerId,
            number,
            createdAt,
            amount,
            rejectionDetails,
            attachedDocuments);

        loanCase._state = LoanCaseStateFactory.FromStatus(loanCase, status);

        return loanCase;
    }

    public void Submit()
    {
        _state.Submit();
    }

    public void Reject(RejectionDetails rejectionDetails)
    {
        _state.Reject(rejectionDetails);
    }

    public void SendToCommittee()
    {
        _state.SendToCommittee();
    }

    public void Approve()
    {
        _state.Approve();
    }

    public void Close()
    {
        _state.Close();
    }

    internal void TransitionTo(LoanCaseState nextState)
    {
        _state = nextState;
    }
}