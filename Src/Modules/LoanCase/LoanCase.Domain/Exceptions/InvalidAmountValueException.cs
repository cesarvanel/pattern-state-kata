using Shared.Domain.Exceptions;

namespace LoanCase.Domain.Exceptions;

public class InvalidAmountValueException(decimal value)
    : DomainException($"Le montant du prêt doit être supérieur à zéro: {value}.");