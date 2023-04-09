using IdentityAuth.Models.Dtos;

namespace IdentityAuth.Services.IdentityServices.Interfaces;

public interface IAuthenticationCredentialsService
{
    ValueTask<bool> SendPasswordResetEmailAsync(string emailAddress);

    ValueTask<bool> ChangePasswordAsync(UpdatePasswordDto updatePasswordDetails);

    ValueTask<bool> ResetPasswordAsync(UpdatePasswordDto updatePasswordDetails);
}