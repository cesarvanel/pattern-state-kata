using LoanCase.Application.Command.Create;
using LoanCase.Tests.Repository;

namespace LoanCase.Tests;

public class CreateLoanCaseTest
{
    private readonly InMemoryLoanRepository _loanCaseRepository;

    public CreateLoanCaseTest()
    {
        _loanCaseRepository = new InMemoryLoanRepository();
    }

    [Fact]
    public async Task CanCreateLoanCase()
    {
        CreateLoanCaseCommand command = new(Guid.NewGuid(), Guid.NewGuid(), 1500.00m, []);
        CreateLoanCaseResponse result =
            await new CreateLoanCaseHandler(_loanCaseRepository).HandleAsync(command);

        Assert.True(result.IsCreated);
        Assert.Single(_loanCaseRepository.LoanCases.Values);
    }
}