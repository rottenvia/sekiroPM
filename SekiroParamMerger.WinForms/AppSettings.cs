using System.Text.Json;

namespace SekiroParamMerger.WinForms
{
    /// <summary>
    /// Persists user settings between sessions.
    /// Saved as settings.json next to the executable.
    /// Remembered: game folder path, output path, keep mod files toggle.
    /// Forgotten each session: Mod A path, Mod B path, Mod A name, Mod B name.
    /// </summary>
    public class AppSettings
    {
        private static readonly string SettingsPath = Path.Combine(
            AppContext.BaseDirectory, "settings.json");

        // ── Persisted settings ────────────────────────────────────────────────

        /// <summary>Path to the Sekiro game folder (contains sekiro.exe)</summary>
        public string GameFolderPath { get; set; } = string.Empty;

        /// <summary>
        /// Output folder path — where the merged file's param\gameparam\ structure is created.
        /// Defaults to the Sekiro mods folder if found.
        /// </summary>
        public string OutputFolderPath { get; set; } = string.Empty;

        /// <summary>
        /// If true, Mod A and Mod B param folders are NOT deleted after merging.
        /// Default is false — delete them to prevent conflicts.
        /// </summary>
        public bool KeepModFilesAfterMerge { get; set; } = false;

        // ── Computed paths derived from GameFolderPath ────────────────────────

        /// <summary>Expected path to vanilla gameparam.parambnd.dcx</summary>
        public string VanillaParamPath => Path.Combine(
            GameFolderPath, "param", "gameparam", "gameparam.parambnd.dcx");

        /// <summary>Expected path to the param folder inside game directory</summary>
        public string GameParamFolderPath => Path.Combine(
            GameFolderPath, "param", "gameparam");

        /// <summary>True if the vanilla gameparam file exists on disk</summary>
        public bool VanillaFileExists => File.Exists(VanillaParamPath);

        /// <summary>True if the param folder has been unpacked at all</summary>
        public bool ParamFolderExists => Directory.Exists(GameParamFolderPath);

        // ── Save / Load ───────────────────────────────────────────────────────

        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(SettingsPath, json);
            }
            catch
            {
                // Settings save failure is non-critical — app continues working
            }
        }

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch
            {
                // Corrupted settings — start fresh
            }
            return new AppSettings();
        }

        public bool IsFirstRun => string.IsNullOrWhiteSpace(GameFolderPath);
    }
}
