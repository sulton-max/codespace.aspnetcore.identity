using System.Net;
using ClaimsManagement.Extensions;
using ClaimsManagement.Models.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet("[action]")]
    public async ValueTask<IActionResult> SignIn([FromQuery] SignInDto model)
    {
        var providerName = await HttpContext.GetProviderName(model.Provider);
        return !string.IsNullOrWhiteSpace(providerName)
            ? Challenge(new AuthenticationProperties
            {
                RedirectUri = "/"
            }, providerName)
            : BadRequest();
    }
}