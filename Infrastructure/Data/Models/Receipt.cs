using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Models;

public class Receipt
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public byte[] ImageData { get; set; } = null!;
}