using LoanCase.Domain.Enum;
using LoanCase.Domain.Vo;

namespace LoanCase.Tests.Builder;

public sealed class LoanCaseSut
{
    private static readonly DateTime Now = new(2026, 09, 20, 0, 0, 0, DateTimeKind.Utc);

    private readonly List<AttachedDocument> _attachedDocuments =
    [
        AttachedDocument.Create(
            "CNI_Dirigeant.pdf",
            1.5f,
            ".pdf",
            AttachedDocumentType.IdentityCard
        ),
        AttachedDocument.Create(
            "RCCM_LaConfiance.pdf",
            2.3f,
            ".pdf",
            AttachedDocumentType.Payslip
        ),
        AttachedDocument.Create(
            "Attestation_Fiscale.pdf",
            1.1f,
            ".pdf",
            AttachedDocumentType.TaxReturn
        ),
        AttachedDocument.Create(
            "Plan_Localisation.png",
            840f,
            ".png",
            AttachedDocumentType.ProofOfAddress
        )
    ];

    private readonly Guid _borrowerId = Guid.NewGuid();
    private readonly string _loanNumber = "DOSSIER-2026-0001";
    private LoanCaseStatus _initialStatus = LoanCaseStatus.Draft;
    private Guid _loanCaseId = Guid.NewGuid();


    public LoanCaseSut WithLoanCaseId(Guid loanCaseId)
    {
        _loanCaseId = loanCaseId;
        return this;
    }

    public LoanCaseSut WithStatus(LoanCaseStatus status)
    {
        _initialStatus = status;
        return this;
    }


    public Domain.LoanCase Build()
    {
        Domain.LoanCase loanCase = Domain.LoanCase.Create(_loanCaseId,
            _borrowerId,
            _loanNumber,
            25_000_000,
            Now,
            _attachedDocuments);

        if (_initialStatus == LoanCaseStatus.UnderReview)
        {
            loanCase.Submit();
        }


        if (_initialStatus == LoanCaseStatus.CommitteeReview)
        {
            loanCase.Submit();
        }

        return loanCase;
    }
}