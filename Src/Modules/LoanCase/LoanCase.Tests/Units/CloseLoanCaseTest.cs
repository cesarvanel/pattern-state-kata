using LoanCase.Application.Command.Close;
using LoanCase.Domain.Enum;
using LoanCase.Tests.Builder;
using LoanCase.Tests.Repository;

namespace LoanCase.Tests.Units;

public class CloseLoanCaseTest
{
    private readonly Guid _loanCaseId = Guid.NewGuid();

    private readonly InMemoryLoanRepository _loanRepository;

    public CloseLoanCaseTest()
    {
        _loanRepository = new InMemoryLoanRepository();
    }

    [Fact]
    public async Task Close_Loan_Case_Should_Transit_State_From_Approved_To_Closed()
    {
        await BuildSut(LoanCaseStatus.Approved);
        CloseLoanCaseResponse result =
            await new CloseLoanCaseHandler(_loanRepository).HandleAsync(
                new CloseLoanCaseCommand(_loanCaseId));

        Assert.True(result.IsClosed);
        Domain.LoanCase loanCase = Assert.Single(_loanRepository.LoanCases.Values);
        Assert.Equal(LoanCaseStatus.Closed, loanCase.Status);
    }

    [Fact]
    public async Task Close_Loan_Case_Should_Fails_When_State_Is_Not_Approved()
    {
        await BuildSut(LoanCaseStatus.CommitteeReview);

        CloseLoanCaseResponse result =
            await new CloseLoanCaseHandler(_loanRepository).HandleAsync(
                new CloseLoanCaseCommand(_loanCaseId));

        Assert.False(result.IsClosed);
        Domain.LoanCase loanCase = Assert.Single(_loanRepository.LoanCases.Values);
        Assert.NotEqual(LoanCaseStatus.Closed, loanCase.Status);
    }


    private async Task BuildSut(LoanCaseStatus status)
    {
        Domain.LoanCase loanCase = new LoanCaseSut().WithLoanCaseId(_loanCaseId)
            .WithStatus(status).Build();

        await _loanRepository.AddAsync(loanCase);
    }
}