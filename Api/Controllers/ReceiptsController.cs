using System.Net;
using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;


[Authorize]
[ApiController]
[Route("[controller]")]
public class ReceiptsController : ControllerBase
{
    private readonly IReceiptService _receiptService;

    public ReceiptsController(IReceiptService receiptService)
    {
        _receiptService = receiptService;
    }
    
    [HttpGet("{id}")]
    public IActionResult GetReceiptByTransactionId(Guid id)
    {
        return Ok(_receiptService.GetReceiptByTransactionId(id));
    }
    
    [HttpPost("{id}")]
    public IActionResult AddReceiptToTransaction([FromForm] ReceiptUploadDto receiptUploadDto, Guid id)
    {
        _receiptService.AddReceiptToTransaction(receiptUploadDto, id);
        return Ok();
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteReceiptFromTransaction(Guid id)
    {
        _receiptService.DeleteReceiptFromTransaction(id);
        return Ok();
    }
}