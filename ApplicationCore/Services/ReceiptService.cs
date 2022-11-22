using System.Security.Claims;
using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using Infrastructure;
using Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApplicationCore.Services;

public class ReceiptService : IReceiptService
{
    private ApplicationDbContext _context;
    private readonly User _user;

    public ReceiptService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        var email = httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
        _user = _context.Users.First(u => u.Email == email);
    }

    public void AddReceiptToTransaction(ReceiptUploadDto receiptUploadDto, Guid transactionId)
    {
        if (_context.Transactions
            .Include("Account")
            .Where(t => t.Account.User.Id == _user.Id && t.Id == transactionId)
            .IsNullOrEmpty())
        {
            throw new NotFoundTransactionException();
        }
        
        var transaction =  _context.Transactions
            .First(t => t.Account.User.Id == _user.Id && t.Id == transactionId);

        if (receiptUploadDto.ImageFile.Length <= 0)
        {
            throw new Exception("File is empty");
        };
        using var ms = new MemoryStream();
        {
            receiptUploadDto.ImageFile.CopyTo(ms);
            var fileBytes = ms.ToArray();
            var newReceipt = new Receipt()
            {
                ImageData = fileBytes,
            };
            var addedReceipt = _context.Receipts.Add(newReceipt);
            _context.SaveChanges();
                   
            transaction.Receipt = addedReceipt.Entity;
        }
    }
    
    public Receipt GetReceiptByTransactionId(Guid transactionId)
    {
        if (_context.Transactions
            .Include("Account")
            .Where(t => t.Account.User.Id == _user.Id && t.Id == transactionId)
            .IsNullOrEmpty())
        {
            throw new NotFoundTransactionException();
        }
    
        return _context.Transactions
            .First(t => t.Account.User.Id == _user.Id && t.Id == transactionId)
            .Receipt;
    }
}