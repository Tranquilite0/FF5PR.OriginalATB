using System.Diagnostics;

namespace FF5PR.OriginalATB;

public enum GameVersion
{
    Unknown = 0,
    FF1 = 1 << 0,
    FF2 = 1 << 1,
    FF3 = 1 << 2,
    FF4 = 1 << 3,
    FF5 = 1 << 4,
    FF6 = 1 << 5,
    Any = int.MaxValue
}

public class GameDetection
{
    private static GameVersion _version = GameVersion.Unknown;
    public static GameVersion Version
    {
        get
        {
            if (_version == GameVersion.Unknown)
            {
                _version = GetGameVersion();
            }
            return _version;
        }
        private set => _version = value;
    }

    private static GameVersion GetGameVersion() => GetProductName() switch
    {
        "FINAL FANTASY" => GameVersion.FF1,
        "FINAL FANTASY II" => GameVersion.FF2,
        "FINAL FANTASY III" => GameVersion.FF3,
        "FINAL FANTASY IV" => GameVersion.FF4,
        "FINAL FANTASY V" => GameVersion.FF5,
        "FINAL FANTASY VI" => GameVersion.FF6,
        _ => GetModuleFileName() switch
        {
            "FINAL FANTASY" => GameVersion.FF1,
            "FINAL FANTASY II" => GameVersion.FF2,
            "FINAL FANTASY III" => GameVersion.FF3,
            "FINAL FANTASY IV" => GameVersion.FF4,
            "FINAL FANTASY V" => GameVersion.FF5,
            "FINAL FANTASY VI" => GameVersion.FF6,
            _ => GameVersion.Unknown,
        },
    };

    private static string GetProductName()
    {
        var fileInfo = FileVersionInfo.GetVersionInfo(BepInEx.Paths.ExecutablePath);
        var productName = TrimAfterNullCharacter(fileInfo.ProductName);

        Plugin.Log.LogDebug($"Product name: {productName}");

        return productName;
    }

    private static string GetModuleFileName()
    {
        Plugin.Log.LogDebug($"Module name: {BepInEx.Paths.ProcessName}");

        return BepInEx.Paths.ProcessName;
    }

    private static string TrimAfterNullCharacter(string input)
    {
        int nullCharIndex = input.IndexOf('\0');

        if (nullCharIndex == -1)
        {
            return input;
        }

        return input[..nullCharIndex];
    }
}
