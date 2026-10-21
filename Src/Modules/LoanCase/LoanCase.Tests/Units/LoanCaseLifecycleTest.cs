using LoanCase.Application.Command.Approve;
using LoanCase.Application.Command.Close;
using LoanCase.Application.Command.Create;
using LoanCase.Application.Command.Reject;
using LoanCase.Application.Command.SendToCommittee;
using LoanCase.Application.Command.Submit;
using LoanCase.Domain.Enum;
using LoanCase.Domain.Vo;
using LoanCase.Tests.Builder;
using LoanCase.Tests.Repository;

using Shared.Domain.Vo;

namespace LoanCase.Tests.Units;

public class LoanCaseLifecycleTest
{
    private readonly ApproveLoanCaseHandler _approveHandler;
    private readonly Guid _borrowerId = Guid.NewGuid();
    private readonly CloseLoanCaseHandler _closeHandler;
    private readonly CreateLoanCaseHandler _createHandler;
    private readonly InMemoryGenerateLoanCaseNumberService _generateLoanCaseNumberService;
    private readonly Guid _loanCaseId = Guid.NewGuid();
    private readonly InMemoryLoanRepository _loanRepository;
    private readonly RejectLoanCaseHandler _rejectHandler;
    private readonly SendLoanCaseToCommitteeHandler _sendToCommitteeHandler;
    private readonly InMemoryLoanCaseSnapshotRepository _snapshotRepository;
    private readonly SubmitLoanCaseHandler _submitHandler;

    public LoanCaseLifecycleTest()
    {
        LoanCaseTestContext context = new();
        _loanRepository = context.LoanCaseRepository;
        _snapshotRepository = context.SnapshotRepository;
        _generateLoanCaseNumberService = context.GenerateLoanCaseNumberService;
        _createHandler = context.CreateHandler;
        _submitHandler = context.SubmitHandler;
        _sendToCommitteeHandler = context.SendToCommitteeHandler;
        _approveHandler = context.ApproveHandler;
        _closeHandler = context.CloseHandler;
        _rejectHandler = context.RejectHandler;
    }

    [Fact]
    public async Task Lifecycle_Approved_Should_Increment_Version_At_Each_State()
    {
        await CreateAsync();
        AssertState(1, LoanCaseStatus.Draft);

        await SubmitAsync();
        AssertState(2, LoanCaseStatus.UnderReview);

        await SendToCommitteeAsync();
        AssertState(3, LoanCaseStatus.CommitteeReview);

        await ApproveAsync();
        AssertState(4, LoanCaseStatus.Approved);

        await CloseAsync();
        AssertState(5, LoanCaseStatus.Closed);
    }

    [Fact]
    public async Task Lifecycle_Rejected_Should_Increment_Version_At_Each_State()
    {
        await CreateAsync();
        AssertState(1, LoanCaseStatus.Draft);

        await SubmitAsync();
        AssertState(2, LoanCaseStatus.UnderReview);

        await SendToCommitteeAsync();
        AssertState(3, LoanCaseStatus.CommitteeReview);

        await RejectAsync();
        AssertState(4, LoanCaseStatus.Rejected);
    }

    private async Task CreateAsync()
    {
        CreateLoanCaseCommand command = new(_loanCaseId,
            _borrowerId,
            25_000_000m,
            GenerateAllTypes());

        CreateLoanCaseResponse response =
            await _createHandler.HandleAsync(command, CancellationToken.None);

        Assert.True(response.IsCreated, response.Message);
    }

    private async Task SubmitAsync()
    {
        SubmitLoanCaseResponse response =
            await _submitHandler.HandleAsync(new SubmitLoanCaseCommand(_loanCaseId));

        Assert.True(response.IsSubmitted, response.Message);
    }

    private async Task SendToCommitteeAsync()
    {
        SendLoanCaseToCommitteeResponse response =
            await _sendToCommitteeHandler.HandleAsync(
                new SendLoanCaseToCommitteeCommand(_loanCaseId),
                CancellationToken.None);

        Assert.True(response.IsSendToCommittee, response.Message);
    }

    private async Task ApproveAsync()
    {
        ApproveLoanCaseResponse response =
            await _approveHandler.HandleAsync(new ApproveLoanCaseCommand(_loanCaseId));

        Assert.True(response.IsApproved, response.Message);
    }

    private async Task CloseAsync()
    {
        CloseLoanCaseResponse response =
            await _closeHandler.HandleAsync(new CloseLoanCaseCommand(_loanCaseId));

        Assert.True(response.IsClosed, response.Message);
    }

    private async Task RejectAsync()
    {
        RejectLoanCaseResponse response = await _rejectHandler.HandleAsync(
            new RejectLoanCaseCommand(_loanCaseId,
                "Document non conforme",
                DocumentFile.Create("Documents/rejected", 150, ".pdf")),
            CancellationToken.None);

        Assert.True(response.IsRejected, response.Message);
    }

    private void AssertState(int expectedVersion, LoanCaseStatus expectedStatus)
    {
        Domain.LoanCase? loanCase = _loanRepository.LoanCases.GetValueOrDefault(_loanCaseId);

        Assert.NotNull(loanCase);
        Assert.Equal(expectedVersion, loanCase.Version);
        Assert.Equal(expectedStatus, loanCase.Status);

        Assert.Equal(expectedVersion, _snapshotRepository.StateSnapshots.Count);
        Assert.Equal(expectedVersion, _snapshotRepository.StateSnapshots.Last().Version);
        Assert.Equal(expectedStatus, _snapshotRepository.StateSnapshots.Last().Status);
    }

    private static List<AttachedDocument> GenerateAllTypes()
    {
        return
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
    }
}
