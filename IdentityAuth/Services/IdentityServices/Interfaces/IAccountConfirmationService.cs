using IdentityAuth.Models.Entities;

namespace IdentityAuth.Services.IdentityServices.Interfaces;

public interface IAccountConfirmationService
{
    ValueTask<bool> SendEmailConfirmationEmailAsync(string emailAddress);

    ValueTask<bool> ConfirmEmailAsync(long userId, string token);
}