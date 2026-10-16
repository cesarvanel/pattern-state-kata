using Shared.Domain.Validator;
using Shared.Domain.Vo;

namespace LoanCase.Domain.Vo;

public record RejectionDetails
{
    public RejectionDetails(string rejectReason, DocumentFile documentFile)
    {
        DomainValidator<RejectionDetails> validator = new();

        validator.Ensure(!string.IsNullOrEmpty(rejectReason),
            "Le motif de rejet ne peut pas être vide.");

        validator.ThrowIfInvalid();
        RejectReason = rejectReason;
        DocumentFile = documentFile;
    }

    public string RejectReason { get; }
    public DocumentFile DocumentFile { get; }
}