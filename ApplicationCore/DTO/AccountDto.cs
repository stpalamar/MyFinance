using Infrastructure.Data.Models;

namespace ApplicationCore.DTO;

public class AccountDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public double InitialAmount { get; set; }
    public double Amount { get; set; }
    
    public static explicit operator AccountDto(Account a) => new AccountDto
    {
        Id = a.Id,
        Name = a.Name,
        InitialAmount = a.InitialAmount,
        Amount = a.Amount
    };
}