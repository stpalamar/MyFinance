namespace ApplicationCore.DTO;

public class AuthenticatedUserDto
{
    public string AccessToken { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}