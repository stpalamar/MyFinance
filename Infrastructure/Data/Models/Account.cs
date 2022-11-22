using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Models;

public class Account
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = null!;

    public double InitialAmount { get; set; }
    public double Amount { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}