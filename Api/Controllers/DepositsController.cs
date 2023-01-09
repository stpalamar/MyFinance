using Api.Authorization;
using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class DepositsController : ControllerBase
{
    private readonly IDepositService _depositService;
    
    public DepositsController(IDepositService depositService)
    {
        _depositService = depositService;
    }
    
    [HttpGet]
    public IActionResult GetAccounts()
    {
        return Ok(_depositService.GetDeposits());
    }
    
    [HttpGet("{id}", Name = "GetDeposit")]
    public IActionResult GetAccount(Guid id)
    {
        return Ok(_depositService.GetDepositById(id));
    }
    
    [HttpPost]
    public IActionResult CreateDeposit(DepositDto deposit)
    {
        var newDeposit = _depositService.AddDeposit(deposit);
        return CreatedAtRoute("GetDeposit", new { id = newDeposit.Id }, newDeposit);
    }
    
    [HttpPut]
    public IActionResult UpdateDeposit(DepositDto deposit)
    {
        return Ok(_depositService.UpdateDeposit(deposit));
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteDeposit(Guid id)
    {
        _depositService.DeleteDeposit(id);
        return Ok();
    }
}