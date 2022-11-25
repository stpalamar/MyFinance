using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public IActionResult GetTransactions()
    {
        return Ok(_transactionService.GetTransactions());
    }

    [HttpGet("{id}", Name = "GetTransaction")]
    public IActionResult GetTransaction(Guid id)
    {
        return Ok(_transactionService.GetTransactionById(id));
    }

    [HttpGet("Account/{id}")]
    public IActionResult GetTransactionByAccountId(Guid id)
    {
        return Ok(_transactionService.GetTransactionsByAccountId(id));
    }

    [HttpPost]
    public IActionResult AddTransaction(TransactionDto transaction)
    {
        var newTransaction = _transactionService.AddTransaction(transaction);
        return CreatedAtRoute("GetTransaction", new { id = newTransaction.Id }, newTransaction);
    }

    [HttpPut]
    public IActionResult UpdateTransaction(TransactionDto transaction)
    {
        return Ok(_transactionService.UpdateTransaction(transaction));
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTransaction(Guid id)
    {
        _transactionService.DeleteTransaction(id);
        return Ok();
    }
    
}