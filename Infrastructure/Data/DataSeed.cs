using Infrastructure.Data.Models;

namespace Infrastructure.Data;

public class DataSeed
{
    private readonly ApplicationDbContext _context;
    
    public DataSeed(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        if(!_context.Transactions.Any())
        {
            var transactions = new List<Transaction>()
            {
                // new Transaction()
                // {
                //     Description = "My first transaction",
                //     Date = DateTime.Now,
                //     Type = "Outcome",
                //     Amount = 100,
                //     AccountId = 0,
                //     CategoryId = 0,
                //     UserId = 0,
                // },
                // new Transaction()
                // {
                //     Description = "My second transaction",
                //     Date = DateTime.Today,
                //     Type = "Outcome",
                //     Amount = 100,
                //     AccountId = 0,
                //     CategoryId = 0,
                //     UserId = 0,
                // }
            };

            _context.Transactions.AddRange(transactions);
            _context.SaveChanges();
        }
    }
}