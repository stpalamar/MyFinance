using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using Infrastructure;
using Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Type = Infrastructure.Data.Models.Type;

namespace ApplicationCore.Services;

public class AccountService : IAccountService
{
    
    private ApplicationDbContext _context;
    private readonly User _user;
    
    public AccountService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _user = (httpContextAccessor.HttpContext.Items["User"] as User)!;
    }
    
    public List<AccountDto> GetAccounts()
    {
        return _context.Accounts
            .Where(a => a.User.Id == _user.Id)
            .Select(a => (AccountDto)a)
            .ToList();
    }

    public AccountDto GetAccountById(Guid id)
    {
        if (_context.Accounts
            .Where(a => a.User.Id == _user.Id && a.Id == id).IsNullOrEmpty())
        {
            throw new NotFoundAccountException();
        }
        return _context.Accounts
            .Where(a => a.User.Id == _user.Id && a.Id == id)
            .Select(a => (AccountDto)a)
            .First();
    }

    public AccountDto AddAccount(AccountDto account)
    {
        var newAccount = new Account
        {
            Name = account.Name,
            InitialAmount = account.InitialAmount,
            Amount = account.InitialAmount,
            User = _user
        };
        _context.Accounts.Add(newAccount);
        _context.SaveChanges();
        return (AccountDto)newAccount;
    }

    public AccountDto UpdateAccount(AccountDto account)
    {
        if (_context.Accounts
            .Where(a => a.User.Id == _user.Id && a.Id == account.Id).IsNullOrEmpty())
        {
            throw new NotFoundAccountException();
        }
        
        var dbAccount = _context.Accounts
            .First(a => a.User.Id == _user.Id && a.Id == account.Id);
        
        dbAccount.Name = account.Name;
        dbAccount.InitialAmount = account.InitialAmount;
        dbAccount.Amount = account.Amount;
        
        
        _context.SaveChanges();
        CalculateAccountAmount(_context, _user);
        return account;
    }

    public void DeleteAccount(Guid id)
    {
        if (_context.Accounts
            .Where(a => a.User.Id == _user.Id && a.Id == id).IsNullOrEmpty())
        {
            throw new NotFoundAccountException();
        }
        _context.Transactions.RemoveRange(_context.Transactions.Where(t => t.Account.Id == id));
        _context.Accounts
            .Remove(_context.Accounts.First(t => t.User.Id == _user.Id && t.Id == id));
        _context.SaveChanges();
    }
    
    public void CalculateAccountAmount(ApplicationDbContext context, User user)
    {
        var userTransactions = context.Transactions
            .Include("Account")
            .Where(t => t.Account.User.Id == user.Id)
            .ToList();
        var accounts = _context.Accounts
            .Where(a => a.User.Id == user.Id)
            .ToList();
        context.Accounts
            .Where(a => a.User.Id == user.Id)
            .ToList()
            .ForEach(a =>
            {
                a.Amount = a.InitialAmount;
                a.Amount = userTransactions
                    .Where(t => t.Account.Id == a.Id)
                    .Sum(t => t.Type == Type.Income ? t.Amount : -t.Amount);
            });
        
        context.SaveChanges();
    }
}
