using Microsoft.AspNetCore.Http;

namespace ApplicationCore.DTO;

public class ReceiptDto
{
    public string ImageDataBase64 { get; set; } = null!;
}