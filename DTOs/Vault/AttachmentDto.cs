namespace PassXYZ.Server.DTOs.Vault;

public class AttachmentDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long Size { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public bool IsImage { get; set; }
}