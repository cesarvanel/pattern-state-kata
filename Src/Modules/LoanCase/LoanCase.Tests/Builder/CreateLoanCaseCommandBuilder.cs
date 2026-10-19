using LoanCase.Application.Command.Create;
using LoanCase.Domain.Vo;

namespace LoanCase.Tests.Builder;

public class CreateLoanCaseCommandBuilder
{
    private const decimal Amount = 15.000m;
    private readonly Guid _borrowerId = Guid.NewGuid();
    private readonly Guid _id = Guid.NewGuid();

    private List<AttachedDocument> _documents = [];


    public CreateLoanCaseCommandBuilder WithDocuments(List<AttachedDocument> documents)
    {
        _documents = documents;
        return this;
    }

    public CreateLoanCaseCommand Build()
    {
        return new CreateLoanCaseCommand(_id, _borrowerId, Amount, _documents);
    }
}