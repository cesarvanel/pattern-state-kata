using LoanCase.Domain.Enum;
using LoanCase.Domain.Exceptions;
using LoanCase.Domain.Factory;
using LoanCase.Domain.Messages;
using LoanCase.Domain.Snapshot;
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
        int version,
        List<AttachedDocument> attachedDocuments)
    {
        Id = id;
        BorrowerId = borrowerId;
        Number = number;
        CreatedAt = createdAt;
        Amount = amount;
        RejectionDetails = rejectionDetails;
        _attachedDocuments = [.. attachedDocuments];
        Version = version;
    }

    public LoanCaseStatus Status => _state.Status;

    public int Version { get; private set; }

    public Guid Id { get; }
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
            0,
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
        RejectionDetails? rejectionDetails,
        int version = 0)
    {
        LoanCase loanCase = new(id,
            borrowerId,
            number,
            createdAt,
            amount,
            rejectionDetails,
            version,
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

    public LoanCaseStateSnapshot SaveState(
        Guid id,
        DateTime dateTime,
        Guid? createdBy = null,
        Guid? rolledBackFrom = null)
    {
        int version = ++Version;
        return new LoanCaseStateSnapshot(
            id,
            Id,
            BorrowerId,
            Status,
            Number,
            Amount.Value,
            CreatedAt,
            dateTime,
            createdBy,
            version,
            rolledBackFrom,
            [.. _attachedDocuments],
            RejectionDetails);
    }

    public void Restore(LoanCaseStateSnapshot snapshot)
    {
        if (Id != snapshot.LoanCaseId)
        {
            throw new InvalidLoanCaseSnapshotException(LoanCaseMessages.LoanCaseIdMismatch);
        }

        if (Status == LoanCaseStatus.Closed)
        {
            throw new InvalidLoanCaseStateActionException(
                LoanCaseMessages.ActionNotAllowedInState(nameof(Restore), Status));
        }

        BorrowerId = snapshot.BorrowerId;
        Number = snapshot.Number;
        Amount = new Amount(snapshot.Amount);
        CreatedAt = snapshot.CreatedAt;
        RejectionDetails = snapshot.RejectionDetails;
        _attachedDocuments.Clear();
        _attachedDocuments.AddRange(snapshot.AttachedDocuments);
        _state = LoanCaseStateFactory.FromStatus(this, snapshot.Status);
        Version = Math.Max(Version, snapshot.Version);
    }
}