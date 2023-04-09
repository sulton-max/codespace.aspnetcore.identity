using IdentityAuth.Models.Entities;
using IdentityAuth.Services.IdentityServices.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAuth.Controllers;

[Route("/api/2fa")]
public class TwoFactorAuthController : CustomControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ITwoFactorAuthenticationService _twoFactorAuthenticationService;

    public TwoFactorAuthController(UserManager<User> userManager, ITwoFactorAuthenticationService twoFactorAuthenticationService)
    {
        _userManager = userManager;
        _twoFactorAuthenticationService = twoFactorAuthenticationService;
    }

    [HttpPost("enable")]
    public async ValueTask<IActionResult> Enable([FromQuery]string token)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        return user != null ? await _twoFactorAuthenticationService.EnableAsync(user, token) ? Ok() : BadRequest() : Unauthorized();
    }

    [HttpPost("disable")]
    public async ValueTask<IActionResult> Disable([FromQuery]string token)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        return user != null ? await _twoFactorAuthenticationService.DisableAsync(user, token) ? Ok() : BadRequest() : Unauthorized();
    }

    [HttpPost("send-code")]
    public async ValueTask<IActionResult> SendCode()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        return user != null ? await _twoFactorAuthenticationService.SendCodeAsync(user) ? Ok() : BadRequest() : Unauthorized();
    }
}