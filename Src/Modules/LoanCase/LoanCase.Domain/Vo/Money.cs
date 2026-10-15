namespace LoanCase.Domain.Vo;

public class Money
{

    public decimal Value { get; private set; }
    public Money(decimal value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        
        Value = value;
    }
}