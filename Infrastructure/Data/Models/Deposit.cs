using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Models;

public class Deposit
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double InitialDeposit { get; set; }
    public double MonthlyContribution { get; set; }
    public double InterestRate { get; set; }
    
    public DateTime StartDate { get; set; }
    public int Months { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}