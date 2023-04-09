using IdentityAuth.Models.Files;

namespace IdentityAuth.Services.NotificationServices.Interfaces;

public interface IEmailSender
{
    ValueTask<bool> SendEmailAsync(
        string emailAddress,
        string subject,
        string message,
        MemoryFile? file = default,
        CancellationToken cancellationToken = default
    );
}