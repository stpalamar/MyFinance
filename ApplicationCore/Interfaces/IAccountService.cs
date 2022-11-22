using ApplicationCore.DTO;

namespace ApplicationCore.Interfaces;

public interface IAccountService
{
    List<AccountDto> GetAccounts();
    AccountDto GetAccountById(Guid id);
    AccountDto AddAccount(AccountDto account);
    AccountDto UpdateAccount(AccountDto account);
    void DeleteAccount(Guid id);
}