namespace PassXYZ.Server.DTOs.Vault;

public class GroupDto : ItemDto
{
    public int ChildCount { get; set; }
    public List<ItemDto>? Children { get; set; }
    public string? ParentId { get; set; }
}