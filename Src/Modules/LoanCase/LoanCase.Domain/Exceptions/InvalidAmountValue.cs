using Shared.Domain.Exceptions;

namespace LoanCase.Domain.Exceptions;

public class InvalidAmountValue(decimal value)
    : DomainException($"Le montant du prêt doit être supérieur à zéro: {value}.");