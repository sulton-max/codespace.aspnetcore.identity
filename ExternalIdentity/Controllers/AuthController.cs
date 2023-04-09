using System.Security.Claims;
using ExternalIdentity.Extensions;
using ExternalIdentity.Models.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExternalIdentity.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("[action]")]
    public async ValueTask<IActionResult> SignIn([FromQuery] SignInDto model)
    {
        var provider = await HttpContext.GetAuthenticationProviderName(model.Provider);
        return !string.IsNullOrWhiteSpace(provider)
            // ? Challenge(provider)
            ? Challenge(new AuthenticationProperties
            {
                RedirectUri = "/api/users/me",
            }, provider)
            : BadRequest();
    }

    [HttpGet("[action]")]
    public new async ValueTask<IActionResult> SignOut()
    {
        await HttpContext.SignOutAsync();
        return base.SignOut();
    }

    [HttpGet("signin-microsoft")]
    public ValueTask<IActionResult> SignInMicrosoft()
    {
        var user = HttpContext.User;
        var identity = new
        {
            UserId = user.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier))?.Value,
            Email = user.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email))?.Value
        };

        return new ValueTask<IActionResult>(Ok(identity));
    }
}