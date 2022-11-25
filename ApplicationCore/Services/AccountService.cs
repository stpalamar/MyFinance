using System.Globalization;
using System.Security.Claims;
using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using ClosedXML.Excel;
using Infrastructure;
using Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApplicationCore.Services;

public class AccountService : IAccountService
{
    
    private ApplicationDbContext _context;
    private readonly User _user;
    
    public AccountService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        var email = httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
        _user = _context.Users.First(u => u.Email == email);
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
            Amount = account.Amount,
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
}