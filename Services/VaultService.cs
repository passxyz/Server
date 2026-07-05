using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Serialization;
using PassXYZ.Server.DTOs.Vault;
using PassXYZLib;
using Microsoft.AspNetCore.Http;

namespace PassXYZ.Server.Services;

public class VaultService : IVaultService
{
    private readonly IVaultSessionManager _vaultSessionManager;
    private readonly IConfiguration _configuration;

    public VaultService(IVaultSessionManager vaultSessionManager, IConfiguration configuration)
    {
        _vaultSessionManager = vaultSessionManager;
        _configuration = configuration;
    }

    public async Task<List<ItemDto>> GetItems(string username, string groupId)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            return new List<ItemDto>();
        }

        var group = GetGroupById(db, groupId);
        if (group == null)
        {
            return new List<ItemDto>();
        }

        var items = new List<ItemDto>();
        foreach (var g in group.Groups)
        {
            items.Add(ConvertGroupToDto(g));
        }
        foreach (var e in group.Entries)
        {
            items.Add(ConvertEntryToDto(e));
        }
        return items;
    }

    public async Task<ItemDto?> GetItem(string username, string itemId)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            return null;
        }

        var uuid = new PwUuid(Guid.Parse(itemId).ToByteArray());
        
        var entry = db.RootGroup.FindEntry(uuid, true);
        if (entry != null)
        {
            return ConvertEntryToDto(entry);
        }

        var group = db.RootGroup.FindGroup(uuid, true);
        if (group != null)
        {
            return ConvertGroupToDto(group);
        }

        return null;
    }

    public async Task<EntryDto?> GetEntry(string username, string entryId)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            return null;
        }

        var uuid = new PwUuid(Guid.Parse(entryId).ToByteArray());
        var entry = db.RootGroup.FindEntry(uuid, true);
        
        return entry != null ? ConvertEntryToDto(entry) : null;
    }

    public async Task<GroupDto?> GetGroup(string username, string groupId)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            return null;
        }

        var group = GetGroupById(db, groupId);
        return group != null ? ConvertGroupToDto(group) as GroupDto : null;
    }

    public async Task<List<EntryDto>> SearchEntries(string username, string keyword)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            return new List<EntryDto>();
        }

        if (string.IsNullOrWhiteSpace(keyword))
        {
            return db.RootGroup.GetEntries(true)
                .Select(e => ConvertEntryToDto(e))
                .ToList();
        }

        var searchResults = db.RootGroup.GetEntries(true)
            .Where(e => e.Strings.ReadSafe(PwDefs.TitleField).Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        e.Strings.ReadSafe(PwDefs.UserNameField).Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        e.Strings.ReadSafe(PwDefs.UrlField).Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .Select(e => ConvertEntryToDto(e))
            .ToList();

        return searchResults;
    }

    public async Task<List<EntryDto>> GetOtpEntries(string username)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            return new List<EntryDto>();
        }

        return db.RootGroup.GetEntries(true)
            .Where(e => !string.IsNullOrEmpty(GetOtpUrl(e)))
            .Select(e => ConvertEntryToDto(e))
            .ToList();
    }

    public async Task<string> CreateEntry(string username, string groupId, NewEntryRequest request)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            throw new InvalidOperationException("Session not found");
        }

        var group = GetGroupById(db, groupId);
        if (group == null)
        {
            throw new InvalidOperationException("Group not found");
        }

        var entry = new PwEntry(true, true);
        entry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, request.Name));
        
        if (!string.IsNullOrEmpty(request.Username))
        {
            entry.Strings.Set(PwDefs.UserNameField, new ProtectedString(true, request.Username));
        }
        
        if (!string.IsNullOrEmpty(request.Password))
        {
            entry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, request.Password));
        }
        
        if (!string.IsNullOrEmpty(request.Url))
        {
            entry.Strings.Set(PwDefs.UrlField, new ProtectedString(true, request.Url));
        }
        
        if (!string.IsNullOrEmpty(request.Email))
        {
            entry.Strings.Set("Email", new ProtectedString(true, request.Email));
        }
        
        if (!string.IsNullOrEmpty(request.Mobile))
        {
            entry.Strings.Set("Mobile", new ProtectedString(true, request.Mobile));
        }
        
        if (!string.IsNullOrEmpty(request.Notes))
        {
            entry.Strings.Set(PwDefs.NotesField, new ProtectedString(true, request.Notes));
        }

        if (request.CustomFields != null)
        {
            foreach (var (key, value) in request.CustomFields)
            {
                entry.Strings.Set(key, new ProtectedString(true, value));
            }
        }

        group.AddEntry(entry, true);
        await SaveDatabase(db);

        return new Guid(entry.Uuid.UuidBytes).ToString();
    }

    public async Task<string> CreateGroup(string username, string parentGroupId, NewGroupRequest request)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            throw new InvalidOperationException("Session not found");
        }

        var parentGroup = GetGroupById(db, parentGroupId);
        if (parentGroup == null)
        {
            throw new InvalidOperationException("Parent group not found");
        }

        var group = new PwGroup(true, true, request.Name, PwIcon.Folder);
        parentGroup.AddGroup(group, true);
        await SaveDatabase(db);

        return new Guid(group.Uuid.UuidBytes).ToString();
    }

    public async Task<bool> UpdateEntry(string username, string entryId, EntryDto request)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            return false;
        }

        var uuid = new PwUuid(Guid.Parse(entryId).ToByteArray());
        var entry = db.RootGroup.FindEntry(uuid, true);
        
        if (entry == null)
        {
            return false;
        }

        entry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, request.Name));
        
        if (!string.IsNullOrEmpty(request.Username))
        {
            entry.Strings.Set(PwDefs.UserNameField, new ProtectedString(true, request.Username));
        }
        
        if (!string.IsNullOrEmpty(request.Password))
        {
            entry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, request.Password));
        }
        
        if (!string.IsNullOrEmpty(request.Url))
        {
            entry.Strings.Set(PwDefs.UrlField, new ProtectedString(true, request.Url));
        }
        
        if (!string.IsNullOrEmpty(request.Email))
        {
            entry.Strings.Set("Email", new ProtectedString(true, request.Email));
        }
        
        if (!string.IsNullOrEmpty(request.Mobile))
        {
            entry.Strings.Set("Mobile", new ProtectedString(true, request.Mobile));
        }
        
        if (!string.IsNullOrEmpty(request.Notes))
        {
            entry.Strings.Set(PwDefs.NotesField, new ProtectedString(true, request.Notes));
        }

        if (!string.IsNullOrEmpty(request.OtpUrl))
        {
            entry.Strings.Set("OTP", new ProtectedString(true, request.OtpUrl));
        }

        if (request.CustomFields != null)
        {
            foreach (var (key, value) in request.CustomFields)
            {
                entry.Strings.Set(key, new ProtectedString(true, value));
            }
        }

        if (!string.IsNullOrEmpty(request.Icon))
        {
            entry.IconId = (PwIcon)uint.Parse(request.Icon);
        }

        await SaveDatabase(db);

        return true;
    }

    public async Task<bool> UpdateGroup(string username, string groupId, GroupDto request)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            return false;
        }

        var group = GetGroupById(db, groupId);
        if (group == null || group.ParentGroup == null)
        {
            return false;
        }

        group.Name = request.Name;

        if (!string.IsNullOrEmpty(request.Icon))
        {
            group.IconId = (PwIcon)uint.Parse(request.Icon);
        }

        await SaveDatabase(db);

        return true;
    }

    public async Task<bool> DeleteEntry(string username, string entryId)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            return false;
        }

        var uuid = new PwUuid(Guid.Parse(entryId).ToByteArray());
        var entry = db.RootGroup.FindEntry(uuid, true);
        
        if (entry == null)
        {
            return false;
        }

        entry.ParentGroup?.Entries.Remove(entry);
        await SaveDatabase(db);

        return true;
    }

    public async Task<bool> DeleteGroup(string username, string groupId)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            return false;
        }

        var group = GetGroupById(db, groupId);
        if (group == null || group.ParentGroup == null)
        {
            return false;
        }

        group.ParentGroup.Groups.Remove(group);
        await SaveDatabase(db);

        return true;
    }

    public async Task<bool> ChangeMasterPassword(string username, string newPassword)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            return false;
        }

        var newCompositeKey = new CompositeKey();
        newCompositeKey.AddUserKey(new KcpPassword(newPassword));

        db.MasterKey = newCompositeKey;
        
        try
        {
            db.Save(null);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task<List<IconDto>> GetIcons()
    {
        var icons = new List<IconDto>();
        foreach (var icon in Enum.GetValues(typeof(PwIcon)).Cast<PwIcon>())
        {
            icons.Add(new IconDto
            {
                Id = (int)icon,
                Name = icon.ToString()
            });
        }
        return Task.FromResult(icons);
    }

    public async Task<List<AttachmentDto>> GetAttachments(string username, string entryId)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            return new List<AttachmentDto>();
        }

        var uuid = new PwUuid(Guid.Parse(entryId).ToByteArray());
        var entry = db.RootGroup.FindEntry(uuid, true);
        if (entry == null)
        {
            return new List<AttachmentDto>();
        }

        var attachments = new List<AttachmentDto>();
        foreach (var binary in entry.Binaries)
        {
            var data = binary.Value.ReadData();
            var extension = Path.GetExtension(binary.Key)?.ToLowerInvariant();
            var isImage = extension is ".png" or ".jpg" or ".jpeg" or ".gif" or ".bmp" or ".svg" or ".webp";

            attachments.Add(new AttachmentDto
            {
                Id = binary.Key,
                Name = binary.Key,
                Size = data.Length,
                ContentType = GetContentType(extension),
                IsImage = isImage
            });
        }

        return attachments;
    }

    public async Task<byte[]?> DownloadAttachment(string username, string entryId, string attachmentId)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            return null;
        }

        var uuid = new PwUuid(Guid.Parse(entryId).ToByteArray());
        var entry = db.RootGroup.FindEntry(uuid, true);
        if (entry == null)
        {
            return null;
        }

        foreach (var binary in entry.Binaries)
        {
            if (binary.Key == attachmentId)
            {
                return binary.Value.ReadData();
            }
        }

        return null;
    }

    public async Task<string> UploadAttachment(string username, string entryId, IFormFile file)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            throw new InvalidOperationException("Session not found");
        }

        var uuid = new PwUuid(Guid.Parse(entryId).ToByteArray());
        var entry = db.RootGroup.FindEntry(uuid, true);
        if (entry == null)
        {
            throw new InvalidOperationException("Entry not found");
        }

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            var data = stream.ToArray();
            var protectedBinary = new ProtectedBinary(false, data);
            
            string attachmentId = file.FileName;
            int counter = 1;
            while (entry.Binaries.Any(b => b.Key == attachmentId))
            {
                var extension = Path.GetExtension(file.FileName);
                var nameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                attachmentId = $"{nameWithoutExtension}_{counter}{extension}";
                counter++;
            }

            entry.Binaries.Set(attachmentId, protectedBinary);
            await SaveDatabase(db);

            return attachmentId;
        }
    }

    public async Task<bool> DeleteAttachment(string username, string entryId, string attachmentId)
    {
        var db = await _vaultSessionManager.GetSession(username);
        if (db == null)
        {
            return false;
        }

        var uuid = new PwUuid(Guid.Parse(entryId).ToByteArray());
        var entry = db.RootGroup.FindEntry(uuid, true);
        if (entry == null)
        {
            return false;
        }

        if (entry.Binaries.Remove(attachmentId))
        {
            await SaveDatabase(db);
            return true;
        }

        return false;
    }

    private string GetContentType(string? extension)
    {
        var contentTypeMap = new Dictionary<string, string>
        {
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".bmp", "image/bmp" },
            { ".svg", "image/svg+xml" },
            { ".webp", "image/webp" },
            { ".pdf", "application/pdf" },
            { ".txt", "text/plain" },
            { ".doc", "application/msword" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".zip", "application/zip" },
            { ".rar", "application/vnd.rar" },
            { ".7z", "application/x-7z-compressed" }
        };

        return contentTypeMap.TryGetValue(extension ?? string.Empty, out var contentType) 
            ? contentType 
            : "application/octet-stream";
    }

    private PwGroup? GetGroupById(KeePassLib.PwDatabase db, string groupId)
    {
        if (groupId.Equals("root", StringComparison.OrdinalIgnoreCase))
        {
            return db.RootGroup;
        }

        var uuid = new PwUuid(Guid.Parse(groupId).ToByteArray());
        return db.RootGroup.FindGroup(uuid, true);
    }

    private async Task SaveDatabase(KeePassLib.PwDatabase db)
    {
        db.Save(null);
    }

    private string GetVaultPath(string username)
    {
        var vaultsPath = _configuration["Data:VaultsPath"] ?? "/data/vaults";
        return Path.Combine(vaultsPath, $"{username}.kdbx");
    }

    private ItemDto ConvertGroupToDto(PwGroup group)
    {
        var childItems = new List<ItemDto>();
        foreach (var g in group.Groups)
        {
            childItems.Add(ConvertGroupToDto(g));
        }
        foreach (var e in group.Entries)
        {
            childItems.Add(ConvertEntryToDto(e));
        }

        return new GroupDto
        {
            Id = new Guid(group.Uuid.UuidBytes).ToString(),
            Name = group.Name,
            Type = ItemSubType.Group,
            IsGroup = true,
            LastModified = group.LastModificationTime,
            Icon = group.IconId.ToString(),
            Description = group.Description,
            ChildCount = (int)(group.Groups.UCount + group.Entries.UCount),
            Children = childItems,
            ParentId = group.ParentGroup != null ? new Guid(group.ParentGroup.Uuid.UuidBytes).ToString() : null
        };
    }

    private EntryDto ConvertEntryToDto(PwEntry entry)
    {
        var itemSubType = GetItemSubType(entry);
        var isPxEntry = PxDefs.IsPxEntry(entry);

        var entryDto = new EntryDto
        {
            Id = new Guid(entry.Uuid.UuidBytes).ToString(),
            Name = entry.Strings.ReadSafe(PwDefs.TitleField),
            Type = itemSubType,
            IsGroup = false,
            LastModified = entry.LastModificationTime,
            Icon = entry.IconId.ToString(),
            Description = entry.Description,
            Notes = entry.Strings.ReadSafe(PwDefs.NotesField),
            OtpUrl = GetOtpUrl(entry),
            GroupId = entry.ParentGroup != null ? new Guid(entry.ParentGroup.Uuid.UuidBytes).ToString() : null
        };

        var customFields = new Dictionary<string, string>();
        var fields = new List<FieldDto>();

        foreach (var kvp in entry.Strings)
        {
            if (kvp.Key == PwDefs.TitleField || kvp.Key == PwDefs.NotesField)
            {
                continue;
            }

            var decodedKey = isPxEntry ? PxDefs.DecodeKey(kvp.Key) : kvp.Key;
            var value = kvp.Value.ReadString();
            var isProtected = kvp.Value.IsProtected;

            var fieldDto = new FieldDto
            {
                Key = decodedKey,
                Value = value,
                IsProtected = isProtected,
                IsBinary = false,
                EncodedKey = isPxEntry ? kvp.Key : null
            };
            fields.Add(fieldDto);

            switch (decodedKey)
            {
                case PwDefs.UserNameField:
                    entryDto.Username = value;
                    break;
                case PwDefs.PasswordField:
                    entryDto.Password = value;
                    break;
                case PwDefs.UrlField:
                    entryDto.Url = value;
                    break;
                case PxDefs.EmailField:
                    entryDto.Email = value;
                    break;
                case PxDefs.MobileField:
                    entryDto.Mobile = value;
                    break;
                default:
                    customFields[decodedKey] = value;
                    break;
            }
        }

        entryDto.Fields = fields.Count > 0 ? fields : null;
        entryDto.CustomFields = customFields.Count > 0 ? customFields : null;

        return entryDto;
    }

    private ItemSubType GetItemSubType(PwEntry entry)
    {
        if (PxDefs.IsPxEntry(entry))
        {
            return ItemSubType.PxEntry;
        }
        else if (PxDefs.IsNotes(entry))
        {
            return ItemSubType.Notes;
        }
        return ItemSubType.Entry;
    }

    private string? GetOtpUrl(PwEntry entry)
    {
        var otpUrl = entry.CustomData.Get(PxDefs.PxCustomDataOtpUrl);
        if (!string.IsNullOrEmpty(otpUrl))
        {
            return otpUrl;
        }
        return entry.Strings.ReadSafe("OTP");
    }
}