using System.Net;
using System.Net.Mail;
using IdentityAuth.Models.Configuration;
using IdentityAuth.Models.Files;
using IdentityAuth.Services.NotificationServices.Interfaces;
using Microsoft.Extensions.Options;

namespace IdentityAuth.Services.NotificationServices;

public class EmailSender : IEmailSender
{
    private readonly EmailSenderConfiguration _emailSenderConfiguration;

    public EmailSender(IOptions<EmailSenderConfiguration> emailSenderConfiguration)
    {
        _emailSenderConfiguration = emailSenderConfiguration.Value;
    }

    public async ValueTask<bool> SendEmailAsync(
        string emailAddress,
        string subject,
        string message,
        MemoryFile? file = default,
        CancellationToken cancellationToken = default
    )
    {
        // Creating a mail message
        var mail = new MailMessage(_emailSenderConfiguration.EmailAddress, emailAddress, subject, message);

        if (file is not null)
            mail.Attachments.Add(new Attachment(file.Content, file.Name));

        mail.IsBodyHtml = true;

        // Creating SMTP client
        var smtpClient = new SmtpClient
        {
            Host = _emailSenderConfiguration.SmtpServer,
            Port = _emailSenderConfiguration.SmtpPort,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_emailSenderConfiguration.EmailAddress, _emailSenderConfiguration.Password)
        };

        // Sending mail
        var result = true;
        try
        {
            await smtpClient.SendMailAsync(mail, cancellationToken);
            // await smtpClient.SendMailAsync(_emailSenderConfiguration.EmailAddress, emailAddress, subject, message, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            result = false;
        }

        return result;
    }
}