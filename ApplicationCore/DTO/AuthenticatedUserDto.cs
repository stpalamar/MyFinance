namespace ApplicationCore.DTO;

public class AuthenticatedUserDto
{
    public string Token { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}