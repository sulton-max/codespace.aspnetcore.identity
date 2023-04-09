namespace IdentityAuth.Models.Configuration;

public class EmailSenderConfiguration
{
    public static string Position => "EmailSenderConfiguration";
    
    public string EmailAddress { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string SmtpServer { get; set; } = default!;

    public int SmtpPort { get; set; } = default!;
}