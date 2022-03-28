namespace APIwebASPNETCore6JWT.Controllers;

public class AuthenticateRequest
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
}