using LoanCase.Domain.Vo;

namespace LoanCase.Application.Command.Create;

public record CreateLoanCaseCommand(Guid Id,Guid BorrowerId, decimal Amount,List<AttachedDocument> AttachedDocuments);