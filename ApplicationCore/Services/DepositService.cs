using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using Infrastructure;
using Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace ApplicationCore.Services;

public class DepositService : IDepositService
{
    private ApplicationDbContext _context;
    private readonly User _user;
    
    public DepositService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _user = (httpContextAccessor.HttpContext.Items["User"] as User)!;
    }

    public List<DepositDto> GetDeposits()
    {
        return _context.Deposits
            .Where(d => d.User.Id == _user.Id)
            .Select(d => (DepositDto)d)
            .ToList();
    }

    public DepositDto GetDepositById(Guid id)
    {
        if (_context.Deposits
            .Where(d => d.User.Id == _user.Id && d.Id == id).IsNullOrEmpty())
        {
            throw new NotFoundDepositException();
        }
        return _context.Deposits
            .Where(d=> d.User.Id == _user.Id && d.Id == id)
            .Select(d => (DepositDto)d)
            .First();
    }

    public DepositDto AddDeposit(DepositDto deposit)
    {
        var newDeposit = new Deposit
        {
            Name = deposit.Name,
            InitialDeposit = deposit.InitialDeposit,
            MonthlyContribution = deposit.MonthlyContribution,
            InterestRate = deposit.InterestRate,
            StartDate = deposit.StartDate,
            Months = deposit.Months,
            User = _user
        };
        _context.Deposits.Add(newDeposit);
        _context.SaveChanges();
        return (DepositDto)newDeposit;
    }

    public DepositDto UpdateDeposit(DepositDto deposit)
    {
        if (_context.Deposits
            .Where(d => d.User.Id == _user.Id && d.Id == deposit.Id).IsNullOrEmpty())
        {
            throw new NotFoundDepositException();
        }
        
        var dbDeposit = _context.Deposits
            .First(d => d.User.Id == _user.Id && d.Id == deposit.Id);
        
        dbDeposit.Name = deposit.Name;
        dbDeposit.InitialDeposit = deposit.InitialDeposit;
        dbDeposit.MonthlyContribution = deposit.MonthlyContribution;
        dbDeposit.InterestRate = deposit.InterestRate;
        dbDeposit.StartDate = deposit.StartDate;
        dbDeposit.Months = deposit.Months;
        
        _context.SaveChanges();
        return deposit;
    }

    public void DeleteDeposit(Guid id)
    {
        if (_context.Deposits
            .Where(d => d.User.Id == _user.Id && d.Id == id).IsNullOrEmpty())
        {
            throw new NotFoundDepositException();
        }
        _context.Deposits
            .Remove(_context.Deposits.First(d => d.User.Id == _user.Id && d.Id == id));
        _context.SaveChanges();
    }
}