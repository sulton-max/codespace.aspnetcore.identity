namespace IdentityAuth.Models.Dtos;

public class SignInDto
{
    public string EmailAddress { get; set; } = default!;
    public string Password { get; set; } = default!;

    public bool RememberMe { get; set; }

    public string? TwoFactorCode { get; set; }
}