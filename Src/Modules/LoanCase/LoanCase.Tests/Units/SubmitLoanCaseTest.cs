using LoanCase.Application.Command.Submit;
using LoanCase.Domain.Enum;
using LoanCase.Tests.Builder;
using LoanCase.Tests.Repository;

namespace LoanCase.Tests.Units;

public class SubmitLoanCaseTest
{
    private readonly Guid _loanCaseId = Guid.NewGuid();
    private readonly InMemoryLoanRepository _loanRepository;

    public SubmitLoanCaseTest()
    {
        _loanRepository = new InMemoryLoanRepository();
    }


    [Fact]
    public async Task Submit_Should_Transit_LoanCase_From_Draft_To_UnderReview()
    {
        await BuilSut();

        SubmitLoanCaseCommand command = new(_loanCaseId);

        SubmitLoanCaseResponse result =
            await new SubmitLoanCaseHandler(_loanRepository).HandleAsync(command);

        Assert.True(result.IsSubmitted, result.Message);
        Domain.LoanCase loanCase = Assert.Single(_loanRepository.LoanCases.Values);

        Assert.Equal(LoanCaseStatus.UnderReview, loanCase.Status);
    }

    private async Task BuilSut()
    {
        Domain.LoanCase loanCase = new LoanCaseSut().WithLoanCaseId(_loanCaseId).Build();

        await _loanRepository.AddAsync(loanCase);
    }
}