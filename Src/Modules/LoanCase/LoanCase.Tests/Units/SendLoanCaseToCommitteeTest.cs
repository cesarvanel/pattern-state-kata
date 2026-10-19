using LoanCase.Application.Command.SendToCommittee;
using LoanCase.Domain.Enum;
using LoanCase.Tests.Builder;
using LoanCase.Tests.Repository;

namespace LoanCase.Tests.Units;

public class SendLoanCaseToCommitteeTest
{
    private readonly Guid _loanCaseId = Guid.NewGuid();

    private readonly InMemoryLoanRepository _loanRepository;

    public SendLoanCaseToCommitteeTest()
    {
        _loanRepository = new InMemoryLoanRepository();
    }

    [Fact]
    public async Task SendToCommittee_When_UnderReview_Should_Transition_To_CommitteeReview()
    {
        await BuildSut();
        SendLoanCaseToCommitteeCommand command = new(_loanCaseId);
        SendLoanCaseToCommitteeResponse result =
            await new SendLoanCaseToCommitteeHandler(_loanRepository).HandleAsync(command,
                CancellationToken.None);

        Assert.True(result.IsSendToCommittee, result.Message);

        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);

        Assert.Equal(LoanCaseStatus.CommitteeReview, loanCase.Status);
    }

    [Fact]
    public async Task SendToCommittee_Fails_When_LoanCase_State_Is_Not_UnderReview()
    {
        Domain.LoanCase loanCase = new LoanCaseSut().WithLoanCaseId(_loanCaseId).Build();
        await _loanRepository.AddAsync(loanCase);

        SendLoanCaseToCommitteeCommand command = new(_loanCaseId);
        SendLoanCaseToCommitteeResponse result =
            await new SendLoanCaseToCommitteeHandler(_loanRepository).HandleAsync(command,
                CancellationToken.None);

        Assert.False(result.IsSendToCommittee, result.Message);
        Assert.Equal(LoanCaseStatus.Draft, loanCase.Status);
    }



    private async Task BuildSut()
    {
        Domain.LoanCase loanCase = new LoanCaseSut().WithLoanCaseId(_loanCaseId)
            .WithStatus(LoanCaseStatus.UnderReview).Build();

        await _loanRepository.AddAsync(loanCase);
    }
}