namespace IdentityAuth.Models.Dtos;

public class UpdatePasswordDto
{
    public string EmailAddress { get; set; } = default!;

    public string? CurrentPassword { get; set; } = default!;

    public string NewPassword { get; set; } = default!;

    public string? SecurityToken { get; set; }
}