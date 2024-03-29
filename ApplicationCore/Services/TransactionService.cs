﻿using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using Infrastructure;
using Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApplicationCore.Services;

public class TransactionService : ITransactionService
{
    private ApplicationDbContext _context;
    private IAccountService _accountService;
    private readonly User _user;

    public TransactionService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IAccountService accountService)
    {
        _context = context;
        _accountService = accountService;
        _user = (httpContextAccessor.HttpContext.Items["User"] as User)!;
    }

    public List<TransactionDto> GetTransactions()
    {
        return _context.Transactions
            .Include("Account")
            .Include("Receipt")
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
    
    public List<TransactionDto> GetTransactionsByAccountId(Guid accountId)
    {
        if (_context.Accounts
            .Where(a => a.User.Id == _user.Id && a.Id == accountId)
            .IsNullOrEmpty())
        {
            throw new NotFoundAccountException();
        }
        return _context.Transactions
            .Include("Account")
            .Where(t => t.Account.User.Id == _user.Id && t.Account.Id == accountId)
            .Select(t => (TransactionDto)t)
            .ToList();
    }

    public TransactionDto AddTransaction(TransactionDto transaction)
    {
        
        var accountWhere = _context.Accounts
            .Where(a => a.User.Id == _user.Id && a.Id == transaction.AccountId);

        if (accountWhere.IsNullOrEmpty())
        {
            throw new NotFoundAccountException();
        }
        
        var account = accountWhere.First();

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

        _accountService.CalculateAccountAmount(_context, _user);
        return (TransactionDto)newTransaction;
    }

    public TransactionDto UpdateTransaction(TransactionDto transaction)
    {
        if (_context.Transactions.Include("Account")
            .Where(t => t.Account.User.Id == _user.Id && t.Id == transaction.Id).IsNullOrEmpty())
        {
            throw new NotFoundTransactionException();
        }
        
        var dbTransaction = _context.Transactions
            .Include("Account")
            .First(t => t.Account.User.Id == _user.Id && t.Id == transaction.Id);
        
        var dbAccount = _context.Accounts
            .First(a => a.User.Id == _user.Id && a.Id == transaction.AccountId);

        dbTransaction.Description = transaction.Description;
        dbTransaction.Date = transaction.Date;
        dbTransaction.Type = transaction.Type;
        dbTransaction.Amount = transaction.Amount;
        dbTransaction.Account = dbAccount;

        _context.SaveChanges();

        _accountService.CalculateAccountAmount(_context, _user);
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
        _accountService.CalculateAccountAmount(_context, _user);
    }

    public TransactionDto DuplicateTransaction(Guid id)
    {
        if (_context.Transactions
            .Include("Account")
            .Where(t => t.Account.User.Id == _user.Id && t.Id == id).IsNullOrEmpty())
        {
            throw new NotFoundTransactionException();
        }

        var transaction = _context.Transactions
            .Include("Account")
            .Include("Receipt")
            .First(t => t.Account.User.Id == _user.Id && t.Id == id);
        var clone = (Transaction)transaction.Clone();
        _context.Transactions.Add(clone);
        _context.SaveChanges();
        return (TransactionDto)clone;
    }
}