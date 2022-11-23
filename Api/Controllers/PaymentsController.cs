using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using Infrastructure.Data.Models;
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
    
    [HttpPost]
    public IActionResult AddPayment(PaymentDto paymentDto)
    {
        return Ok(_paymentService.AddPayment(paymentDto));
    }
}