using System;
using System.IO;
using System.Text.Json;

namespace TwitchChatOverlay;

public class AppSettings
{
    public string TwitchChannel { get; set; } = "";
    public double ZoomLevel { get; set; } = 1.0;
    public byte OpacityLevel { get; set; } = 128;
    public bool AutoHideBorders { get; set; } = false;
    public bool HideFromOBS { get; set; } = true;
    public bool HideTaskbarIcon { get; set; } = true;
    
    public double WindowLeft { get; set; } = 10;
    public double WindowTop { get; set; } = 10;
    public double WindowWidth { get; set; } = 350;
    public double WindowHeight { get; set; } = 600;

    private static string SettingsFilePath => 
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                     "TwitchChatOverlay", "settings.json");

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                string json = File.ReadAllText(SettingsFilePath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"failed to load settings: {ex.Message}");
        }
        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            string directory = Path.GetDirectoryName(SettingsFilePath)!;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(SettingsFilePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"failed to save settings: {ex.Message}");
        }
    }
}
