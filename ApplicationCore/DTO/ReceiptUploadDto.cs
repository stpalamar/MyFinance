using Microsoft.AspNetCore.Http;

namespace ApplicationCore.DTO;

public class ReceiptUploadDto
{
    public Guid Id { get; set; }
    public IFormFile ImageFile { get; set; }
    
}