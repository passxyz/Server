namespace PassXYZ.Server.DTOs.Vault;

public class FieldDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsProtected { get; set; }
    public bool IsBinary { get; set; }
    public string? EncodedKey { get; set; }
}