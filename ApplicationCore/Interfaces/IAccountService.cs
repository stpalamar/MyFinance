using ApplicationCore.DTO;
using DocumentFormat.OpenXml.InkML;
using Infrastructure;
using Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationCore.Interfaces;

public interface IAccountService
{
    List<AccountDto> GetAccounts();
    AccountDto GetAccountById(Guid id);
    AccountDto AddAccount(AccountDto account);
    AccountDto UpdateAccount(AccountDto account);
    void DeleteAccount(Guid id);
    void CalculateAccountAmount(ApplicationDbContext context, User user);
}