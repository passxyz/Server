using PassXYZLib;

namespace PassXYZ.Server.DTOs.Vault;

public class ItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ItemSubType Type { get; set; }
    public bool IsGroup { get; set; }
    public DateTime LastModified { get; set; }
    public string? Icon { get; set; }
}