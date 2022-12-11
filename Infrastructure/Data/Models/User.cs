using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [MaxLength(100)]
    public string Email { get; set; } = null!;
    [MaxLength(50)]
    public string FirstName { get; set; } = null!;
    [MaxLength(50)]
    public string LastName { get; set; } = null!;
    [MaxLength(100)]
    public string Password { get; set; } = null!;

    public virtual List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}