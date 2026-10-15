using LoanCase.Domain.Vo;

namespace LoanCase.Domain;

public class LoanCase
{
   public Guid Id { get; private set; }
   public Guid BorrowerId { get; private set; } 
   public string Number { get; private set; } = string.Empty;
   public DateTime CreatedAt { get; private set; }
   public Money Amount { get; private set; }

   private List<AttachedDocument> _attachedDocuments = [];

   private LoanCase(Guid id, Guid borrowerId, string number, DateTime createdAt, Money amount,  List<AttachedDocument> attachedDocuments)
   {
       Id = id;
       BorrowerId = borrowerId;
       Number = number;
       CreatedAt = createdAt;
       Amount = amount;
       _attachedDocuments = attachedDocuments;
       
   }

   public static LoanCase Create(Guid id, Guid borrowerId, string number, decimal amount, DateTime createdAt,List<AttachedDocument> attachedDocuments)
   {
       return new LoanCase(id, borrowerId, number, createdAt, new Money(amount), attachedDocuments);
   }

}