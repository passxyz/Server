using KeePassLib;
using PassXYZLib;
using System.Diagnostics;

namespace PassXYZ.Server.Services;

public static class KeePassItemExtensions
{
    public static (string? Data, string? ContentType) SetIcon(this PwEntry entry, PwDatabase db)
    {
        try
        {
            Debug.WriteLine($"SetIcon (Entry): PxDataFile.IconFilePath = {PxDataFile.IconFilePath}");

            if (entry.CustomData.Exists(PxDefs.PxCustomDataIconName))
            {
                var iconName = entry.CustomData.Get(PxDefs.PxCustomDataIconName);
                var builtInIcon = GetBuiltInIconData(iconName);
                if (builtInIcon.Data != null)
                {
                    return builtInIcon;
                }
            }

            if (entry.CustomIconUuid != PwUuid.Zero)
            {
                var customIcon = GetCustomIconFromDb(db, entry.CustomIconUuid);
                if (customIcon != null)
                {
                    var pngData = GetCustomIconPngData(customIcon);
                    if (pngData != null)
                    {
                        return (Convert.ToBase64String(pngData), "image/png");
                    }
                }
            }

            var fontIcon = entry.GetFontIcon();
            if (fontIcon != null && !string.IsNullOrEmpty(fontIcon.Glyph))
            {
                var result = KeePassIcons.GetIconDataFromFontGlyph(fontIcon.Glyph);
                if (result.Data != null)
                {
                    return result;
                }
            }

            return KeePassIcons.GetIconData(entry.IconId);
        }
        catch (Exception)
        {
            return KeePassIcons.GetIconData(entry.IconId);
        }
    }

    public static (string? Data, string? ContentType) SetIcon(this PwGroup group, PwDatabase db)
    {
        try
        {
            Debug.WriteLine($"SetIcon (Group): PxDataFile.IconFilePath = {PxDataFile.IconFilePath}");

            if (group.CustomData.Exists(PxDefs.PxCustomDataIconName))
            {
                var iconName = group.CustomData.Get(PxDefs.PxCustomDataIconName);
                var builtInIcon = GetBuiltInIconData(iconName);
                if (builtInIcon.Data != null)
                {
                    return builtInIcon;
                }
            }

            if (group.CustomIconUuid != PwUuid.Zero)
            {
                var customIcon = GetCustomIconFromDb(db, group.CustomIconUuid);
                if (customIcon != null)
                {
                    var pngData = GetCustomIconPngData(customIcon);
                    if (pngData != null)
                    {
                        return (Convert.ToBase64String(pngData), "image/png");
                    }
                }
            }

            var fontIcon = group.GetFontIcon();
            if (fontIcon != null && !string.IsNullOrEmpty(fontIcon.Glyph))
            {
                var result = KeePassIcons.GetIconDataFromFontGlyph(fontIcon.Glyph);
                if (result.Data != null)
                {
                    return result;
                }
            }

            return KeePassIcons.GetIconData(group.IconId);
        }
        catch (Exception)
        {
            return KeePassIcons.GetIconData(group.IconId);
        }
    }

    private static (string? Data, string? ContentType) GetBuiltInIconData(string iconName)
    {
        if (string.IsNullOrEmpty(iconName))
        {
            return (null, null);
        }

        try
        {
            var iconPath = System.IO.Path.Combine(PxDataFile.IconFilePath, iconName);
            if (System.IO.File.Exists(iconPath))
            {
                var extension = System.IO.Path.GetExtension(iconPath).ToLowerInvariant();
                var fileData = System.IO.File.ReadAllBytes(iconPath);

                string contentType = extension switch
                {
                    ".png" => "image/png",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".svg" => "image/svg+xml",
                    ".gif" => "image/gif",
                    _ => "image/png"
                };

                return (Convert.ToBase64String(fileData), contentType);
            }
        }
        catch (Exception)
        {
        }

        return (null, null);
    }

    private static PwCustomIcon? GetCustomIconFromDb(PwDatabase db, PwUuid iconUuid)
    {
        foreach (var ci in db.CustomIcons)
        {
            if (ci.Uuid.Equals(iconUuid))
            {
                return ci;
            }
        }
        return null;
    }

    private static byte[]? GetCustomIconPngData(PwCustomIcon customIcon)
    {
        if (customIcon.ImageDataPng != null)
        {
            return customIcon.ImageDataPng;
        }

        try
        {
            var imageProperty = customIcon.GetType().GetProperty("Image");
            if (imageProperty != null)
            {
                var image = imageProperty.GetValue(customIcon);
                if (image is SkiaSharp.SKBitmap bitmap)
                {
                    using var ms = new System.IO.MemoryStream();
                    bitmap.Encode(ms, SkiaSharp.SKEncodedImageFormat.Png, 100);
                    return ms.ToArray();
                }
            }
        }
        catch (Exception)
        {
        }

        return null;
    }
}