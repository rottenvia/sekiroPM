using SoulsFormats;
using SekiroParamMerger.Core.Models;

namespace SekiroParamMerger.Core
{
    /// <summary>
    /// Writes a MergeResult to disk as a valid gameparam.parambnd.dcx file.
    ///
    /// HOW IT WORKS:
    ///   1. Re-reads the vanilla BND4 as a structural template
    ///      (preserves all BND4 metadata — flags, version, timestamps etc.)
    ///   2. Replaces each param's bytes with the merged version from MergeResult
    ///   3. Saves the BND4 to the output path — DCX recompression is automatic
    ///
    /// WHY WE USE VANILLA AS TEMPLATE:
    ///   The BND4 archive contains metadata beyond just the param files.
    ///   Starting from vanilla ensures we never corrupt that metadata.
    ///   We only touch the param file bytes inside — nothing else.
    /// </summary>
    public class ParamWriter
    {
        /// <summary>
        /// Writes the merged param bundle to disk.
        /// </summary>
        /// <param name="vanillaFilePath">
        /// Path to the vanilla gameparam.parambnd.dcx.
        /// Used as the structural template for the output BND4.
        /// </param>
        /// <param name="mergeResult">The merge result from ParamMerger.</param>
        /// <param name="outputPath">Where to save the merged file.</param>
        /// <exception cref="FileNotFoundException">If vanilla file is missing.</exception>
        /// <exception cref="UnauthorizedAccessException">If output path is not writable.</exception>
        public void Write(string vanillaFilePath, MergeResult mergeResult, string outputPath)
        {
            // ── Guard: vanilla file must exist ────────────────────────────────
            if (!File.Exists(vanillaFilePath))
            {
                throw new FileNotFoundException(
                    $"Vanilla file not found for writing:\n  {vanillaFilePath}\n\n" +
                    $"The vanilla file is needed as a structural template.\n" +
                    $"Make sure it still exists and has not been moved.");
            }

            // ── Create output directory if it doesn't exist ───────────────────
            string? outputDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                try
                {
                    Directory.CreateDirectory(outputDir);
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $"Could not create output directory:\n  {outputDir}\n\n" +
                        $"Details: {ex.Message}");
                }
            }

            // ── Step 1: Re-read vanilla BND4 as our working template ──────────
            // DCX decompression is automatic — requires oo2core.dll (already validated)
            BND4 bnd;
            try
            {
                bnd = BND4.Read(vanillaFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Could not re-read vanilla file for writing:\n  {vanillaFilePath}\n\n" +
                    $"Details: {ex.Message}");
            }

            // ── Step 2: Replace param bytes in the BND4 ───────────────────────
            int replacedCount = 0;
            int keptVanillaCount = 0;

            foreach (BinderFile binderFile in bnd.Files)
            {
                string fileName = Path.GetFileName(binderFile.Name);

                // Only touch .param files — leave everything else alone
                if (!fileName.EndsWith(".param", StringComparison.OrdinalIgnoreCase))
                    continue;

                string paramName = Path.GetFileNameWithoutExtension(fileName);

                if (mergeResult.MergedParamBytes.TryGetValue(paramName, out byte[]? mergedBytes)
                    && mergedBytes.Length > 0)
                {
                    // Replace with our merged bytes
                    binderFile.Bytes = mergedBytes;
                    replacedCount++;
                }
                else
                {
                    // No merged version — keep vanilla bytes (already there, no action needed)
                    keptVanillaCount++;
                }
            }

            Console.WriteLine($"  Params replaced with merged version : {replacedCount}");
            Console.WriteLine($"  Params kept as vanilla              : {keptVanillaCount}");

            // ── Step 3: Write the BND4 to output path ─────────────────────────
            // BND4.Write() automatically reapplies DCX compression
            // The compression type is remembered from when we read the file
            try
            {
                bnd.Write(outputPath);
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException(
                    $"Cannot write to:\n  {outputPath}\n\n" +
                    $"Possible causes:\n" +
                    $"  • The folder requires administrator permissions\n" +
                    $"  • Antivirus software is blocking the write\n" +
                    $"  • The file is already open in another program\n\n" +
                    $"Try saving to your Desktop instead:\n" +
                    $"  C:\\Users\\{Environment.UserName}\\Desktop\\merged_gameparam.parambnd.dcx");
            }
            catch (IOException ex)
            {
                throw new IOException(
                    $"IO error writing merged file:\n  {outputPath}\n\n" +
                    $"Details: {ex.Message}\n\n" +
                    $"Try saving to your Desktop instead.");
            }
        }
    }
}
