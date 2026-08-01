namespace SekiroParamMerger.Core
{
    public static class OodleValidator
    {
        private const string DllName = "oo2core_6_win64.dll";

        private static readonly string[] KnownSekiroSteamPaths =
        {
            @"C:\Program Files (x86)\Steam\steamapps\common\Sekiro",
            @"C:\Program Files\Steam\steamapps\common\Sekiro",
            @"C:\Steam\steamapps\common\Sekiro",
            @"D:\Steam\steamapps\common\Sekiro",
            @"D:\SteamLibrary\steamapps\common\Sekiro",
            @"D:\Games\Steam\steamapps\common\Sekiro",
            @"E:\Steam\steamapps\common\Sekiro",
            @"E:\SteamLibrary\steamapps\common\Sekiro",
            @"E:\Games\Steam\steamapps\common\Sekiro",
            @"F:\Steam\steamapps\common\Sekiro",
            @"F:\SteamLibrary\steamapps\common\Sekiro",
        };

        public static OodleValidationResult Validate(string toolDirectory)
        {
            string targetPath = Path.Combine(toolDirectory, DllName);

            if (File.Exists(targetPath))
                return OodleValidationResult.Success(targetPath, wasAutoCopied: false);

            foreach (string sekiroPath in KnownSekiroSteamPaths)
            {
                string sourcePath = Path.Combine(sekiroPath, DllName);

                if (!File.Exists(sourcePath))
                    continue;

                try
                {
                    File.Copy(sourcePath, targetPath, overwrite: false);
                    return OodleValidationResult.Success(targetPath, wasAutoCopied: true, copiedFrom: sourcePath);
                }
                catch (UnauthorizedAccessException)
                {
                    return OodleValidationResult.Failure(
                        $"Found '{DllName}' at:\n  {sourcePath}\n\n" +
                        $"But could not copy it to the tool folder (permission denied).\n\n" +
                        $"Please copy it manually:\n" +
                        $"  FROM: {sourcePath}\n" +
                        $"  TO:   {targetPath}\n\n" +
                        $"Tip: Move the tool out of Program Files to avoid permission issues.");
                }
                catch (Exception ex)
                {
                    return OodleValidationResult.Failure(
                        $"Found '{DllName}' at:\n  {sourcePath}\n\n" +
                        $"But could not copy it: {ex.Message}\n\n" +
                        $"Please copy it manually into:\n  {toolDirectory}");
                }
            }

            return OodleValidationResult.Failure(
                $"'{DllName}' was not found in any known Sekiro install location.\n\n" +
                $"To fix this:\n" +
                $"  1. Open your Sekiro install folder\n" +
                $"     (Default: C:\\Program Files (x86)\\Steam\\steamapps\\common\\Sekiro)\n" +
                $"  2. Find '{DllName}' in that folder\n" +
                $"  3. Copy it into THIS folder:\n" +
                $"     {toolDirectory}\n\n" +
                $"If Sekiro is installed on a different drive,\n" +
                $"find sekiro.exe — '{DllName}' will be right next to it.\n\n" +
                $"The tool CANNOT read Sekiro files without this DLL.");
        }
    }

    public class OodleValidationResult
    {
        public bool IsValid { get; private init; }
        public bool WasAutoCopied { get; private init; }
        public string Message { get; private init; } = string.Empty;
        public string DllPath { get; private init; } = string.Empty;
        public string? CopiedFrom { get; private init; }

        private OodleValidationResult() { }

        public static OodleValidationResult Success(string dllPath, bool wasAutoCopied, string? copiedFrom = null)
        {
            string message = wasAutoCopied
                ? $"'{Path.GetFileName(dllPath)}' was automatically copied from Sekiro install."
                : $"'{Path.GetFileName(dllPath)}' found. Ready to read Sekiro files.";

            return new OodleValidationResult
            {
                IsValid = true,
                WasAutoCopied = wasAutoCopied,
                Message = message,
                DllPath = dllPath,
                CopiedFrom = copiedFrom
            };
        }

        public static OodleValidationResult Failure(string message) =>
            new OodleValidationResult
            {
                IsValid = false,
                WasAutoCopied = false,
                Message = message,
                DllPath = string.Empty
            };
    }
}