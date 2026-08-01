using SekiroParamMerger.Core;
using SekiroParamMerger.Core.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;

PrintHeader("SEKIRO PARAM MERGER — Phase 3: Merge");

string toolDir = AppContext.BaseDirectory;

// ── STEP 1: Validate oo2core DLL ─────────────────────────────────────────────
PrintStep(1, "Checking for oo2core_6_win64.dll...");

OodleValidationResult oodleResult = OodleValidator.Validate(toolDir);
if (!oodleResult.IsValid) { PrintFail(oodleResult.Message); PauseAndExit(); return; }

if (oodleResult.WasAutoCopied)
    PrintSuccess($"Auto-copied from: {oodleResult.CopiedFrom}");
else
    PrintSuccess(oodleResult.Message);

Console.WriteLine();

// ── STEP 2: Load paramdefs ────────────────────────────────────────────────────
PrintStep(2, "Loading Sekiro paramdefs...");

string paramdefDir = Path.Combine(toolDir, "Resources", "Paramdefs");
ParamLoader loader;
try { loader = new ParamLoader(paramdefDir); }
catch (Exception ex) { PrintFail(ex.Message); PauseAndExit(); return; }

PrintSuccess($"{loader.ParamdefCount} paramdefs loaded.");
Console.WriteLine();

// ── STEP 3: Vanilla file ──────────────────────────────────────────────────────
PrintStep(3, "Locate your VANILLA gameparam.parambnd.dcx");
Console.WriteLine();
Console.WriteLine("  This MUST be the original unmodded file from Sekiro.");
Console.WriteLine("  Usually at:");
Console.WriteLine(@"  C:\Program Files (x86)\Steam\steamapps\common\Sekiro\param\gameparam\gameparam.parambnd.dcx");
Console.WriteLine();
Console.Write("  Paste vanilla path: ");

string vanillaPath = Console.ReadLine()?.Trim().Trim('"') ?? string.Empty;
if (string.IsNullOrWhiteSpace(vanillaPath)) { PrintFail("No path entered."); PauseAndExit(); return; }
Console.WriteLine();

// ── STEP 4: Mod A file ────────────────────────────────────────────────────────
PrintStep(4, "Locate Mod A's gameparam.parambnd.dcx  (MOD A = HIGHER PRIORITY — wins conflicts)");
Console.WriteLine();
Console.Write("  Paste Mod A path: ");

string modAPath = Console.ReadLine()?.Trim().Trim('"') ?? string.Empty;
if (string.IsNullOrWhiteSpace(modAPath)) { PrintFail("No path entered."); PauseAndExit(); return; }

Console.Write("  Enter a display name for Mod A (e.g. Combat Overhaul): ");
string modAName = Console.ReadLine()?.Trim() ?? "Mod A";
if (string.IsNullOrWhiteSpace(modAName)) modAName = "Mod A";
Console.WriteLine();

// ── STEP 5: Mod B file ────────────────────────────────────────────────────────
PrintStep(5, "Locate Mod B's gameparam.parambnd.dcx");
Console.WriteLine();
Console.Write("  Paste Mod B path: ");

string modBPath = Console.ReadLine()?.Trim().Trim('"') ?? string.Empty;
if (string.IsNullOrWhiteSpace(modBPath)) { PrintFail("No path entered."); PauseAndExit(); return; }

Console.Write("  Enter a display name for Mod B (e.g. Enemy Rebalance): ");
string modBName = Console.ReadLine()?.Trim() ?? "Mod B";
if (string.IsNullOrWhiteSpace(modBName)) modBName = "Mod B";
Console.WriteLine();

// ── STEP 6: Load all three bundles ────────────────────────────────────────────
PrintStep(6, "Loading all three param files...");
Console.WriteLine();

LoadedParamBundle vanillaBundle, modABundle, modBBundle;

try
{
    Console.Write("  Loading vanilla...  ");
    vanillaBundle = loader.LoadParamBundle(vanillaPath);
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"OK ({vanillaBundle.Params.Count} params)");
    Console.ResetColor();
}
catch (Exception ex) { PrintFail($"Failed to load vanilla:\n{ex.Message}"); PauseAndExit(); return; }

try
{
    Console.Write($"  Loading {modAName}...  ");
    modABundle = loader.LoadParamBundle(modAPath);
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"OK ({modABundle.Params.Count} params)");
    Console.ResetColor();
}
catch (Exception ex) { PrintFail($"Failed to load Mod A:\n{ex.Message}"); PauseAndExit(); return; }

try
{
    Console.Write($"  Loading {modBName}...  ");
    modBBundle = loader.LoadParamBundle(modBPath);
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"OK ({modBBundle.Params.Count} params)");
    Console.ResetColor();
}
catch (Exception ex) { PrintFail($"Failed to load Mod B:\n{ex.Message}"); PauseAndExit(); return; }

Console.WriteLine();

// ── STEP 7: Diff both mods against vanilla ────────────────────────────────────
PrintStep(7, "Running cell-level diffs against vanilla...");
Console.WriteLine();

var differ = new ParamDiffer();

DiffResult diffA, diffB;

Console.Write($"  Diffing {modAName}... ");
diffA = differ.Diff(vanillaBundle, modABundle, modAName);
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"OK  ({diffA.ChangedParams.Count} params changed, {diffA.TotalChangedCells} cells)");
Console.ResetColor();

Console.Write($"  Diffing {modBName}... ");
diffB = differ.Diff(vanillaBundle, modBBundle, modBName);
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"OK  ({diffB.ChangedParams.Count} params changed, {diffB.TotalChangedCells} cells)");
Console.ResetColor();

Console.WriteLine();

// ── STEP 8: Merge ─────────────────────────────────────────────────────────────
PrintStep(8, $"Merging at cell level ({modAName} has priority over {modBName})...");
Console.WriteLine();

var merger = new ParamMerger(loader.Paramdefs);
MergeResult mergeResult;

try
{
    mergeResult = merger.Merge(vanillaBundle, modABundle, diffA, modBBundle, diffB);
}
catch (Exception ex)
{
    PrintFail($"Merge failed unexpectedly:\n{ex.Message}");
    PauseAndExit();
    return;
}

PrintSuccess("Merge complete.");
Console.WriteLine();

// ── STEP 9: Merge report ──────────────────────────────────────────────────────
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("══════════════════════════════════════════════════════════");
Console.WriteLine("  MERGE REPORT");
Console.WriteLine("══════════════════════════════════════════════════════════");
Console.ResetColor();
Console.WriteLine();

Console.WriteLine($"  Params merged (cell-level) : {mergeResult.TotalParamsMerged}");
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"  Cells from {modAName,-20}: {mergeResult.TotalCellsFromA}");
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine($"  Cells from {modBName,-20}: {mergeResult.TotalCellsFromB}");
Console.ResetColor();

if (mergeResult.TotalConflicts > 0)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"  Conflicts (auto-resolved)  : {mergeResult.TotalConflicts}  ← {modAName} won all");
    Console.ResetColor();
    Console.WriteLine();

    // Show first 10 conflicts so user can see what happened
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"  First {Math.Min(10, mergeResult.ResolvedConflicts.Count)} conflicts (showing param / row / cell):");
    Console.ResetColor();

    foreach (var conflict in mergeResult.ResolvedConflicts.Take(10))
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"    {conflict.ParamName} → Row {conflict.RowId} → {conflict.CellName}");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"      {modAName}: {conflict.ModAValue}");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("  vs  ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{modBName}: {conflict.ModBValue}");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"      Used: {conflict.ResolvedValue} ({conflict.ResolvedBy} wins)");
        Console.ResetColor();
    }

    if (mergeResult.TotalConflicts > 10)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"    ... and {mergeResult.TotalConflicts - 10} more conflicts (all won by {modAName})");
        Console.ResetColor();
    }
}
else
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"  Conflicts                  : 0  ← perfect, no overlapping cell changes!");
    Console.ResetColor();
}

if (mergeResult.Warnings.Count > 0)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"  Warnings: {mergeResult.Warnings.Count} (non-fatal)");
    Console.ResetColor();
}

Console.WriteLine();

// ── STEP 9: Output path ───────────────────────────────────────────────────────
string defaultOutputPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
    "merged_gameparam.parambnd.dcx");

PrintStep(9, "Where to save the merged file?");
Console.WriteLine();
Console.WriteLine($"  Default: {defaultOutputPath}");
Console.WriteLine("  Press ENTER to use default, or paste a custom path:");
Console.Write("  ");

string? customOutput = Console.ReadLine()?.Trim().Trim('"');
string outputPath = string.IsNullOrWhiteSpace(customOutput) ? defaultOutputPath : customOutput;

Console.WriteLine();

// ── STEP 10: Write output ─────────────────────────────────────────────────────
PrintStep(10, $"Writing merged file to:");
Console.WriteLine($"  {outputPath}");
Console.WriteLine();

var writer = new ParamWriter();
try
{
    writer.Write(vanillaPath, mergeResult, outputPath);
}
catch (Exception ex)
{
    PrintFail($"Could not write output file:\n{ex.Message}");
    PauseAndExit();
    return;
}

PrintSuccess($"File saved successfully.");
Console.WriteLine();

// ── FINAL RESULT ──────────────────────────────────────────────────────────────
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("══════════════════════════════════════════════════════════");
Console.WriteLine("  PHASE 3 RESULT");
Console.WriteLine("══════════════════════════════════════════════════════════");
Console.ResetColor();
Console.WriteLine();

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("  PHASE 3 PASSED.");
Console.WriteLine();
Console.WriteLine("  Your merged file is ready at:");
Console.ResetColor();
Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine($"  {outputPath}");
Console.ResetColor();
Console.WriteLine();
Console.WriteLine("  HOW TO INSTALL:");
Console.WriteLine("  Copy the merged file into your mod engine mods folder:");
Console.WriteLine(@"  [mods folder]\param\gameparam\gameparam.parambnd.dcx");
Console.WriteLine();
Console.WriteLine("  Make sure ONLY the merged file is active for gameparam —");
Console.WriteLine("  disable the individual Mod A and Mod B gameparam files");
Console.WriteLine("  so the merged version is the only one loaded.");
Console.WriteLine();

if (mergeResult.TotalConflicts > 0)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"  NOTE: {mergeResult.TotalConflicts} conflict(s) were auto-resolved in favour of {modAName}.");
    Console.WriteLine($"  If you want {modBName} to win some conflicts instead,");
    Console.WriteLine($"  re-run the tool with the mods swapped (put {modBName} as Mod A).");
    Console.ResetColor();
    Console.WriteLine();
}

PauseAndExit();

// ── Helpers ───────────────────────────────────────────────────────────────────
static void PrintHeader(string text)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("══════════════════════════════════════════════════════════");
    Console.WriteLine($"  {text}");
    Console.WriteLine("══════════════════════════════════════════════════════════");
    Console.ResetColor();
    Console.WriteLine();
}

static void PrintStep(int number, string text)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"[STEP {number}] ");
    Console.ResetColor();
    Console.WriteLine(text);
}

static void PrintSuccess(string text)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("  [OK] ");
    Console.ResetColor();
    Console.WriteLine(text);
}

static void PrintFail(string text)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("  [FAIL]");
    Console.ResetColor();
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(text);
    Console.ResetColor();
    Console.WriteLine();
}

static void PauseAndExit()
{
    Console.WriteLine("Press any key to exit.");
    Console.ReadKey();
}
