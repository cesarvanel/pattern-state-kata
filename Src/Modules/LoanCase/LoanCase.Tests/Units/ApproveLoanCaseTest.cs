using LoanCase.Application.Command.Approve;
using LoanCase.Domain.Enum;
using LoanCase.Tests.Builder;
using LoanCase.Tests.Repository;

using Xunit.Abstractions;

namespace LoanCase.Tests.Units;

public class ApproveLoanCaseTest
{
    private readonly Guid _loanCaseId = Guid.NewGuid();

    private readonly InMemoryLoanRepository _loanRepository;
    private readonly ITestOutputHelper _output;


    public ApproveLoanCaseTest(ITestOutputHelper output)
    {
        _output = output;
        _loanRepository = new InMemoryLoanRepository();
    }

    [Fact]
    public async Task Approve_When_UnderReview_Should_Transition_To_Approved()
    {
        await BuildSut();

        ApprovedLoanCaseCommand command = new(_loanCaseId);

        ApproveLoanCaseResponse result =
            await new ApproveLoanCaseHandler(_loanRepository).HandleAsync(command);

        _output.WriteLine(result.Message);
        Assert.True(result.IsApproved);
        Domain.LoanCase loanCase = Assert.Single(_loanRepository.LoanCases.Values);
        Assert.Equal(LoanCaseStatus.Approved, loanCase.Status);
    }

    private async Task BuildSut()
    {
        Domain.LoanCase loanCase = new LoanCaseSut().WithLoanCaseId(_loanCaseId)
            .WithStatus(LoanCaseStatus.CommitteeReview).Build();

        await _loanRepository.AddAsync(loanCase);
    }
}