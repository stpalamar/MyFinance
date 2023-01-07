using Infrastructure.Data.Models;

namespace ApplicationCore.DTO;

public class DepositDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double InitialDeposit { get; set; }
    public double MonthlyContribution { get; set; }
    public double InterestRate { get; set; }
    public DateTime StartDate { get; set; }
    public int Months { get; set; }
    
    public static explicit operator DepositDto(Deposit d) => new DepositDto()
    {
        Id = d.Id,
        Name = d.Name,
        InitialDeposit = d.InitialDeposit,
        MonthlyContribution = d.MonthlyContribution,
        InterestRate = d.InterestRate,
        StartDate = d.StartDate,
        Months = d.Months
    };
}