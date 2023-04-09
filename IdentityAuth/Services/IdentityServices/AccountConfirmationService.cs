using System.Text;
using IdentityAuth.Models.Entities;
using IdentityAuth.Services.IdentityServices.Interfaces;
using IdentityAuth.Services.NotificationServices.Interfaces;
using IdentityAuth.Services.RequestServices.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityAuth.Services.IdentityServices;

public class AccountConfirmationService : IAccountConfirmationService
{
    private readonly UserManager<User> _userManager;
    private readonly IRequestService _requestService;
    private readonly IEmailSender _emailSender;

    public AccountConfirmationService(UserManager<User> userManager, IRequestService requestService, IEmailSender emailSender)
    {
        _userManager = userManager;
        _requestService = requestService;
        _emailSender = emailSender;
    }

    public async ValueTask<bool> SendEmailConfirmationEmailAsync(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            throw new ArgumentException("Cannot send confirm email to empty email address", nameof(emailAddress));

        var user = await _userManager.FindByEmailAsync(emailAddress) ??
                   throw new ArgumentException("Cannot send confirm email to empty email address", nameof(emailAddress));
        if (string.IsNullOrWhiteSpace(user.Email))
            throw new InvalidOperationException("Cannot send email confirmation email to user without email address");
        if (user.EmailConfirmed)
            return false;

        // Creating confirmation token
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        // Creating confirmation url
        var confirmationUrl = _requestService.GetActionUrl("ConfirmEmail", "Auth", new
        {
            userId = user.Id,
            token = token
        });

        // Sending confirmation email
        return await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
            $"Please confirm your email by <a href=\"{confirmationUrl}\">clicking here</a>.");
    }

    public async ValueTask<bool> ConfirmEmailAsync(long userId, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Cannot confirm email with empty token", nameof(token));

        token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var user = await _userManager.FindByIdAsync(userId.ToString()) ??
                   throw new InvalidOperationException("Cannot confirm email for non-existing user");
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }
}