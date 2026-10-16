using LoanCase.Application.Command.Create;
using LoanCase.Domain.Enum;
using LoanCase.Domain.Vo;
using LoanCase.Tests.Builder;
using LoanCase.Tests.Repository;

namespace LoanCase.Tests.Units;

public class CreateLoanCaseTest
{
    private readonly CancellationToken _ct = CancellationToken.None;
    private readonly InMemoryGenerateLoanCaseNumberService _generateLoanCaseNumberService;
    private readonly CreateLoanCaseHandler _handler;
    private readonly InMemoryLoanRepository _loanCaseRepository;

    public CreateLoanCaseTest()
    {
        _loanCaseRepository = new InMemoryLoanRepository();
        _generateLoanCaseNumberService = new InMemoryGenerateLoanCaseNumberService(null);
        _handler = new CreateLoanCaseHandler(_loanCaseRepository, _generateLoanCaseNumberService);
    }

    [Fact]
    public async Task Create_LoanCase_Successfully()
    {
        CreateLoanCaseCommand command = new CreateLoanCaseCommandBuilder()
            .WithDocuments(GenerateAllTypes()).Build();
        CreateLoanCaseResponse result =
            await _handler.HandleAsync(command);

        Assert.True(result.IsCreated, result.Message);
        Domain.LoanCase loanCase = Assert.Single(_loanCaseRepository.LoanCases.Values);

        Assert.NotEmpty(loanCase.AttachedDocuments);
        Assert.Equal(command.Amount, loanCase.Amount.Value);
        Assert.Equal(command.Id, loanCase.Id);
        Assert.Equal(LoanCaseStatus.Draft, loanCase.Status);
    }

    [Fact]
    public async Task Create_Should_Fails_When_AttachedDocument_Is_Empty()
    {
        CreateLoanCaseCommand command = new CreateLoanCaseCommandBuilder().Build();
        CreateLoanCaseResponse result =
            await _handler.HandleAsync(command);

        Assert.False(result.IsCreated, result.Message);
        Assert.Empty(_loanCaseRepository.LoanCases.Values);
    }

    [Fact]
    public async Task Create_Should_Fail_When_Some_Of_Required_AttachedDocumentType_Is_Missing()
    {
        CreateLoanCaseCommand command =
            new CreateLoanCaseCommandBuilder().WithDocuments([.. GenerateAllTypes().Take(2)])
                .Build();
        CreateLoanCaseResponse result =
            await _handler.HandleAsync(command);

        Assert.False(result.IsCreated, result.Message);
        Assert.Empty(_loanCaseRepository.LoanCases.Values);
    }

    [Fact]
    public async Task Create_Should_Fails_When_Existed_LoanCase_With_Same_Number()
    {
        await BuildSut();
        CreateLoanCaseCommand command = new CreateLoanCaseCommandBuilder()
            .WithDocuments(GenerateAllTypes())
            .Build();

        CreateLoanCaseResponse result = await _handler.HandleAsync(command);

        Assert.False(result.IsCreated, result.Message);
    }


    private async Task BuildSut()
    {
        string loanNumber = await _generateLoanCaseNumberService.ExecuteAsync();
        Domain.LoanCase loanCase = Domain.LoanCase.Create(Guid.NewGuid(),
            Guid.NewGuid(),
            loanNumber,
            25_000_000m,
            DateTime.UtcNow,
            GenerateAllTypes());
        await _loanCaseRepository.AddAsync(loanCase, _ct);
        _generateLoanCaseNumberService.ResetCounter();
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