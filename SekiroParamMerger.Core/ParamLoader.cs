using SoulsFormats;
using SekiroParamMerger.Core.Models;

namespace SekiroParamMerger.Core
{
    /// <summary>
    /// Handles loading a gameparam.parambnd.dcx file into a usable, structured form.
    ///
    /// THE LOADING CHAIN (every step explained):
    ///   1. BND4.Read(path)              — decompresses DCX, reads BND4 archive (requires oo2core DLL)
    ///   2. PARAM.Read(bytes)            — parses each .param binary file inside the archive
    ///   3. ApplyParamdefCarefully()     — applies XML paramdef so cells have names and types
    ///   4. Results go into LoadedParamBundle — a clean, safe object for the rest of the tool to use
    ///
    /// IMPORTANT: This class does NOT throw on individual param failures.
    /// It collects warnings and skips bad params, so one broken param doesn't kill the whole load.
    /// It DOES throw on critical failures (file not found, file is locked, not a valid BND4).
    /// </summary>
    public class ParamLoader
    {
        private readonly List<PARAMDEF> _paramdefs;

        /// <summary>How many paramdefs were successfully loaded from the XML directory.</summary>
        public int ParamdefCount => _paramdefs.Count;
        public IReadOnlyList<PARAMDEF> Paramdefs => _paramdefs.AsReadOnly();

        /// <summary>
        /// Creates a ParamLoader and immediately loads all paramdef XMLs from the given directory.
        /// </summary>
        /// <param name="paramdefDirectory">
        /// Path to the folder containing Sekiro (SDT) paramdef .xml files from Paramdex.
        /// Typically: [toolDirectory]\Resources\Paramdefs\
        /// </param>
        /// <exception cref="DirectoryNotFoundException">If the paramdef directory doesn't exist.</exception>
        /// <exception cref="FileNotFoundException">If no XML files are found in the directory.</exception>
        public ParamLoader(string paramdefDirectory)
        {
            _paramdefs = new List<PARAMDEF>();
            LoadParamdefs(paramdefDirectory);
        }

        // ─────────────────────────────────────────────────────────
        // Private: Load all paramdef XMLs
        // ─────────────────────────────────────────────────────────

        private void LoadParamdefs(string directory)
        {
            // Guard: directory must exist
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(
                    $"Paramdef directory not found:\n  {directory}\n\n" +
                    $"Make sure the 'Resources\\Paramdefs' folder exists next to the executable.\n" +
                    $"It should contain .xml files downloaded from the Paramdex repository (SDT folder).");
            }

            string[] xmlFiles = Directory.GetFiles(directory, "*.xml", SearchOption.AllDirectories);

            // Guard: must have at least some XML files
            if (xmlFiles.Length == 0)
            {
                throw new FileNotFoundException(
                    $"No paramdef XML files found in:\n  {directory}\n\n" +
                    $"Download them from https://github.com/soulsmods/Paramdex\n" +
                    $"Navigate to the Sekiro (or SDT) folder → Defs\n" +
                    $"Copy all .xml files into the Resources\\Paramdefs folder.");
            }

            int loadedCount = 0;
            int failedCount = 0;

            foreach (string xmlPath in xmlFiles)
            {
                try
                {
                    PARAMDEF def = PARAMDEF.XmlDeserialize(xmlPath);
                    _paramdefs.Add(def);
                    loadedCount++;
                }
                catch (Exception ex)
                {
                    // A single bad XML should NOT stop the whole load.
                    // Log it and keep going — we may have enough paramdefs for what we need.
                    Console.WriteLine($"  [WARN] Could not load paramdef '{Path.GetFileName(xmlPath)}': {ex.Message}");
                    failedCount++;
                }
            }

            if (loadedCount == 0)
            {
                // Every single XML failed — this is a critical failure
                throw new Exception(
                    $"All {failedCount} paramdef XML files failed to load.\n" +
                    $"Make sure you are using the correct Sekiro (SDT) paramdefs from Paramdex.\n" +
                    $"DS1/DS3/ER paramdefs will not work for Sekiro.");
            }

            Console.WriteLine($"  Loaded {loadedCount} paramdefs" +
                              (failedCount > 0 ? $" ({failedCount} failed — see warnings above)" : "."));
        }

        // ─────────────────────────────────────────────────────────
        // Public: Load a gameparam.parambnd.dcx file
        // ─────────────────────────────────────────────────────────

        /// <summary>
        /// Loads a gameparam.parambnd.dcx file and returns a structured bundle.
        ///
        /// THROWS on critical errors (file missing, locked, not a valid BND4).
        /// COLLECTS warnings on non-critical errors (individual bad params, missing paramdefs).
        /// </summary>
        /// <param name="parambndPath">Full path to gameparam.parambnd.dcx</param>
        public LoadedParamBundle LoadParamBundle(string parambndPath)
        {
            var bundle = new LoadedParamBundle { SourceFilePath = parambndPath };

            // ── Guard 1: File must exist ──────────────────────────────────────────
            if (!File.Exists(parambndPath))
            {
                throw new FileNotFoundException(
                    $"File not found:\n  {parambndPath}\n\n" +
                    $"Please check the path and try again.\n" +
                    $"The vanilla file is usually at:\n" +
                    $"  [Sekiro install]\\param\\gameparam\\gameparam.parambnd.dcx");
            }

            // ── Step 1: Read the BND4 archive (DCX decompression is automatic) ───
            // IMPORTANT: This requires oo2core_6_win64.dll to be in the process directory.
            // OodleValidator must have already confirmed this before we get here.
            BND4 bnd;
            try
            {
                bnd = BND4.Read(parambndPath);
            }
            catch (UnauthorizedAccessException)
            {
                // File is locked (Sekiro running, or Smithbox has it open)
                throw new UnauthorizedAccessException(
                    $"Cannot read the file — it is locked by another process:\n  {parambndPath}\n\n" +
                    $"Please close Sekiro and all other modding tools (Smithbox, DSMapStudio, etc.) then try again.");
            }
            catch (IOException ex)
            {
                throw new IOException(
                    $"IO error reading:\n  {parambndPath}\n\n" +
                    $"Make sure no other program has this file open.\n" +
                    $"Details: {ex.Message}");
            }
            catch (Exception ex)
            {
                // The most common cause here is: wrong file type (DS1/DS3 file), or missing oo2core DLL
                throw new Exception(
                    $"Could not read '{Path.GetFileName(parambndPath)}' as a BND4 archive.\n\n" +
                    $"Possible causes:\n" +
                    $"  • This is a DS1, DS3, or other non-Sekiro param file (wrong format)\n" +
                    $"  • The file is corrupted\n" +
                    $"  • oo2core_6_win64.dll is missing from the tool folder\n\n" +
                    $"Technical detail: {ex.Message}");
            }

            // ── Step 2: Process each file inside the BND4 archive ────────────────
            foreach (BinderFile binderFile in bnd.Files)
            {
                string fileName = Path.GetFileName(binderFile.Name);

                // Skip anything that isn't a .param file
                // (Some BND4 archives contain other file types — we ignore them)
                if (!fileName.EndsWith(".param", StringComparison.OrdinalIgnoreCase))
                    continue;

                string paramName = Path.GetFileNameWithoutExtension(fileName);

                // Guard: Empty bytes
                // An entry can exist in the archive but have no data — skip it, log it
                if (binderFile.Bytes == null || binderFile.Bytes.Length == 0)
                {
                    bundle.Warnings.Add($"[WARN] '{paramName}' has no data (0 bytes) — skipped.");
                    continue;
                }

                // Guard: Duplicate param name
                // Shouldn't happen in a valid gameparam, but we handle it defensively
                if (bundle.Params.ContainsKey(paramName))
                {
                    bundle.Warnings.Add($"[WARN] Duplicate param name '{paramName}' found — keeping first occurrence.");
                    continue;
                }

                // ── Step 3: Parse the raw bytes into a PARAM object ───────────────
                PARAM param;
                try
                {
                    param = PARAM.Read(binderFile.Bytes);
                }
                catch (Exception ex)
                {
                    // One bad param doesn't stop the whole load — collect warning and skip
                    bundle.Warnings.Add($"[WARN] Could not parse '{paramName}' as PARAM: {ex.Message} — skipped entirely.");
                    continue;
                }

                // ── Step 4: Apply paramdefs (the Rosetta Stone step) ─────────────
                // This gives each cell a name and type.
                // Without this, param.Rows exists but cells have no names — useless for merging.
                //
                // CRITICAL: We check the bool return value.
                // false = no matching paramdef found — we store the param but mark it as unapplied.
                // We never access named cells on an unapplied param.
                bool defApplied;
                try
                {
                    defApplied = param.ApplyParamdefCarefully(_paramdefs);
                }
                catch (Exception ex)
                {
                    // ApplyParamdefCarefully should never throw, but we wrap it defensively
                    bundle.Warnings.Add(
                        $"[WARN] Unexpected error applying paramdef to '{paramName}': {ex.Message}\n" +
                        $"       This param will be passed through from the higher-priority mod unchanged.");
                    defApplied = false;
                }

                // ── Step 5: Store the result ──────────────────────────────────────
                var loadedParam = new LoadedParam
                {
                    ParamName      = paramName,
                    Param          = param,
                    ParamdefApplied = defApplied,
                    BinderPath     = binderFile.Name,
                    RawBytes       = binderFile.Bytes
                };

                if (!defApplied)
                {
                    bundle.Warnings.Add(
                        $"[WARN] No matching paramdef for '{paramName}'.\n" +
                        $"       This param cannot be merged at cell level.\n" +
                        $"       It will be taken from the higher-priority mod without merging.");
                    bundle.SkippedParams.Add(paramName);
                }

                bundle.Params[paramName] = loadedParam;
            }

            return bundle;
        }
    }
}
