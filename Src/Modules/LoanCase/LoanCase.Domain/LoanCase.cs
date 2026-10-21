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
    private readonly List<LoanCaseStateSnapshot> _stateHistory;
    private LoanCaseState _state = null!;

    private LoanCase(Guid id,
        Guid borrowerId,
        string number,
        DateTime createdAt,
        Amount amount,
        RejectionDetails? rejectionDetails,
        List<AttachedDocument> attachedDocuments,
        IReadOnlyList<LoanCaseStateSnapshot> stateHistory)
    {
        Id = id;
        BorrowerId = borrowerId;
        Number = number;
        CreatedAt = createdAt;
        Amount = amount;
        RejectionDetails = rejectionDetails;
        _attachedDocuments = [.. attachedDocuments];
        _stateHistory = [.. stateHistory];
    }

    public LoanCaseStatus Status => _state.Status;

    public int Version => _stateHistory.Count;

    public IReadOnlyCollection<LoanCaseStateSnapshot> StateHistory => _stateHistory.AsReadOnly();

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
            attachedDocuments,
            []);

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
        IReadOnlyList<LoanCaseStateSnapshot>? stateHistory = null)
    {
        IReadOnlyList<LoanCaseStateSnapshot> history = stateHistory ?? [];

        EnsureHistoryIsConsistent(id, status, history);

        LoanCase loanCase = new(id,
            borrowerId,
            number,
            createdAt,
            amount,
            rejectionDetails,
            attachedDocuments,
            history);

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

    public LoanCaseStateSnapshot RecordState(DateTime savedAt,
        Guid? createdBy = null,
        Guid? rolledBackFrom = null)
    {
        LoanCaseStateSnapshot entry = new(Guid.NewGuid(),
            Id,
            BorrowerId,
            Status,
            Number,
            Amount.Value,
            CreatedAt,
            savedAt,
            createdBy,
            _stateHistory.Count + 1,
            rolledBackFrom,
            [.. _attachedDocuments],
            RejectionDetails);

        _stateHistory.Add(entry);

        return entry;
    }

    public LoanCaseStateSnapshot Restore(int version,
        DateTime savedAt,
        Guid? createdBy = null)
    {
        if (Status == LoanCaseStatus.Closed)
        {
            throw new InvalidLoanCaseStateActionException(
                LoanCaseMessages.ActionNotAllowedInState(nameof(Restore), Status));
        }

        LoanCaseStateSnapshot target = _stateHistory
                                           .FirstOrDefault(entry => entry.Version == version)
                                       ?? throw new LoanCaseSnapshotNotFoundException(
                                           $"{Id}:{version}");

        BorrowerId = target.BorrowerId;
        Number = target.Number;
        Amount = new Amount(target.Amount);
        CreatedAt = target.CreatedAt;
        RejectionDetails = target.RejectionDetails;
        _attachedDocuments.Clear();
        _attachedDocuments.AddRange(target.AttachedDocuments);
        _state = LoanCaseStateFactory.FromStatus(this, target.Status);

        return RecordState(savedAt, createdBy, target.Id);
    }

    private static void EnsureHistoryIsConsistent(Guid id,
        LoanCaseStatus status,
        IReadOnlyList<LoanCaseStateSnapshot> history)
    {
        if (history.Count == 0)
        {
            return;
        }

        LoanCaseStateSnapshot lastEntry = history[^1];

        if (lastEntry.LoanCaseId != id)
        {
            throw new InvalidLoanCaseSnapshotException(LoanCaseMessages.LoanCaseIdMismatch);
        }

        if (lastEntry.Version != history.Count)
        {
            throw new InvalidLoanCaseSnapshotException(LoanCaseMessages.LoanCaseStateHistoryGap);
        }

        if (lastEntry.Status != status)
        {
            throw new InvalidLoanCaseSnapshotException(
                LoanCaseMessages.LoanCaseStateHistoryMismatch);
        }
    }
}