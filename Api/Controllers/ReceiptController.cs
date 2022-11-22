using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;


[Authorize]
[ApiController]
[Route("[controller]")]
public class ReceiptController : ControllerBase
{
    private readonly IReceiptService _receiptService;

    public ReceiptController(IReceiptService receiptService)
    {
        _receiptService = receiptService;
    }
    
    [HttpPost("{id}")]
    public IActionResult AddReceiptToTransaction(ReceiptDto receiptDto, Guid id)
    {
        _receiptService.AddReceiptToTransaction(receiptDto, id);
        return Ok();
    }
    
    // [HttpGet("{id}")]
    // public IActionResult GetReceiptOfTransaction(Guid id)
    // {
    //     _receiptService.GetReceiptByTransactionId(id);
    //     return Ok();
    // }
}