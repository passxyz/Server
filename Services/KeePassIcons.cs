using KeePassLib;

namespace PassXYZ.Server.Services;

public static class KeePassIcons
{
    public static (string? Data, string? ContentType) GetIconData(PwIcon iconId)
    {
        var svg = iconId switch
        {
            PwIcon.Key => GetKeySvg(),
            PwIcon.World => GetWorldSvg(),
            PwIcon.Warning => GetWarningSvg(),
            PwIcon.NetworkServer => GetNetworkSvg(),
            PwIcon.MarkedDirectory => GetFolderFavoriteSvg(),
            PwIcon.UserCommunication => GetUserGroupSvg(),
            PwIcon.Parts => GetCpuSvg(),
            PwIcon.Notepad => GetFileTextSvg(),
            PwIcon.WorldSocket => GetLinkSvg(),
            PwIcon.Identity => GetUserSvg(),
            PwIcon.PaperReady => GetFileSvg(),
            PwIcon.Digicam => GetCameraSvg(),
            PwIcon.IRCommunication => GetPhoneSvg(),
            PwIcon.MultiKeys => GetKeySvg(),
            PwIcon.Energy => GetLightbulbSvg(),
            PwIcon.Scanner => GetScannerSvg(),
            PwIcon.WorldStar => GetStarSvg(),
            PwIcon.CDRom => GetDriveSvg(),
            PwIcon.Monitor => GetComputerSvg(),
            PwIcon.EMail => GetMailSvg(),
            PwIcon.Configuration => GetSettingsSvg(),
            PwIcon.ClipboardReady => GetCopySvg(),
            PwIcon.PaperNew => GetFileSvg(),
            PwIcon.Screen => GetMonitorSvg(),
            PwIcon.EnergyCareful => GetBatterySvg(),
            PwIcon.EMailBox => GetMailSvg(),
            PwIcon.Disk => GetDriveSvg(),
            PwIcon.Drive => GetDriveSvg(),
            PwIcon.PaperQ => GetFileTextSvg(),
            PwIcon.TerminalEncrypted => GetTerminalSvg(),
            PwIcon.Console => GetTerminalSvg(),
            PwIcon.Printer => GetPrintSvg(),
            PwIcon.ProgramIcons => GetImageSvg(),
            PwIcon.Run => GetPlaySvg(),
            PwIcon.Settings => GetSettingsSvg(),
            PwIcon.WorldComputer => GetNetworkSvg(),
            PwIcon.Archive => GetArchiveSvg(),
            PwIcon.Homebanking => GetBankSvg(),
            PwIcon.DriveWindows => GetDriveSvg(),
            PwIcon.Clock => GetCalendarSvg(),
            PwIcon.EMailSearch => GetSearchSvg(),
            PwIcon.PaperFlag => GetFlagSvg(),
            PwIcon.Memory => GetMemorySvg(),
            PwIcon.TrashBin => GetTrashSvg(),
            PwIcon.Note => GetFileTextSvg(),
            PwIcon.Expired => GetWarningSvg(),
            PwIcon.Info => GetInfoSvg(),
            PwIcon.Package => GetPackageSvg(),
            PwIcon.Folder => GetFolderSvg(),
            PwIcon.FolderOpen => GetFolderOpenSvg(),
            PwIcon.FolderPackage => GetFolderSvg(),
            PwIcon.LockOpen => GetLockSvg(),
            PwIcon.PaperLocked => GetLockSvg(),
            PwIcon.Checked => GetCheckSvg(),
            PwIcon.Pen => GetPencilSvg(),
            PwIcon.Thumbnail => GetImageSvg(),
            PwIcon.Book => GetBookSvg(),
            PwIcon.List => GetListSvg(),
            PwIcon.UserKey => GetUserKeySvg(),
            PwIcon.Tool => GetWrenchSvg(),
            PwIcon.Home => GetHomeSvg(),
            PwIcon.Star => GetStarSvg(),
            PwIcon.Tux => GetPenguinSvg(),
            PwIcon.Feather => GetFeatherSvg(),
            PwIcon.Apple => GetAppleSvg(),
            PwIcon.Wiki => GetBookSvg(),
            PwIcon.Money => GetMoneySvg(),
            PwIcon.Certificate => GetShieldSvg(),
            PwIcon.BlackBerry => GetSmartphoneSvg(),
            _ => GetDefaultSvg()
        };

        if (svg != null)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(svg);
            return (Convert.ToBase64String(bytes), "image/svg+xml");
        }

        return (null, null);
    }

    public static (string? Data, string? ContentType) GetIconDataFromFontGlyph(string glyph)
    {
        var svg = glyph switch
        {
            "\uf07b" => GetFolderSvg(),
            "\uf07c" => GetFolderOpenSvg(),
            "\uf15b" => GetFileSvg(),
            "\uf15c" => GetFileTextSvg(),
            "\uf1c1" => GetFileTextSvg(),
            "\uf1c2" => GetFileTextSvg(),
            "\uf1c3" => GetFileTextSvg(),
            "\uf1c4" => GetFileTextSvg(),
            "\uf1c5" => GetImageSvg(),
            "\uf1c6" => GetArchiveSvg(),
            "\uf1c7" => GetAudioSvg(),
            "\uf1c8" => GetVideoSvg(),
            "\uf1c9" => GetCodeSvg(),
            "\uf005" => GetStarSvg(),
            "\uf004" => GetHeartSvg(),
            "\uf007" => GetUserSvg(),
            "\uf0e0" => GetMailSvg(),
            "\uf06e" => GetEyeSvg(),
            "\uf023" => GetLockSvg(),
            "\uf017" => GetClockSvg(),
            "\uf022" => GetListSvg(),
            "\uf024" => GetFlagSvg(),
            "\uf03e" => GetImageSvg(),
            "\uf044" => GetPencilSvg(),
            "\uf058" => GetCheckSvg(),
            "\uf057" => GetTimesCircleSvg(),
            "\uf0eb" => GetLightbulbSvg(),
            "\uf0f3" => GetBellSvg(),
            "\uf073" => GetCalendarSvg(),
            "\uf075" => GetCommentSvg(),
            "\uf080" => GetChartBarSvg(),
            "\uf09d" => GetCreditCardSvg(),
            "\uf0a0" => GetDriveSvg(),
            "\uf0c5" => GetCopySvg(),
            "\uf133" => GetCalendarSvg(),
            "\uf14a" => GetCheckSvg(),
            "\uf150" => GetCaretDownSvg(),
            "\uf151" => GetCaretUpSvg(),
            "\uf273" => GetCalendarTimesSvg(),
            "\uf274" => GetCalendarCheckSvg(),
            "\uf279" => GetMapSvg(),
            "\uf2ed" => GetTrashSvg(),
            "\uf328" => GetClipboardSvg(),
            "\uf4ad" => GetCommentDotsSvg(),
            "\uf556" => GetAngrySvg(),
            "\uf579" => GetFlushedSvg(),
            "\uf580" => GetGrinSvg(),
            "\uf5b8" => GetSmileBeamSvg(),
            "\uf5c2" => GetSurpriseSvg(),
            "\uf5c8" => GetTiredSvg(),
            _ => GetDefaultSvg()
        };

        if (svg != null)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(svg);
            return (Convert.ToBase64String(bytes), "image/svg+xml");
        }

        return (null, null);
    }

    public static string GetIconName(PwIcon iconId)
    {
        return iconId.ToString();
    }

    private static string GetFolderSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z\"/></svg>";
    private static string GetFolderOpenSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h7l5 5v11z\"/></svg>";
    private static string GetFileSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z\"/><polyline points=\"14 2 14 8 20 8\"/></svg>";
    private static string GetFileTextSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z\"/><polyline points=\"14 2 14 8 20 8\"/><line x1=\"16\" y1=\"13\" x2=\"8\" y2=\"13\"/><line x1=\"16\" y1=\"17\" x2=\"8\" y2=\"17\"/><polyline points=\"10 9 9 9 8 9\"/></svg>";
    private static string GetLockSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><rect x=\"3\" y=\"11\" width=\"18\" height=\"11\" rx=\"2\" ry=\"2\"/><path d=\"M7 11V7a5 5 0 0 1 10 0v4\"/></svg>";
    private static string GetKeySvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\"><rect x=\"3\" y=\"11\" width=\"18\" height=\"11\" rx=\"2\" ry=\"2\"/><circle cx=\"7\" cy=\"11\" r=\"1\"/><path d=\"M7 11V7a5 5 0 0 1 10 0v4\"/></svg>";
    private static string GetUserSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2\"/><circle cx=\"12\" cy=\"7\" r=\"4\"/></svg>";
    private static string GetUserGroupSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2\"/><circle cx=\"9\" cy=\"7\" r=\"4\"/><path d=\"M23 21v-2a4 4 0 0 0-3-3.87\"/><path d=\"M16 3.13a4 4 0 0 1 0 7.75\" /></svg>";
    private static string GetWorldSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M2 12h20\"/><path d=\"M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z\"/></svg>";
    private static string GetMailSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z\"/><polyline points=\"22,6 12,13 2,6\"/></svg>";
    private static string GetCameraSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z\"/><circle cx=\"12\" cy=\"13\" r=\"4\"/></svg>";
    private static string GetPhoneSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z\"/></svg>";
    private static string GetCreditCardSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><rect x=\"1\" y=\"4\" width=\"22\" height=\"16\" rx=\"2\" ry=\"2\"/><line x1=\"1\" y1=\"10\" x2=\"23\" y2=\"10\"/></svg>";
    private static string GetBankSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M12 2L2 7l10 5 10-5-10-5z\"/><path d=\"M2 17l10 5 10-5\" /><path d=\"M2 12l10 5 10-5\"/></svg>";
    private static string GetBriefcaseSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M20 6h-3V4a2 2 0 0 0-2-2H9a2 2 0 0 0-2 2v2H4a2 2 0 0 0-2 2v11a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2V8a2 2 0 0 0-2-2z\"/></svg>";
    private static string GetCalculatorSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><rect x=\"4\" y=\"2\" width=\"16\" height=\"20\" rx=\"2\" ry=\"2\"/><line x1=\"8\" y1=\"6\" x2=\"16\" y2=\"6\"/><line x1=\"4\" y1=\"10\" x2=\"20\" y2=\"10\"/><rect x=\"8\" y=\"14\" width=\"2\" height=\"2\"/><rect x=\"12\" y=\"14\" width=\"2\" height=\"2\"/><rect x=\"16\" y=\"14\" width=\"2\" height=\"2\"/><rect x=\"8\" y=\"18\" width=\"2\" height=\"2\"/><rect x=\"12\" y=\"18\" width=\"2\" height=\"2\"/><rect x=\"16\" y=\"18\" width=\"2\" height=\"2\"/></svg>";
    private static string GetCalendarSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M19 4h-1V2h-2v2H8V2H6v2H5c-1.11 0-1.99.9-1.99 2L3 20c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 16H5V9h14v11z\"/></svg>";
    private static string GetCloudSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M19 18H6a4 4 0 0 1-.17-8 5.5 5.5 0 0 1 10.63-2.38A3.5 3.5 0 0 1 19 11.5v6.5z\"/></svg>";
    private static string GetCloudDownloadSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M19 18H6a4 4 0 0 1-.17-8 5.5 5.5 0 0 1 10.63-2.38A3.5 3.5 0 0 1 19 11.5v6.5z\"/><polyline points=\"12 11 8 15 12 19\"/><line x1=\"12\" y1=\"19\" x2=\"12\" y2=\"11\"/></svg>";
    private static string GetCloudUploadSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M19 18H6a4 4 0 0 1-.17-8 5.5 5.5 0 0 1 10.63-2.38A3.5 3.5 0 0 1 19 11.5v6.5z\"/><polyline points=\"12 19 8 15 12 11\"/><line x1=\"12\" y1=\"11\" x2=\"12\" y2=\"19\"/></svg>";
    private static string GetComputerSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><rect x=\"2\" y=\"3\" width=\"20\" height=\"14\" rx=\"2\" ry=\"2\"/><line x1=\"8\" y1=\"21\" x2=\"16\" y2=\"21\"/><line x1=\"12\" y1=\"17\" x2=\"12\" y2=\"21\"/></svg>";
    private static string GetCpuSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><rect x=\"4\" y=\"4\" width=\"16\" height=\"16\" rx=\"2\" ry=\"2\"/><rect x=\"9\" y=\"9\" width=\"6\" height=\"6\"/><line x1=\"4\" y1=\"9\" x2=\"9\" y2=\"9\"/><line x1=\"15\" y1=\"9\" x2=\"20\" y2=\"9\"/><line x1=\"4\" y1=\"15\" x2=\"9\" y2=\"15\"/><line x1=\"15\" y1=\"15\" x2=\"20\" y2=\"15\"/><line x1=\"9\" y1=\"4\" x2=\"9\" y2=\"9\"/><line x1=\"9\" y1=\"15\" x2=\"9\" y2=\"20\"/><line x1=\"15\" y1=\"4\" x2=\"15\" y2=\"9\"/><line x1=\"15\" y1=\"15\" x2=\"15\" y2=\"20\"/></svg>";
    private static string GetDriveSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z\"/><path d=\"M6 10h12\"/><path d=\"M6 14h12\"/><path d=\"M6 18h6\"/></svg>";
    private static string GetEditSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M12 20h9\"/><path d=\"M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z\"/></svg>";
    private static string GetEyeSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z\"/><circle cx=\"12\" cy=\"12\" r=\"3\"/></svg>";
    private static string GetFlagSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M4 15s1.5-2 4-2 4 2 4 2\"/><path d=\"M4 12s2-2 5-2 5 2 5 2\"/><path d=\"M20.49 9.49a9 9 0 0 0-12.73 0l-1.41 1.41a4 4 0 0 0 5.66 5.66l1.41-1.41a9 9 0 0 0 0-12.73l-1.41-1.41a4 4 0 0 0-5.66 5.66l1.41 1.41a9 9 0 0 0 12.73 0l1.41-1.41a4 4 0 0 0 0-5.66l-1.41-1.41a9 9 0 0 0 0 12.73l1.41 1.41a4 4 0 0 0 5.66-5.66l-1.41-1.41z\"/></svg>";
    private static string GetFolderFavoriteSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z\"/><path d=\"M12 16l-4-4h8z\"/></svg>";
    private static string GetGearSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M19.43 12.98c.04-.32.07-.64.07-.98s-.03-.66-.07-.98l2.11-1.65a1 1 0 0 0 .25-1.08l-2-3.46a1 1 0 0 0-.93-.52h-4.16a1 1 0 0 0-.91.59l-1.64 2.73a1 1 0 0 0 .21 1.27l2.12 1.64a1 1 0 0 0 1.22.22l2.09-1.05a1 1 0 0 0 .78-.9V8.9a1 1 0 0 0-.79-.99l-2.3-1a1 1 0 0 0-1.02.21l-1.49 2.53a1 1 0 0 0 .22 1.28l1.96 1.56a1 1 0 0 0 1.27.22l2.09-1.05a1 1 0 0 0 .78-.9v-.57a1 1 0 0 0-.79-.99l-2.3-1a1 1 0 0 0-1.02.21L9.19 9.46a1 1 0 0 0 .22 1.28l1.96 1.56c.36.28.86.28 1.22 0l1.96-1.56a1 1 0 0 0 .22-1.28l-2.3-1a1 1 0 0 0-1.02-.21h-.18a1 1 0 0 0-.91.59l-1.64 2.73a1 1 0 0 0 .21 1.27l2.12 1.64c.36.28.86.28 1.22 0l2.12-1.64a1 1 0 0 0 .21-1.27l-1.64-2.73a1 1 0 0 0-.91-.59h-.18a1 1 0 0 0-.98.79l-1.05 2.09a1 1 0 0 0 .22 1.22l1.64 2.12a1 1 0 0 0 1.27.21l2.73-1.64a1 1 0 0 0 .59-.91v-.18a1 1 0 0 0-.79-.98l-2.09-1.05a1 1 0 0 0-1.22.22l-1.64 2.12a1 1 0 0 0-.21 1.27l1.64 2.73a1 1 0 0 0 .91.59h4.16a1 1 0 0 0 .93-.52l2-3.46a1 1 0 0 0-.25-1.08l-2.11-1.65z\"/></svg>";
    private static string GetHeartSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z\"/></svg>";
    private static string GetHomeSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V9z\"/><polyline points=\"9 22 9 12 15 12 15 22\"/></svg>";
    private static string GetImageSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\" ry=\"2\"/><circle cx=\"8.5\" cy=\"8.5\" r=\"1.5\"/><polyline points=\"21 15 16 10 5 21\"/></svg>";
    private static string GetLaptopSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><rect x=\"2\" y=\"3\" width=\"20\" height=\"14\" rx=\"2\" ry=\"2\"/><line x1=\"8\" y1=\"21\" x2=\"16\" y2=\"21\"/></svg>";
    private static string GetLightbulbSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M9 21c0 .55.45 1 1 1h4c.55 0 1-.45 1-1v-1H9v1zm3-19C8.14 2 5 5.14 5 9c0 2.38 1.19 4.47 3 5.74V17c0 .55.45 1 1 1h6c.55 0 1-.45 1-1v-2.26c1.81-1.27 3-3.36 3-5.74 0-3.86-3.14-7-7-7zm2.85 11.1l-.85.6V16h-4v-2.3l-.85-.6C7.8 12.16 7 10.63 7 9c0-2.76 2.24-5 5-5s5 2.24 5 5c0 1.63-.8 3.16-2.15 4.1z\"/></svg>";
    private static string GetLinkSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M10 13a5 5 0 0 1 5-5h4a5 5 0 0 1 5 5v4a5 5 0 0 1-5 5h-4a5 5 0 0 1-5-5v-4z\"/><path d=\"M4 13a5 5 0 0 1 5-5h4a5 5 0 0 1 5 5v4a5 5 0 0 1-5 5H9a5 5 0 0 1-5-5v-4z\"/></svg>";
    private static string GetMapSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><polygon points=\"1 6 1 22 8 18 16 22 23 18 23 2 16 6 8 2 1 6\"/><line x1=\"8\" y1=\"2\" x2=\"8\" y2=\"18\"/><line x1=\"16\" y1=\"6\" x2=\"16\" y2=\"22\"/></svg>";
    private static string GetMedalSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z\"/></svg>";
    private static string GetMoneySvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><line x1=\"12\" y1=\"1\" x2=\"12\" y2=\"23\"/><path d=\"M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6\"/></svg>";
    private static string GetNetworkSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z\"/><path d=\"M2 12h20\"/></svg>";
    private static string GetPaperclipSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M16 12h-2.34L12 9.34 10.34 12H8l3-3-3-3h2.34L12 6.66 13.66 4H16l-3 3 3 3z\"/></svg>";
    private static string GetPencilSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M12 20h9\"/><path d=\"M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z\"/></svg>";
    private static string GetPictureSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\" ry=\"2\"/><circle cx=\"8.5\" cy=\"8.5\" r=\"1.5\"/><polyline points=\"21 15 16 10 5 21\"/></svg>";
    private static string GetPrintSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M22 12h-4l-3 9L9 3l-3 9H2\"/><polyline points=\"16 15 8 15 8 21\"/><line x1=\"16\" y1=\"3\" x2=\"16\" y2=\"15\"/></svg>";
    private static string GetSearchSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><circle cx=\"11\" cy=\"11\" r=\"8\"/><line x1=\"21\" y1=\"21\" x2=\"16.65\" y2=\"16.65\"/></svg>";
    private static string GetServerSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><rect x=\"2\" y=\"2\" width=\"20\" height=\"8\" rx=\"2\" ry=\"2\"/><rect x=\"2\" y=\"14\" width=\"20\" height=\"8\" rx=\"2\" ry=\"2\"/><line x1=\"6\" y1=\"10\" x2=\"6\" y2=\"14\"/><line x1=\"10\" y1=\"10\" x2=\"10\" y2=\"14\"/><line x1=\"14\" y1=\"10\" x2=\"14\" y2=\"14\"/><line x1=\"18\" y1=\"10\" x2=\"18\" y2=\"14\"/></svg>";
    private static string GetSettingsSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M12 15.5c.83 0 1.5-.67 1.5-1.5s-.67-1.5-1.5-1.5-1.5.67-1.5 1.5.67 1.5 1.5 1.5zm0-8c.83 0 1.5-.67 1.5-1.5S12.83 4.5 12 4.5 10.5 5.17 10.5 6s.67 1.5 1.5 1.5zm0 17c1.66 0 3-1.34 3-3h-6c0 1.66 1.34 3 3 3zm0-20C8.14 4 5 7.14 5 11c0 2.38 1.19 4.47 3 5.74V19c0 .55.45 1 1 1h6c.55 0 1-.45 1-1v-2.26c1.81-1.27 3-3.36 3-5.74 0-3.86-3.14-7-7-7z\"/></svg>";
    private static string GetShieldSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M12 1L3 5v6c0 5.55 3.84 10.74 9 12 5.16-1.26 9-6.45 9-12V5l-9-4z\"/></svg>";
    private static string GetSmartphoneSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><rect x=\"6\" y=\"2\" width=\"12\" height=\"20\" rx=\"2\" ry=\"2\"/><path d=\"M12 18h.01\"/></svg>";
    private static string GetSoftwareSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M12 2L2 7l10 5 10-5-10-5z\"/><path d=\"M2 17l10 5 10-5\" /><path d=\"M2 12l10 5 10-5\"/></svg>";
    private static string GetStarSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z\"/></svg>";
    private static string GetTagSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M21 12l-8-8v6a4 4 0 0 0-1 1.72V21l5.29-5.29a2 2 0 0 1 1.42-.58H21z\"/></svg>";
    private static string GetTerminalSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M4 4h16v16H4z\"/><path d=\"M4 10h16M4 14h16M8 6h2M8 18h2\"/></svg>";
    private static string GetTrashSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M3 6h18\" /><path d=\"M19 6v14c0 1.1-.9 2-2 2H7c-1.1 0-2-.9-2-2V6\" /><path d=\"M8 6V4c0-1.1.9-2 2-2h4c1.1 0 2 .9 2 2v2\" /><line x1=\"10\" y1=\"11\" x2=\"10\" y2=\"17\" /><line x1=\"14\" y1=\"11\" x2=\"14\" y2=\"17\" /></svg>";
    private static string GetUploadSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4\"/><polyline points=\"17 8 12 3 7 8\"/><line x1=\"12\" y1=\"3\" x2=\"12\" y2=\"15\"/></svg>";
    private static string GetUserKeySvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2\"/><circle cx=\"12\" cy=\"7\" r=\"4\"/><path d=\"M22 2h-2\"/><path d=\"M14 2h-4\"/><path d=\"M6 2H4\"/></svg>";
    private static string GetWarningSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z\"/><line x1=\"12\" y1=\"9\" x2=\"12\" y2=\"13\"/><line x1=\"12\" y1=\"17\" x2=\"12.01\" y2=\"17\"/></svg>";
    private static string GetWebsiteSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M4 9h16v11H4z\"/><path d=\"M4 4h16v5H4z\"/><path d=\"M8 20h8\"/></svg>";
    private static string GetWifiSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z\"/><path d=\"M12 12c-2.21 0-4-1.79-4-4s1.79-4 4-4 4 1.79 4 4-1.79 4-4 4zm0-6c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2z\"/></svg>";
    private static string GetDefaultSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z\"/><polyline points=\"14 2 14 8 20 8\"/></svg>";

    private static string GetScannerSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M4 4h16v16H4z\"/><rect x=\"6\" y=\"6\" width=\"12\" height=\"12\" rx=\"2\"/><path d=\"M8 12h8\"/><path d=\"M12 8v8\"/></svg>";
    private static string GetCopySvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><rect x=\"9\" y=\"9\" width=\"13\" height=\"13\" rx=\"2\" ry=\"2\"/><path d=\"M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1\"/><path d=\"M17 5H7a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2z\"/></svg>";
    private static string GetMonitorSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><rect x=\"2\" y=\"3\" width=\"20\" height=\"14\" rx=\"2\" ry=\"2\"/><line x1=\"8\" y1=\"21\" x2=\"16\" y2=\"21\"/><line x1=\"12\" y1=\"17\" x2=\"12\" y2=\"21\"/></svg>";
    private static string GetBatterySvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><rect x=\"1\" y=\"6\" width=\"20\" height=\"12\" rx=\"2\" ry=\"2\"/><path d=\"M23 13h-2\"/><path d=\"M15 6v12\"/><path d=\"M9 6v12\"/></svg>";
    private static string GetPlaySvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><polygon points=\"5 3 19 12 5 21 5 3\"/></svg>";
    private static string GetArchiveSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4\"/><polyline points=\"7 10 12 15 17 10\"/><line x1=\"12\" y1=\"15\" x2=\"12\" y2=\"3\"/></svg>";
    private static string GetMemorySvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><rect x=\"5\" y=\"2\" width=\"14\" height=\"20\" rx=\"2\"/><line x1=\"9\" y1=\"6\" x2=\"15\" y2=\"6\"/><line x1=\"9\" y1=\"10\" x2=\"15\" y2=\"10\"/><line x1=\"9\" y1=\"14\" x2=\"15\" y2=\"14\"/><line x1=\"9\" y1=\"18\" x2=\"15\" y2=\"18\"/></svg>";
    private static string GetInfoSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><circle cx=\"12\" cy=\"12\" r=\"10\"/><line x1=\"12\" y1=\"16\" x2=\"12\" y2=\"12\"/><line x1=\"12\" y1=\"8\" x2=\"12.01\" y2=\"8\"/></svg>";
    private static string GetPackageSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M12 2L2 7l10 5 10-5-10-5z\"/><path d=\"M2 17l10 5 10-5\" /><path d=\"M2 12l10 5 10-5\"/></svg>";
    private static string GetCheckSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><polyline points=\"20 6 9 17 4 12\"/></svg>";
    private static string GetBookSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z\"/><path d=\"M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z\"/></svg>";
    private static string GetListSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><line x1=\"8\" y1=\"6\" x2=\"21\" y2=\"6\"/><line x1=\"8\" y1=\"12\" x2=\"21\" y2=\"12\"/><line x1=\"8\" y1=\"18\" x2=\"21\" y2=\"18\"/><line x1=\"3\" y1=\"6\" x2=\"3.01\" y2=\"6\"/><line x1=\"3\" y1=\"12\" x2=\"3.01\" y2=\"12\"/><line x1=\"3\" y1=\"18\" x2=\"3.01\" y2=\"18\"/></svg>";
    private static string GetWrenchSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M16.24 8.66a3 3 0 0 0-4.12 0L7.3 13.7a2.9 2.9 0 0 0-.88 2.1v3a1 1 0 0 0 1 1h3a1 1 0 0 0 1-.88l1.33-6.66a3 3 0 0 0-.88-2.1l-1.95-1.95z\"/><path d=\"M16 19a2 2 0 1 0 4 0 2 2 0 0 0-4 0z\"/></svg>";
    private static string GetPenguinSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M8 15h8\"/><path d=\"M12 7v8\"/></svg>";
    private static string GetFeatherSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M20.24 12.24a6 6 0 0 0-8.49-8.49L5 10.5v4l6.24-6.24a6 6 0 0 1 8.49 8.49L19 14v4h1\"/><path d=\"M17 14v2\"/><path d=\"M12 14v2\"/><path d=\"M7 14v2\"/></svg>";
    private static string GetAppleSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z\"/></svg>";
    private static string GetAudioSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M9 18V5l12-2v13\"/><circle cx=\"6\" cy=\"18\" r=\"3\"/><circle cx=\"18\" cy=\"16\" r=\"3\"/></svg>";
    private static string GetVideoSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M21 3H3c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h18c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z\"/><path d=\"M8 15l6-3-6-3v6z\"/></svg>";
    private static string GetCodeSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><polyline points=\"16 18 22 12 16 6\"/><polyline points=\"8 6 2 12 8 18\"/></svg>";
    private static string GetTimesCircleSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><circle cx=\"12\" cy=\"12\" r=\"10\"/><line x1=\"15\" y1=\"9\" x2=\"9\" y2=\"15\"/><line x1=\"9\" y1=\"9\" x2=\"15\" y2=\"15\"/></svg>";
    private static string GetClockSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><circle cx=\"12\" cy=\"12\" r=\"10\"/><polyline points=\"12 6 12 12 16 14\"/></svg>";
    private static string GetCommentSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z\"/></svg>";
    private static string GetChartBarSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\" ry=\"2\"/><line x1=\"9\" y1=\"9\" x2=\"9\" y2=\"15\"/><line x1=\"15\" y1=\"9\" x2=\"15\" y2=\"15\"/><line x1=\"12\" y1=\"9\" x2=\"12\" y2=\"15\"/></svg>";
    private static string GetCaretDownSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M6 9l6 6 6-6\"/></svg>";
    private static string GetCaretUpSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M18 15l-6-6-6 6\"/></svg>";
    private static string GetCalendarTimesSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M19 3h-1V1h-2v2H8V1H6v2H5c-1.11 0-1.99.9-1.99 2L3 19c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H5V8h14v11zM9 10h2v2H9v-2zm4 0h2v2h-2v-2zm4 0h2v2h-2v-2z\"/></svg>";
    private static string GetCalendarCheckSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M19 3h-1V1h-2v2H8V1H6v2H5c-1.11 0-1.99.9-1.99 2L3 19c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H5V8h14v11zM9 10h2v2H9v-2zm4 0h2v2h-2v-2zm4 0h2v2h-2v-2z\"/></svg>";
    private static string GetClipboardSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M16 1H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V3a2 2 0 0 0-2-2zm-4 14H6v-2h6v2zm0-4H6v-2h6v2zm0-4H6V7h6v2z\"/></svg>";
    private static string GetCommentDotsSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z\"/><circle cx=\"14\" cy=\"13\" r=\"2\"/><circle cx=\"8\" cy=\"13\" r=\"2\"/><circle cx=\"11\" cy=\"17\" r=\"2\"/></svg>";
    private static string GetAngrySvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M8 9h8M9 14l1-1 1 1m4 0l1-1 1 1\"/><path d=\"M9.5 8a2.5 2.5 0 0 1 5 0\"/></svg>";
    private static string GetFlushedSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><circle cx=\"12\" cy=\"12\" r=\"10\"/><circle cx=\"8\" cy=\"10\" r=\"2\"/><circle cx=\"16\" cy=\"10\" r=\"2\"/><path d=\"M8 14l1.5-2.5L11 14l1.5-2.5L14 14\"/><path d=\"M7 16h10\"/></svg>";
    private static string GetGrinSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><circle cx=\"12\" cy=\"12\" r=\"10\"/><circle cx=\"8\" cy=\"10\" r=\"2\"/><circle cx=\"16\" cy=\"10\" r=\"2\"/><path d=\"M8 14l2-2 2 2 2-2 2 2\"/><path d=\"M9 17h6\"/></svg>";
    private static string GetSmileBeamSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><circle cx=\"12\" cy=\"12\" r=\"10\"/><circle cx=\"8\" cy=\"10\" r=\"2\"/><circle cx=\"16\" cy=\"10\" r=\"2\"/><path d=\"M8 14l1.5-2.5L11 14l1.5-2.5L14 14l1.5-2.5L17 14\"/><path d=\"M8 17h8\"/></svg>";
    private static string GetSurpriseSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><circle cx=\"12\" cy=\"12\" r=\"10\"/><circle cx=\"8\" cy=\"10\" r=\"2\"/><circle cx=\"16\" cy=\"10\" r=\"2\"/><path d=\"M8 14h8\"/><path d=\"M12 17v3\"/></svg>";
    private static string GetTiredSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M8 8l2 2M14 8l2 2M8 16l2-2M14 16l2-2\"/><path d=\"M9 12h6\"/><path d=\"M10 15h4\"/></svg>";
    private static string GetBellSvg() => "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" fill=\"currentColor\"><path d=\"M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9\"/><path d=\"M13.73 21a2 2 0 0 1-3.46 0\"/></svg>";
}
