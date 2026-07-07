using PassXYZLib;

namespace PassXYZ.Server.DTOs.Vault;

public class NewEntryRequest
{
    public ItemSubType Type { get; set; } = ItemSubType.Entry;
    public string Name { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Url { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string? Notes { get; set; }
    public string? OtpUrl { get; set; }
    public Dictionary<string, string>? CustomFields { get; set; }
    public List<FieldDto>? Fields { get; set; }
}