namespace PassXYZ.Server.DTOs.Vault;

public class EntryDto : ItemDto
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Url { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string? OtpUrl { get; set; }
    public Dictionary<string, string>? CustomFields { get; set; }
    public List<FieldDto>? Fields { get; set; }
    public string? GroupId { get; set; }
}