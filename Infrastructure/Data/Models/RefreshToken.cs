﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Models;

[Owned]
public class RefreshToken
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public string Token { get; set; } = null!;
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public string CreatedByIp { get; set; } = null!;
    public DateTime? Revoked { get; set; }
    public string RevokedByIp { get; set; }
    public string ReplacedByToken { get; set; }
    public string ReasonRevoked { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked => Revoked != null;
    public bool IsActive => !IsRevoked && !IsExpired;
}