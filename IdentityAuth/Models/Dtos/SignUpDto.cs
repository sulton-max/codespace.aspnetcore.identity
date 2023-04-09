namespace IdentityAuth.Models.Dtos;

public class SignUpDto
{
    public string EmailAddress { get; set; } = default!;
    public string Password { get; set; } = default!;
}