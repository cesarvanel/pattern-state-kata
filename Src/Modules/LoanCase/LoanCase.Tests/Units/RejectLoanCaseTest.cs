using LoanCase.Application.Command.reject;
using LoanCase.Domain.Enum;
using LoanCase.Tests.Builder;
using LoanCase.Tests.Repository;

using Shared.Domain.Vo;

namespace LoanCase.Tests.Units;

public class RejectLoanCaseTest
{
    private readonly Guid _loanCaseId = Guid.NewGuid();

    private readonly InMemoryLoanRepository _loanRepository;

    public RejectLoanCaseTest()
    {
        _loanRepository = new InMemoryLoanRepository();
    }

    [Fact]
    public async Task Reject_When_UnderReview_Should_Transition_To_Rejected()
    {
        await BuildSut();
        RejectLoanCaseCommand command = new(_loanCaseId,
            "Document nom conforme",
            DocumentFile.Create("Documents/rejected", 150, ".pdf"));
        RejectLoanCaseResponse response =
            await new RejectLoanCaseHandler(_loanRepository).HandleAsync(command,
                CancellationToken.None);

        Assert.True(response.IsRejected, response.Message);

        Domain.LoanCase? loanCase =
            await _loanRepository.GetByIdAsync(_loanCaseId, CancellationToken.None);

        Assert.NotNull(loanCase);

        Assert.Equal(LoanCaseStatus.Rejected, loanCase.Status);
        Assert.NotNull(loanCase.RejectionDetails);
    }

    private async Task BuildSut()
    {
        Domain.LoanCase loanCase = new LoanCaseSut().WithLoanCaseId(_loanCaseId)
            .WithStatus(LoanCaseStatus.UnderReview).Build();

        await _loanRepository.AddAsync(loanCase);
    }
}