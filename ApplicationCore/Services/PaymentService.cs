using System.Security.Claims;
using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using Infrastructure;
using Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace ApplicationCore.Services;

public class PaymentService : IPaymentService
{
    private ApplicationDbContext _context;
    private readonly User _user;

    public PaymentService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        var email = httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
        _user = _context.Users.First(u => u.Email == email);
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
            RepeatType = paymentDto.RepeatType,
            NumberOfRepeats = paymentDto.NumberOfRepeats,
            EndDate = paymentDto.EndDate,
            Type = paymentDto.Type,
            Amount = paymentDto.Amount,
            FreqType = paymentDto.FreqType,
            FreqInterval = paymentDto.FreqInterval,
            Account = account
        };
        _context.Payments.Add(newPayment);
        _context.SaveChanges();

        return (PaymentDto)newPayment;
    }
}