using APIwebASPNETCore6JWT.Authorization;
using APIwebASPNETCore6JWT.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace APIwebASPNETCore6JWT.Controllers;

[ApiController]
[Route("api")]
public class JTWController : ControllerBase
{
    private readonly ITokenClaimsService _tokenClaimsService;
    private readonly UserManager<ApplicationUser> _userManager;

    public JTWController(
        ITokenClaimsService tokenClaimsService,
        UserManager<ApplicationUser> userManager)
    {
        _tokenClaimsService = tokenClaimsService;
        _userManager = userManager;
    }

    [HttpGet("currentUser")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser() =>
        Ok(User.Identity.IsAuthenticated ?
            await UserInfo.CreateUserInfo(User, await _tokenClaimsService.GetTokenAsync(User.Identity.Name))
            : UserInfo.Anonymous);

    [HttpPost("authenticate")]
    public async Task<ActionResult<AuthenticateResponse>> Authenticate([FromForm] AuthenticateRequest request)
    {
        var response = new AuthenticateResponse();

        var user = await _userManager.FindByNameAsync(request.UserName);

        var succeeded = await _userManager.CheckPasswordAsync(user, request.Password);

        if (succeeded)
        {
            response.Succeeded = succeeded;
            response.Token = await _tokenClaimsService.GetTokenAsync(request.UserName);
        }

        return response;
    }
}

