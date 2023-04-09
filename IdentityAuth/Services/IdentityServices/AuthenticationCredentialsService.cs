using System.Text;
using IdentityAuth.Models.Dtos;
using IdentityAuth.Models.Entities;
using IdentityAuth.Services.IdentityServices.Interfaces;
using IdentityAuth.Services.NotificationServices.Interfaces;
using IdentityAuth.Services.RequestServices.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityAuth.Services.IdentityServices;

public class AuthenticationCredentialsService : IAuthenticationCredentialsService
{
    private readonly UserManager<User> _userManager;
    private readonly IRequestService _requestService;
    private readonly IEmailSender _emailSender;

    public AuthenticationCredentialsService(UserManager<User> userManager, IRequestService requestService, IEmailSender emailSender)
    {
        _userManager = userManager;
        _requestService = requestService;
        _emailSender = emailSender;
    }

    public async ValueTask<bool> SendPasswordResetEmailAsync(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            throw new ArgumentException("Cannot send password reset email to empty email address", nameof(emailAddress));

        // Creating password reset token
        var user = await _userManager.FindByEmailAsync(emailAddress) ??
                   throw new InvalidOperationException("Cannot send password reset email to non-existing user");
        if (string.IsNullOrWhiteSpace(user.Email))
            throw new InvalidOperationException("Cannot send email confirmation email to user without email address");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        // Creating password reset url
        var passwordResetUrl = _requestService.GetActionUrl("ResetPassword", "Auth", new
        {
            userId = user.Id,
            token = token
        });

        // Sending password reset email
        return await _emailSender.SendEmailAsync(user.Email, "Reset your password",
            $"Please reset your password by <a href=\"{passwordResetUrl}\">clicking here</a>.");
    }

    public async ValueTask<bool> ChangePasswordAsync(UpdatePasswordDto updatePasswordDetails)
    {
        var user = await _userManager.FindByEmailAsync(updatePasswordDetails.EmailAddress) ??
                   throw new ArgumentException("Cannot change password for non-existing user", nameof(updatePasswordDetails.EmailAddress));

        if (!await _userManager.IsEmailConfirmedAsync(user))
            throw new InvalidOperationException("Email not confirmed");

        if (string.IsNullOrWhiteSpace(updatePasswordDetails.CurrentPassword))
            throw new ArgumentException("Cannot change password without current password", nameof(updatePasswordDetails.CurrentPassword));

        var result = await _userManager.ChangePasswordAsync(user, updatePasswordDetails.CurrentPassword, updatePasswordDetails.NewPassword);
        return result.Succeeded;
    }

    public async ValueTask<bool> ResetPasswordAsync(UpdatePasswordDto updatePasswordDetails)
    {
        var user = await _userManager.FindByEmailAsync(updatePasswordDetails.EmailAddress) ??
                   throw new ArgumentException("Cannot change password for non-existing user", nameof(updatePasswordDetails.EmailAddress));

        if (!await _userManager.IsEmailConfirmedAsync(user))
            throw new InvalidOperationException("Email not confirmed");

        if (string.IsNullOrWhiteSpace(updatePasswordDetails.SecurityToken))
            throw new ArgumentException("Cannot reset password with empty token", nameof(updatePasswordDetails.SecurityToken));
        updatePasswordDetails.SecurityToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(updatePasswordDetails.SecurityToken));

        var result = await _userManager.ResetPasswordAsync(user, updatePasswordDetails.SecurityToken, updatePasswordDetails.NewPassword);
        return result.Succeeded;
    }
}