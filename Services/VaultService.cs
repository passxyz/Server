using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Serialization;
using PassXYZ.Server.DTOs.Vault;
using PassXYZLib;

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
            ChildCount = (int)(group.Groups.UCount + group.Entries.UCount),
            Children = childItems
        };
    }

    private EntryDto ConvertEntryToDto(PwEntry entry)
    {
        var customFields = new Dictionary<string, string>();
        foreach (var kvp in entry.Strings)
        {
            if (kvp.Key != PwDefs.TitleField && 
                kvp.Key != PwDefs.UserNameField && 
                kvp.Key != PwDefs.PasswordField && 
                kvp.Key != PwDefs.UrlField && 
                kvp.Key != PwDefs.NotesField)
            {
                customFields[kvp.Key] = kvp.Value.ReadString();
            }
        }

        return new EntryDto
        {
            Id = new Guid(entry.Uuid.UuidBytes).ToString(),
            Name = entry.Strings.ReadSafe(PwDefs.TitleField),
            Type = ItemSubType.Entry,
            IsGroup = false,
            LastModified = entry.LastModificationTime,
            Icon = entry.IconId.ToString(),
            Username = entry.Strings.ReadSafe(PwDefs.UserNameField),
            Password = entry.Strings.ReadSafe(PwDefs.PasswordField),
            Url = entry.Strings.ReadSafe(PwDefs.UrlField),
            Email = entry.Strings.ReadSafe("Email"),
            Mobile = entry.Strings.ReadSafe("Mobile"),
            Notes = entry.Strings.ReadSafe(PwDefs.NotesField),
            CustomFields = customFields.Count > 0 ? customFields : null
        };
    }
}