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
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        _user = _context.Users.First(u => u.Email == email);
    }
    public ReceiptDto GetReceiptByTransactionId(Guid transactionId)
    {
        if (_context.Transactions
            .Include("Account")
            .Where(t => t.Account.User.Id == _user.Id && t.Id == transactionId)
            .IsNullOrEmpty())
        {
            throw new NotFoundTransactionException();
        }
        

        var imageData = _context.Transactions
            .Include("Account")
            .Include("Receipt")
            .First(t => t.Account.User.Id == _user.Id && t.Id == transactionId)
            .Receipt?.ImageData;
        if (imageData.IsNullOrEmpty())
        {
            throw new NoReceiptOfTransactionException();
        }
        ReceiptDto receiptDto = new()
        {
            ImageDataBase64 = Convert.ToBase64String(imageData)
        };
        return receiptDto;
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

        if (receiptUploadDto.ImageData.Length <= 0)
        {
            throw new Exception("File is empty");
        }

        ;
        using var ms = new MemoryStream();
        {
            receiptUploadDto.ImageData.CopyTo(ms);
            var fileBytes = ms.ToArray();
            var newReceipt = new Receipt()
            {
                ImageData = fileBytes,
            };
            var addedReceipt = _context.Receipts.Add(newReceipt);
            _context.Transactions
                .First(t => t.Account.User.Id == _user.Id && t.Id == transactionId)
                .Receipt = addedReceipt.Entity;
            _context.SaveChanges();
        }
    }

    public void DeleteReceiptFromTransaction(Guid transactionId)
    {
        if (_context.Transactions
            .Include("Account")
            .Where(t => t.Account.User.Id == _user.Id && t.Id == transactionId)
            .IsNullOrEmpty())
        {
            throw new NotFoundTransactionException();
        }

        var transaction = _context.Transactions
            .Include("Account")
            .Include("Receipt")
            .First(t => t.Account.User.Id == _user.Id && t.Id == transactionId);
        
        if (transaction.Receipt == null)
        {
            throw new NotFoundTransactionReceiptException();
        }

        var receipt = transaction.Receipt;
        
        transaction.Receipt = null;
        _context.SaveChanges();

        if (_context.Transactions
            .Include("Account")
            .Include("Receipt")
            .Where(t => t.Receipt != null 
                        && t.Account.User.Id == _user.Id 
                        && t.Receipt.Id == receipt.Id)
            .IsNullOrEmpty())
        {
            _context.Receipts.Remove(receipt);
            _context.SaveChanges();
        }
    }
}