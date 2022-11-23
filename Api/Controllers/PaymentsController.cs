using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    
    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    [HttpGet]
    public IActionResult GetPayments()
    {
        return Ok(_paymentService.GetPayments());
    }
    
    [HttpGet("{id}", Name = "GetPayment")]
    public IActionResult GetPaymentById(Guid id)
    {
        return Ok(_paymentService.GetPaymentById(id));
    }
    
    [HttpPost]
    public IActionResult AddPayment(PaymentDto paymentDto)
    {
        return Ok(_paymentService.AddPayment(paymentDto));
    }
    
    [HttpPut]
    public IActionResult UpdatePayment(PaymentDto paymentDto)
    {
        return Ok(_paymentService.UpdatePayment(paymentDto));
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePayment(Guid id)
    {
        _paymentService.DeletePayment(id);
        return Ok();

    }
}