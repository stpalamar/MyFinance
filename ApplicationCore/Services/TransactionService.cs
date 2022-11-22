using System.Security.Claims;
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

public class TransactionService : ITransactionService
{
    private ApplicationDbContext _context;
    private readonly User _user;

    public TransactionService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        var email = httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
        _user = _context.Users.First(u => u.Email == email);
    }

    public List<TransactionDto> GetTransactions()
    {
        return _context.Transactions
            .Include("Account")
            .Where(t => t.Account.User.Id == _user.Id)
            .Select(t => (TransactionDto)t)
            .ToList();
    }

    public TransactionDto GetTransactionById(Guid id)
    {
        if (_context.Transactions.Include("Account").Where(t => t.Account.User.Id == _user.Id && t.Id == id).IsNullOrEmpty())
        {
            throw new NotFoundTransactionException();
        }
        return _context.Transactions
            .Include("Account")
            .Where(t => t.Account.User.Id == _user.Id && t.Id == id)
            .Select(t => (TransactionDto)t)
            .First();
    }

    public TransactionDto AddTransaction(TransactionDto transaction)
    {
        var account = _context.Accounts
            .First(a => a.User.Id == _user.Id && a.Id == transaction.AccountId);

        var newTransaction = new Transaction
        {
            Date = transaction.Date,
            Description = transaction.Description,
            Type = transaction.Type,
            Amount = transaction.Amount,
            Account = account
        };
        _context.Transactions.Add(newTransaction);
        _context.SaveChanges();

        CalculateAccountAmount();
        return (TransactionDto)newTransaction;
    }

    public TransactionDto UpdateTransaction(TransactionDto transaction)
    {
        if (_context.Transactions.Include("Account")
            .Where(t => t.Account.User.Id == _user.Id && t.Id == transaction.Id).IsNullOrEmpty())
        {
            throw new NotFoundTransactionException();
        }
        
        var dbExpense = _context.Transactions
            .Include("Account")
            .First(t => t.Account.User.Id == _user.Id && t.Id == transaction.Id);

        dbExpense.Description = transaction.Description;
        dbExpense.Date = transaction.Date;
        dbExpense.Type = transaction.Type;
        dbExpense.Amount = transaction.Amount;

        _context.SaveChanges();

        CalculateAccountAmount();
        return transaction;
    }

    public void DeleteTransaction(Guid id)
    {
        if (_context.Transactions.Include("Account")
            .Where(t => t.Account.User.Id == _user.Id && t.Id == id).IsNullOrEmpty())
        {
            throw new NotFoundTransactionException();
        }
        
        _context.Transactions
            .Remove(_context.Transactions.First(t => t.Account.User.Id == _user.Id && t.Id == id));
        _context.SaveChanges();
        CalculateAccountAmount();
    }

    private void CalculateAccountAmount()
    {
        var userTransactions = _context.Transactions
            .Include("Account")
            .Where(t => t.Account.User.Id == _user.Id)
            .ToList();

        _context.Accounts
            .Where(a => a.User.Id == _user.Id)
            .ToList()
            .ForEach(a =>
            {
                a.Amount = a.InitialAmount;
                a.Amount = userTransactions
                    .Where(t => t.Account.Id == a.Id)
                    .Sum(t => t.Type == Type.Income ? t.Amount : -t.Amount);
            });
        
        _context.SaveChanges();
    }
}