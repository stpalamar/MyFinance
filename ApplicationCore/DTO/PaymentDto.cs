using Infrastructure.Data.Models;
using Type = Infrastructure.Data.Models.Type;

namespace ApplicationCore.DTO;

public class PaymentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public Type Type { get; set; }
    public double Amount { get; set; }
    public FreqType FreqType { get; set; }
    public int? FreqInterval { get; set; }
    public RepeatType? RepeatType { get; set; }
    public int? NumberOfRepeats { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid AccountId { get; set; }

    public static explicit operator PaymentDto(Payment p) => new PaymentDto
    {
        Id = p.Id,
        Name = p.Name,
        StartDate = p.StartDate,
        RepeatType = p.RepeatType,
        NumberOfRepeats = p.NumberOfRepeats,
        EndDate = p.EndDate,
        Type = p.Type,
        Amount = p.Amount,
        FreqType = p.FreqType,
        FreqInterval = p.FreqInterval,
        AccountId = p.Account.Id
    };
}