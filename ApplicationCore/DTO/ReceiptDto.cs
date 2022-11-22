using Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore.DTO;

public class ReceiptDto
{
    public Guid Id { get; set; }
    public IFormFile ImageFile { get; set; }
    
}