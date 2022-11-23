using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Models;

public enum FreqType
{
    One,
    Daily,
    Weekly,
    Monthly,
    Yearly
}

public enum RepeatType
{
    Forever,
    UntilDate,
    NumberOfEvents
}

public class Payment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [MaxLength(150)]
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public Type Type { get; set; }
    public double Amount { get; set; }
    public FreqType FreqType { get; set; }
    public int? FreqInterval { get; set; }
    public RepeatType? RepeatType { get; set; }
    public int? NumberOfRepeats { get; set; }
    public DateTime? EndDate { get; set; }
    
    
    [ForeignKey("AccountId")]
    public Account Account { get; set; } = null!;
}