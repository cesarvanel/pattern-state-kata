using Shared.Application.Command;

namespace LoanCase.Application.Command.Restore;

public record RestoreLoanCaseStateCommand(Guid LoanCaseId, Guid CreatedBy, int Version)
    : ICommand<RestoreLoanCaseStateResponse>;
