using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Models;

public enum FreqType
{
    Daily,
    Weekly,
    Monthly,
    Yearly
}

public class Payment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [MaxLength(150)]
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double Amount { get; set; }
    public FreqType FreqType { get; set; }
    public int FreqInterval { get; set; }
    
    [ForeignKey("AccountId")]
    public Account Account { get; set; } = null!;
}