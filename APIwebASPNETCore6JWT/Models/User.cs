using Microsoft.AspNetCore.Identity;

namespace APIwebASPNETCore6JWT.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
}