using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Models;

public enum Type
{
    Expense,
    Income
}

public class Transaction : ICloneable
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    [Required] 
    public Type Type { get; set; }
    public double Amount { get; set; }

    [ForeignKey("AccountId")]
    public Account Account { get; set; } = null!;
   
    [ForeignKey("ReceiptId")]
    public Receipt? Receipt { get; set; }
    
    public object Clone()
    {
        var clone = new Transaction()
        {
            Description = new string(Description),
            Date = Date,
            Type = Type,
            Amount = Amount,
            Account = Account,
            Receipt = Receipt,
        };
        return clone;
    }
}