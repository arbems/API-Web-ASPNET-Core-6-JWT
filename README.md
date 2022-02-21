# ASP.NET Core 6 API web & JSON Web Token
Seguridad en servicios Web API de .NET Core usando JSON Web Tokens.

En este proyecto exploraremos las características de los JSON Web Tokens, su composición y su implementación utilizando:

* ASP.NET Core 6
* ASP.NET Core Identity.
* Entity Framework Core
* JSON Web Token

```C#
private async Task<string> GetJwt(User user)
{
    var roles = await _userManager.GetRolesAsync(user);
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Sid, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}")
    };

    foreach (var role in roles)
    {
        claims.Add(new Claim(ClaimTypes.Role, role));
    }

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
    var tokenDescriptor = new JwtSecurityToken(
        issuer: _configuration["JwtSettings:Issuer"],
        audience: _configuration["JwtSettings:Audience"],
        claims: claims,
        expires: DateTime.Now.AddMinutes(720),
        signingCredentials: credentials);

    var jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

    return jwt;
}
```
