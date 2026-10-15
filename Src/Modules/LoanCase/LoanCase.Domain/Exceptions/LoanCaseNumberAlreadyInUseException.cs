using Shared.Domain.Exceptions;

namespace LoanCase.Domain.Exceptions;

public class LoanCaseNumberAlreadyInUseException(string number)
    : DomainException($"Ce numéro de dossier est déjà utilisé {number}");