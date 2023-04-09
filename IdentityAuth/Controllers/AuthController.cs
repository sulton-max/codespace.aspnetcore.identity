using IdentityAuth.Models.Dtos;
using IdentityAuth.Services.IdentityServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAuth.Controllers;

public class AuthController : CustomControllerBase
{
    private readonly IIdentityManagerService _identityManagerService;
    private readonly IAccountConfirmationService _accountConfirmationService;
    private readonly IAuthenticationCredentialsService _authenticationCredentialsService;

    public AuthController(
        IIdentityManagerService identityManagerService,
        IAccountConfirmationService accountConfirmationService,
        IAuthenticationCredentialsService authenticationCredentialsService
    )
    {
        _identityManagerService = identityManagerService;
        _accountConfirmationService = accountConfirmationService;
        _authenticationCredentialsService = authenticationCredentialsService;
    }

    [AllowAnonymous]
    [HttpPost("signup")]
    public async ValueTask<IActionResult> SignUp([FromBody] SignUpDto model)
    {
        var result = await _identityManagerService.SignUpAsync(model);
        return result ? Ok() : BadRequest();
    }

    [AllowAnonymous]
    [HttpPost("signin")]
    public async ValueTask<IActionResult> SignIn([FromBody] SignInDto model)
    {
        var result = await _identityManagerService.SignInAsync(model);
        return result ? Ok() : BadRequest();
    }

    [HttpPost("signout")]
    public new async ValueTask<IActionResult> SignOut()
    {
        await _identityManagerService.SignOutAsync();
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("email/confirm")]
    public async ValueTask<IActionResult> ConfirmEmail([FromQuery] long userId, [FromQuery] string token)
    {
        var result = await _accountConfirmationService.ConfirmEmailAsync(userId, token);
        return result ? Ok() : BadRequest();
    }

    [AllowAnonymous]
    [HttpPost("email/send-confim")]
    public async ValueTask<IActionResult> SendEmailConfirmationEmail([FromQuery] string emailAddress)
    {
        var result = await _accountConfirmationService.SendEmailConfirmationEmailAsync(emailAddress);
        return result ? Ok() : Conflict();
    }

    [HttpPut("password/change")]
    public async ValueTask<IActionResult> ChangePassword([FromBody] UpdatePasswordDto model)
    {
        var result = await _authenticationCredentialsService.ChangePasswordAsync(model);
        return result ? Ok() : BadRequest();
    }

    [AllowAnonymous]
    [HttpPut("password/reset")]
    public async ValueTask<IActionResult> ResetPassword([FromBody] UpdatePasswordDto model)
    {
        var result = await _authenticationCredentialsService.ResetPasswordAsync(model);
        return result ? Ok() : BadRequest();
    }

    [AllowAnonymous]
    [HttpPost("password/send-reset")]
    public async ValueTask<IActionResult> SendResetEmail([FromQuery] string emailAddress)
    {
        var result = await _authenticationCredentialsService.SendPasswordResetEmailAsync(emailAddress);
        return result ? Ok() : Conflict();
    }
    
    
}