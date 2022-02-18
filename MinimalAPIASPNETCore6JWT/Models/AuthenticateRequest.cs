namespace MinimalAPIASPNETCore6JWT.Models;

public class AuthenticateRequest
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
}

