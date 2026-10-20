using LoanCase.Domain.Enum;
using LoanCase.Domain.Vo;

using Shared.Domain.Vo;

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


        switch (_initialStatus)
        {
            case LoanCaseStatus.UnderReview:
                loanCase.Submit();
                break;

            case LoanCaseStatus.CommitteeReview:
                loanCase.Submit();
                loanCase.SendToCommittee();
                break;

            case LoanCaseStatus.Approved:
                loanCase.Submit();
                loanCase.SendToCommittee();
                loanCase.Approve();
                break;

            case LoanCaseStatus.Rejected:
                loanCase.Submit();
                loanCase.Reject(CreateRejectionDetails());
                break;

            case LoanCaseStatus.Closed:
                loanCase.Submit();
                loanCase.SendToCommittee();
                loanCase.Approve();
                loanCase.Close();
                break;

            case LoanCaseStatus.Draft:
            default:
                break;
        }

        return loanCase;
    }

    private static RejectionDetails CreateRejectionDetails()
    {
        return new RejectionDetails("Documents non conformes",
            DocumentFile.Create("Lettre_Rejet.pdf", 150, ".pdf"));
    }
}