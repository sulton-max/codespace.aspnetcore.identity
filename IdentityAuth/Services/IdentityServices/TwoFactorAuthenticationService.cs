using IdentityAuth.Models.Entities;
using IdentityAuth.Models.Files;
using IdentityAuth.Services.IdentityServices.Interfaces;
using IdentityAuth.Services.NotificationServices.Interfaces;
using Microsoft.AspNetCore.Identity;
using QRCoder;

namespace IdentityAuth.Services.IdentityServices;

public class TwoFactorAuthenticationService : ITwoFactorAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailSender _emailSender;

    public TwoFactorAuthenticationService(UserManager<User> userManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    public async ValueTask<bool> EnableAsync(User user, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Two factor authentication token is required", nameof(token));

        if (await _userManager.GetTwoFactorEnabledAsync(user))
            return true;
        
        return await _userManager.VerifyTwoFactorTokenAsync(user, "Email", token) &&
               (await _userManager.SetTwoFactorEnabledAsync(user, true)).Succeeded;
    }

    public async ValueTask<bool> DisableAsync(User user, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Two factor authentication token is required", nameof(token));

        if (!await _userManager.GetTwoFactorEnabledAsync(user))
            return true;

        return await _userManager.VerifyTwoFactorTokenAsync(user, "Email", token) &&
               (await _userManager.SetTwoFactorEnabledAsync(user, false)).Succeeded;
    }

    public async ValueTask<bool> SendCodeAsync(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Email) || !user.EmailConfirmed)
            throw new ArgumentException("User email is required and must be confirmed", nameof(user.Email));

        var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
        var recoveryCode = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

        var qrCodeGenerator = new QRCodeGenerator();
        var qrCodeData = qrCodeGenerator.CreateQrCode(token, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        var file = new MemoryFile
        {
            Name = "2fa.png",
            Content = new MemoryStream(qrCode.GetGraphic(20))
        };

        return await _emailSender.SendEmailAsync(user.Email, "Two factor authentication code",
            $"Scan the QR code to enable two factor authentication on your device. Save the recovery code : {string.Join(" ", recoveryCode!)}", file);
    }
}