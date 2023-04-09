using IdentityAuth.Models.Dtos;

namespace IdentityAuth.Services.IdentityServices.Interfaces;

public interface IIdentityManagerService
{
    ValueTask<bool> SignUpAsync(SignUpDto signUpDetails);

    ValueTask<bool> SignInAsync(SignInDto signInDetails);

    ValueTask SignOutAsync();
}