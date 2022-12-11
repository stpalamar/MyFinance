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

public class PaymentService : IPaymentService
{
    private ApplicationDbContext _context;
    private readonly User _user;

    public PaymentService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        // var email = httpContextAccessor.HttpContext.User.Claims
        //     .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        // _user = _context.Users.First(u => u.Email == email);
        _user = (httpContextAccessor.HttpContext.Items["User"] as User)!;
    }

    public List<PaymentDto> GetPayments()
    {
        return _context.Payments
            .Include("Account")
            .Where(p => p.Account.User.Id == _user.Id)
            .Select(p => (PaymentDto)p)
            .ToList();
    }

    public PaymentDto GetPaymentById(Guid id)
    {
        if (_context.Payments.Include("Account").Where(p => p.Account.User.Id == _user.Id && p.Id == id)
            .IsNullOrEmpty())
        {
            throw new NotFoundPaymentException();
        }

        return _context.Payments
            .Include("Account")
            .Where(p => p.Account.User.Id == _user.Id && p.Id == id)
            .Select(p => (PaymentDto)p)
            .First();
    }

    public PaymentDto AddPayment(PaymentDto paymentDto)
    {
        var accountWhere = _context.Accounts
            .Where(a => a.User.Id == _user.Id && a.Id == paymentDto.AccountId);

        if (accountWhere.IsNullOrEmpty())
        {
            throw new NotFoundAccountException();
        }

        var account = accountWhere.First();

        var newPayment = new Payment
        {
            Name = paymentDto.Name,
            StartDate = paymentDto.StartDate,
            Type = paymentDto.Type,
            Amount = paymentDto.Amount,
            FreqType = paymentDto.FreqType,
            FreqInterval = paymentDto.FreqInterval,
            RepeatType = paymentDto.RepeatType,
            NumberOfRepeats = paymentDto.NumberOfRepeats,
            EndDate = paymentDto.EndDate,
            Account = account
        };
        _context.Payments.Add(newPayment);
        _context.SaveChanges();

        return (PaymentDto)newPayment;
    }

    public PaymentDto UpdatePayment(PaymentDto paymentDto)
    {
        if (_context.Payments.Include("Account")
            .Where(p => p.Account.User.Id == _user.Id && p.Id == paymentDto.Id).IsNullOrEmpty())
        {
            throw new NotFoundPaymentException();
        }
        
        var dbPayment = _context.Payments
            .Include("Account")
            .First(t => t.Account.User.Id == _user.Id && t.Id == paymentDto.Id);

        dbPayment.Name = paymentDto.Name;
        dbPayment.StartDate = paymentDto.StartDate;
        dbPayment.Type = paymentDto.Type;
        dbPayment.Amount = paymentDto.Amount;
        dbPayment.FreqType = paymentDto.FreqType;
        dbPayment.FreqInterval = paymentDto.FreqInterval;
        dbPayment.RepeatType = paymentDto.RepeatType;
        dbPayment.NumberOfRepeats = paymentDto.NumberOfRepeats;
        dbPayment.EndDate = paymentDto.EndDate;
        dbPayment.Account = _context.Accounts
            .First(a => a.Id == paymentDto.AccountId);

        _context.SaveChanges();

        return paymentDto;
    }

    public void DeletePayment(Guid id)
    {
        if (_context.Payments.Include("Account")
            .Where(t => t.Account.User.Id == _user.Id && t.Id == id).IsNullOrEmpty())
        {
            throw new NotFoundPaymentException();
        }

        _context.Payments
            .Remove(_context.Payments.First(p => p.Account.User.Id == _user.Id && p.Id == id));
        _context.SaveChanges();
    }
}