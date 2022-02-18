using Microsoft.AspNetCore.Identity;

namespace MinimalAPIASPNETCore6JWT.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
}