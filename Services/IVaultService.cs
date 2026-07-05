using Microsoft.AspNetCore.Http;
using PassXYZ.Server.DTOs.Vault;

namespace PassXYZ.Server.Services;

public interface IVaultService
{
    Task<List<ItemDto>> GetItems(string username, string groupId);
    Task<ItemDto?> GetItem(string username, string itemId);
    Task<EntryDto?> GetEntry(string username, string entryId);
    Task<GroupDto?> GetGroup(string username, string groupId);
    Task<List<EntryDto>> SearchEntries(string username, string keyword);
    Task<List<EntryDto>> GetOtpEntries(string username);
    Task<string> CreateEntry(string username, string groupId, NewEntryRequest request);
    Task<string> CreateGroup(string username, string parentGroupId, NewGroupRequest request);
    Task<bool> UpdateEntry(string username, string entryId, EntryDto request);
    Task<bool> UpdateGroup(string username, string groupId, GroupDto request);
    Task<bool> DeleteEntry(string username, string entryId);
    Task<bool> DeleteGroup(string username, string groupId);
    Task<bool> ChangeMasterPassword(string username, string newPassword);
    Task<List<IconDto>> GetIcons();
    Task<List<AttachmentDto>> GetAttachments(string username, string entryId);
    Task<byte[]?> DownloadAttachment(string username, string entryId, string attachmentId);
    Task<string> UploadAttachment(string username, string entryId, IFormFile file);
    Task<bool> DeleteAttachment(string username, string entryId, string attachmentId);
}