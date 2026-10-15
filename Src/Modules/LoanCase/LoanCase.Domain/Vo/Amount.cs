using LoanCase.Domain.Exceptions;

namespace LoanCase.Domain.Vo;

public record Amount
{
    public Amount(decimal value)
    {
        if (value < 0)
        {
            throw new InvalidAmountValueException(value);
        }

        Value = value;
    }

    public decimal Value { get; private set; }
}