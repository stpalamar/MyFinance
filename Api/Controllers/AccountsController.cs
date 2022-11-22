using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;
    
    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    
    [HttpGet]
    public IActionResult GetAccounts()
    {
        return Ok(_accountService.GetAccounts());
    }
    
    [HttpGet("{id}", Name = "GetAccount")]
    public IActionResult GetAccount(Guid id)
    {
        return Ok(_accountService.GetAccountById(id));
    }
    
    [HttpPost]
    public IActionResult CreateAccount(AccountDto account)
    {
        var newAccount = _accountService.AddAccount(account);
        return CreatedAtRoute("GetAccount", new { id = newAccount.Id }, newAccount);
    }
    
    [HttpPut]
    public IActionResult UpdateAccount(AccountDto account)
    {
        return Ok(_accountService.UpdateAccount(account));
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteAccount(Guid id)
    {
        _accountService.DeleteAccount(id);
        return Ok();
    }
}