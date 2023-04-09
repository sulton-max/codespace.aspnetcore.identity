namespace IdentityAuth.Models.Files;

public class MemoryFile : File
{
    public Stream Content { get; set; } = default!;
}