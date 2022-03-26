using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APIwebASPNETCore6JWT.Entities;
using APIwebASPNETCore6JWT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace APIwebASPNETCore6JWT.Controllers;

[ApiController]
[Route("")]
public class JTWController : ControllerBase
{
    private readonly IConfiguration _configuration;

    private readonly ILogger<JTWController> _logger;

    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public JTWController(
        ILogger<JTWController> logger,
        IConfiguration configuration,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
        _contextAccessor = contextAccessor;
    }

    /// <summary>
    /// Get info user
    /// </summary>
    /// <returns></returns>
    [HttpGet("me")]
    [Authorize]
    public ActionResult Get()
    {
        var user = _contextAccessor?.HttpContext?.User;

        return Ok(new
        {
            Claims = user?.Claims.Select(s => new
            {
                s.Type,
                s.Value
            }).ToList(),
            user?.Identity?.Name,
            user?.Identity?.IsAuthenticated,
            user?.Identity?.AuthenticationType
        });
    }

    /// <summary>
    /// Get access token
    /// </summary>
    /// <param name="request"></param>
    /// <returns>AccessToken</returns>
    [HttpPost("token")]
    public async Task<ActionResult> Post([FromForm] AuthenticateRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Forbid();
        }

        return Ok(new
        {
            AccessToken = await GetJwt(user)
        });
    }


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
}

