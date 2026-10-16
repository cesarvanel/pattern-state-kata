using LoanCase.Application.Command.Submit;
using LoanCase.Domain.Enum;
using LoanCase.Tests.Builder;
using LoanCase.Tests.Repository;

namespace LoanCase.Tests.Units;

public class SubmitLoanCaseTest
{
    private readonly SubmitLoanCaseHandler _handler;
    private readonly Guid _loanCaseId = Guid.NewGuid();
    private readonly InMemoryLoanRepository _loanRepository;

    public SubmitLoanCaseTest()
    {
        _loanRepository = new InMemoryLoanRepository();
        _handler = new SubmitLoanCaseHandler(_loanRepository);
    }


    [Fact]
    public async Task Submit_Should_Transit_LoanCase_From_Draft_To_UnderReview()
    {
        await BuildSut();

        SubmitLoanCaseCommand command = new(_loanCaseId);

        SubmitLoanCaseResponse result =
            await _handler.HandleAsync(command);

        Assert.True(result.IsSubmitted, result.Message);
        Domain.LoanCase loanCase = Assert.Single(_loanRepository.LoanCases.Values);

        Assert.Equal(LoanCaseStatus.UnderReview, loanCase.Status);
    }

    [Fact]
    public async Task Submit_Fails_When_LoanCase_State_Is_Not_Draft()
    {
        await BuildSutWithUnderReview();

        SubmitLoanCaseCommand command = new(_loanCaseId);
        SubmitLoanCaseResponse result =
            await _handler.HandleAsync(command);

        Assert.False(result.IsSubmitted, result.Message);
    }

    private async Task BuildSut()
    {
        Domain.LoanCase loanCase = new LoanCaseSut().WithLoanCaseId(_loanCaseId).Build();

        await _loanRepository.AddAsync(loanCase);
    }

    private async Task BuildSutWithUnderReview()
    {
        Domain.LoanCase loanCase = new LoanCaseSut()
            .WithLoanCaseId(_loanCaseId)
            .WithStatus(LoanCaseStatus.UnderReview)
            .Build();

        await _loanRepository.AddAsync(loanCase);
    }
}