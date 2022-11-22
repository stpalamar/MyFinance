using Infrastructure.Data.Models;
using Type = Infrastructure.Data.Models.Type;

namespace ApplicationCore.DTO;

public class TransactionDto
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public Type Type { get; set; }
    public double Amount { get; set; }
    public Guid AccountId { get; set; }
    public Guid? ReceiptId { get; set; }

    public static explicit operator TransactionDto(Transaction t) => new TransactionDto
    {
        Id = t.Id,
        Description = t.Description,
        Date = t.Date,
        Type = t.Type,
        Amount = t.Amount,
        AccountId = t.Account.Id,
        ReceiptId = t.Receipt?.Id
    };
}