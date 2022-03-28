namespace MinimalAPIASPNETCore6JWT.Models;

public class AuthenticateResponse
{
    public AuthenticateResponse()
    {
    }

    public bool Succeeded { get; set; }
    public string Token { get; set; } = string.Empty;
}
