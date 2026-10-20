using LoanCase.Domain.Vo;

using Shared.Application.Command;

namespace LoanCase.Application.Command.Create;

public record CreateLoanCaseCommand(
    Guid Id,
    Guid BorrowerId,
    decimal Amount,
    List<AttachedDocument> AttachedDocuments) : ICommand<CreateLoanCaseResponse>;
