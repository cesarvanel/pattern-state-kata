using System.Collections.Immutable;

using LoanCase.Domain.Enum;
using LoanCase.Domain.Vo;

namespace LoanCase.Domain.Snapshot;

public record LoanCaseStateSnapshot(
    Guid Id,
    Guid LoanCaseId,
    Guid BorrowerId,
    LoanCaseStatus Status,
    string Number,
    decimal Amount,
    DateTime CreatedAt,
    DateTime SavedAt,
    Guid? CreatedBy,
    int Version,
    Guid? RolledBackFrom,
    ImmutableArray<AttachedDocument> AttachedDocuments,
    RejectionDetails? RejectionDetails);