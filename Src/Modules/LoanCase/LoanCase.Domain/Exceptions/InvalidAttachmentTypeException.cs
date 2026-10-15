using Shared.Domain.Exceptions;

namespace LoanCase.Domain.Exceptions;

public class InvalidAttachmentTypeException() : DomainException("Type de document invalide")
{
}