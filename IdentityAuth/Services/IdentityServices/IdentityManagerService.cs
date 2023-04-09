using System.Text;
using IdentityAuth.Models.Constants;
using IdentityAuth.Models.Dtos;
using IdentityAuth.Models.Entities;
using IdentityAuth.Services.IdentityServices.Interfaces;
using IdentityAuth.Services.NotificationServices.Interfaces;
using IdentityAuth.Services.RequestServices.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace IdentityAuth.Services.IdentityServices;

public class IdentityManagerService : IIdentityManagerService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IAccountConfirmationService _accountConfirmationService;
    private readonly IEmailSender _emailSender;
    private readonly ITwoFactorAuthenticationService _twoFactorAuthenticationService;
    private readonly RoleManager<Role> _roleManager;

    public IdentityManagerService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IAccountConfirmationService accountConfirmationService,
        IEmailSender emailSender,
        ITwoFactorAuthenticationService twoFactorAuthenticationService,
        RoleManager<Role> roleManager
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _accountConfirmationService = accountConfirmationService;
        _emailSender = emailSender;
        _twoFactorAuthenticationService = twoFactorAuthenticationService;
        _roleManager = roleManager;
    }

    public async ValueTask<bool> SignUpAsync(SignUpDto signUpDetails)
    {
        // Creating user
        var user = new User
        {
            UserName = signUpDetails.EmailAddress,
            Email = signUpDetails.EmailAddress,
        };
        var result = await _userManager.CreateAsync(user, signUpDetails.Password);

        if (!result.Succeeded)
            return result.Succeeded;

        user = await _userManager.FindByEmailAsync(user.Email) ?? throw new InvalidOperationException("User not found");
        
        // Adding user to role
        await _userManager.AddToRoleAsync(user, Roles.Customer.ToString());

        // Sending confirmation email
        if (_signInManager.Options.SignIn.RequireConfirmedEmail)
            await _accountConfirmationService.SendEmailConfirmationEmailAsync(user.Email!);

        return result.Succeeded;
    }

    public async ValueTask<bool> SignInAsync(SignInDto signInDetails)
    {
        var result = await _signInManager.PasswordSignInAsync(signInDetails.EmailAddress, signInDetails.Password, signInDetails.RememberMe, true);

        // Notifying user if account locked out
        if (result.IsLockedOut)
        {
            var user = await _userManager.FindByEmailAsync(signInDetails.EmailAddress);
            if (user is not null)
                await _emailSender.SendEmailAsync(user.Email, "Account locked out",
                    "Your account has been locked out due to multiple failed login attempts.");
        }

        // Requiring Two Factor Authentication if enabled
        if (result.RequiresTwoFactor)
        {
            var user = await _userManager.FindByEmailAsync(signInDetails.EmailAddress) ??
                       throw new ArgumentException("User not found", nameof(signInDetails.EmailAddress));
            if (string.IsNullOrWhiteSpace(signInDetails.TwoFactorCode))
            {
                await _twoFactorAuthenticationService.SendCodeAsync(user);
                return false;
            }

            // var user = await _userManager.FindByEmailAsync(signInDetails.EmailAddress);
            result = await _signInManager.TwoFactorSignInAsync("Email", signInDetails.TwoFactorCode, true, true);
        }

        return result.Succeeded;
    }

    public async ValueTask SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}