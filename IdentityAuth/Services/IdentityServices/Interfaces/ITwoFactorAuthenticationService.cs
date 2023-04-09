using IdentityAuth.Models.Entities;

namespace IdentityAuth.Services.IdentityServices.Interfaces;

public interface ITwoFactorAuthenticationService
{
    ValueTask<bool> EnableAsync(User user, string token);

    ValueTask<bool> DisableAsync(User user, string token);

    ValueTask<bool> SendCodeAsync(User user);
}