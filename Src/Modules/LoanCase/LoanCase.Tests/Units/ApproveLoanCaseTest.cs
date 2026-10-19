using LoanCase.Application.Command.Approve;
using LoanCase.Domain.Enum;
using LoanCase.Tests.Builder;
using LoanCase.Tests.Repository;

namespace LoanCase.Tests.Units;

public class ApproveLoanCaseTest
{
    private readonly Guid _loanCaseId = Guid.NewGuid();

    private readonly InMemoryLoanRepository _loanRepository;

    public ApproveLoanCaseTest()
    {
        _loanRepository = new InMemoryLoanRepository();
    }

    [Fact]
    public async Task Approve_When_CommitteeReview_Should_Transition_To_Approved()
    {
        await BuildSut(LoanCaseStatus.CommitteeReview);

        ApprovedLoanCaseCommand command = new(_loanCaseId);

        ApproveLoanCaseResponse result =
            await new ApproveLoanCaseHandler(_loanRepository).HandleAsync(command);

        Assert.True(result.IsApproved, result.Message);
        Domain.LoanCase loanCase = Assert.Single(_loanRepository.LoanCases.Values);
        Assert.Equal(LoanCaseStatus.Approved, loanCase.Status);
    }

    [Fact]
    public async Task Approve_Fails_When_LoanCase_State_Is_Not_CommitteeReview()
    {
        await BuildSut(LoanCaseStatus.UnderReview);

        ApprovedLoanCaseCommand command = new(_loanCaseId);

        ApproveLoanCaseResponse result =
            await new ApproveLoanCaseHandler(_loanRepository).HandleAsync(command);

        Assert.False(result.IsApproved, result.Message);
        Domain.LoanCase loanCase = Assert.Single(_loanRepository.LoanCases.Values);
        Assert.Equal(LoanCaseStatus.UnderReview, loanCase.Status);
    }

    private async Task BuildSut(LoanCaseStatus status)
    {
        Domain.LoanCase loanCase = new LoanCaseSut().WithLoanCaseId(_loanCaseId)
            .WithStatus(status).Build();

        await _loanRepository.AddAsync(loanCase);
    }
}
