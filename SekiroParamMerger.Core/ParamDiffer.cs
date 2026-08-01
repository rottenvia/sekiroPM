using SoulsFormats;
using SekiroParamMerger.Core.Models;

namespace SekiroParamMerger.Core
{
    /// <summary>
    /// Produces a DiffResult by comparing a modded LoadedParamBundle against a vanilla baseline.
    ///
    /// DIFF LOGIC:
    ///   For each param in the mod:
    ///     - If param not in vanilla         → mark entire param as NEW
    ///     - If paramdef not applied         → mark as SKIPPED (pass through)
    ///     - Otherwise, compare row by row:
    ///         Row in mod but not vanilla    → ADDED row
    ///         Row in vanilla but not mod    → REMOVED row
    ///         Row in both                  → compare cells, record MODIFIED cells only
    ///
    /// IMPORTANT NOTES:
    ///   - Rows are matched by ID (row.ID)
    ///   - Duplicate row IDs are handled via ToLookup (not ToDictionary — avoids crash)
    ///   - Cell values compared with .Equals() not == (type-safe object comparison)
    ///   - Rows identical to vanilla produce NO output (not stored)
    /// </summary>
    public class ParamDiffer
    {
        /// <summary>
        /// Compares a modded bundle against a vanilla bundle and returns the full diff.
        /// </summary>
        /// <param name="vanilla">The vanilla (unmodded) param bundle — the baseline.</param>
        /// <param name="modded">The modded param bundle to diff against vanilla.</param>
        /// <param name="modDisplayName">
        /// A friendly name for the mod shown in reports and conflict UI.
        /// e.g. "Combat Overhaul" or the folder name of the mod.
        /// </param>
        public DiffResult Diff(LoadedParamBundle vanilla, LoadedParamBundle modded, string modDisplayName)
        {
            var result = new DiffResult
            {
                ModFilePath    = modded.SourceFilePath,
                ModDisplayName = modDisplayName
            };

            // ── Process every param in the modded file ────────────────────────────
            foreach (var kvp in modded.Params)
            {
                string paramName    = kvp.Key;
                LoadedParam modParam = kvp.Value;

                // ── Case 1: Param exists in mod but not in vanilla at all ──────────
                // Treat the entire param as new — include it as-is in output
                if (!vanilla.Params.ContainsKey(paramName))
                {
                    result.ChangedParams[paramName] = new ParamDiff
                    {
                        ParamName  = paramName,
                        IsNewParam = true
                    };
                    result.Warnings.Add(
                        $"[INFO] '{paramName}' exists in mod but not in vanilla — treated as new param.");
                    continue;
                }

                LoadedParam vanillaParam = vanilla.Params[paramName];

                // ── Case 2: No paramdef on either side — skip, can't cell-diff ────
                if (!modParam.ParamdefApplied || !vanillaParam.ParamdefApplied)
                {
                    result.SkippedParams.Add(paramName);
                    result.Warnings.Add(
                        $"[WARN] '{paramName}' has no paramdef — cannot diff at cell level. " +
                        $"Will pass through from higher-priority mod unchanged.");
                    continue;
                }

                // ── Case 3: Both have paramdefs — do a full cell-level diff ───────
                ParamDiff paramDiff = DiffParam(vanillaParam.Param, modParam.Param, paramName, result.Warnings);

                // Only store params that actually have changes — no point storing identical params
                if (paramDiff.HasChanges)
                    result.ChangedParams[paramName] = paramDiff;
            }

            return result;
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Private: Diff a single param at row and cell level
        // ─────────────────────────────────────────────────────────────────────────

        private ParamDiff DiffParam(PARAM vanillaParam, PARAM moddedParam, string paramName, List<string> warnings)
        {
            var paramDiff = new ParamDiff { ParamName = paramName };

            // Build lookups by row ID
            // CRITICAL: Use ToLookup not ToDictionary
            // ToLookup handles duplicate IDs gracefully (documented edge case in Sekiro params)
            // ToDictionary would throw InvalidOperationException on duplicate keys
            var vanillaById = vanillaParam.Rows.ToLookup(r => r.ID);
            var moddedById  = moddedParam.Rows.ToLookup(r => r.ID);

            // Collect all unique row IDs across both vanilla and mod
            var allRowIds = vanillaParam.Rows.Select(r => r.ID)
                .Union(moddedParam.Rows.Select(r => r.ID))
                .Distinct();

            foreach (int rowId in allRowIds)
            {
                var vanillaRows = vanillaById[rowId].ToList();
                var moddedRows  = moddedById[rowId].ToList();

                // ── Row exists in mod but not vanilla → ADDED ─────────────────────
                if (vanillaRows.Count == 0 && moddedRows.Count > 0)
                {
                    foreach (var addedRow in moddedRows)
                    {
                        paramDiff.ChangedRows.Add(new RowDiff
                        {
                            RowId    = rowId,
                            RowName  = addedRow.Name ?? string.Empty,
                            DiffType = RowDiffType.Added
                        });
                    }
                    continue;
                }

                // ── Row exists in vanilla but not mod → REMOVED ───────────────────
                if (moddedRows.Count == 0 && vanillaRows.Count > 0)
                {
                    foreach (var removedRow in vanillaRows)
                    {
                        paramDiff.ChangedRows.Add(new RowDiff
                        {
                            RowId    = rowId,
                            RowName  = removedRow.Name ?? string.Empty,
                            DiffType = RowDiffType.Removed
                        });
                    }
                    continue;
                }

                // ── Row exists in both → compare cells ────────────────────────────
                // Match by position when there are duplicates
                // (if vanilla has 2 rows with ID X and mod has 2 rows with ID X,
                //  we compare them positionally: vanilla[0] vs mod[0], vanilla[1] vs mod[1])
                int matchCount = Math.Min(vanillaRows.Count, moddedRows.Count);

                for (int i = 0; i < matchCount; i++)
                {
                    PARAM.Row vRow = vanillaRows[i];
                    PARAM.Row mRow = moddedRows[i];

                    List<CellChange> cellChanges = DiffRow(vRow, mRow, paramName, rowId, warnings);

                    if (cellChanges.Count > 0)
                    {
                        paramDiff.ChangedRows.Add(new RowDiff
                        {
                            RowId       = rowId,
                            RowName     = mRow.Name ?? string.Empty,
                            DiffType    = RowDiffType.Modified,
                            CellChanges = cellChanges
                        });
                    }
                    // If cellChanges is empty — row is identical to vanilla, we don't store it
                }

                // Handle count mismatch (one side has more duplicate rows than the other)
                // Extra vanilla rows = removed, extra mod rows = added
                if (vanillaRows.Count > moddedRows.Count)
                {
                    for (int i = matchCount; i < vanillaRows.Count; i++)
                    {
                        paramDiff.ChangedRows.Add(new RowDiff
                        {
                            RowId    = rowId,
                            RowName  = vanillaRows[i].Name ?? string.Empty,
                            DiffType = RowDiffType.Removed
                        });
                    }
                }
                else if (moddedRows.Count > vanillaRows.Count)
                {
                    for (int i = matchCount; i < moddedRows.Count; i++)
                    {
                        paramDiff.ChangedRows.Add(new RowDiff
                        {
                            RowId    = rowId,
                            RowName  = moddedRows[i].Name ?? string.Empty,
                            DiffType = RowDiffType.Added
                        });
                    }
                }
            }

            return paramDiff;
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Private: Diff a single row at cell level
        // ─────────────────────────────────────────────────────────────────────────

        private List<CellChange> DiffRow(PARAM.Row vanillaRow, PARAM.Row moddedRow,
            string paramName, int rowId, List<string> warnings)
        {
            var changes = new List<CellChange>();

            // Get all cells from both rows
            // We iterate vanilla cells and look for matching mod cells by InternalName
            // This is safer than iterating by index — paramdef field order is consistent
            // but we don't assume it
            try
            {
                foreach (PARAM.Cell vCell in vanillaRow.Cells)
                {
                    string cellName = vCell.Def.InternalName;

                    // Find the matching cell in the mod row by the same internal name
                    PARAM.Cell? mCell = moddedRow[cellName];

                    if (mCell == null)
                    {
                        // Cell exists in vanilla but not in mod row
                        // This shouldn't happen if both rows use the same paramdef
                        // but we handle it defensively
                        warnings.Add(
                            $"[WARN] Cell '{cellName}' in {paramName} row {rowId} " +
                            $"exists in vanilla but not in mod — skipping this cell.");
                        continue;
                    }

                    object? vValue = vCell.Value;
                    object? mValue = mCell.Value;

                    // Handle null values defensively
                    if (vValue == null && mValue == null)
                        continue; // both null = equal

                    if (vValue == null || mValue == null)
                    {
                        // One is null and the other isn't — that's a change
                        changes.Add(new CellChange
                        {
                            CellName     = cellName,
                            VanillaValue = vValue ?? "(null)",
                            ModValue     = mValue ?? "(null)"
                        });
                        continue;
                    }

                    // CRITICAL: Use .Equals() not == for object comparison
                    // == on object checks reference equality, not value equality
                    // .Equals() correctly compares int to int, float to float etc.
                    //
                    // SPECIAL CASE: byte[] (used for padding fields like pad0, pad1)
                    // byte[].Equals() checks reference equality — two identical arrays look different
                    // We must use SequenceEqual() for byte arrays to compare content correctly
                    // Without this fix, every padding field would appear as a false conflict in Phase 3
                    bool valuesAreEqual;
                    if (vValue is byte[] vBytes && mValue is byte[] mBytes)
                        valuesAreEqual = vBytes.SequenceEqual(mBytes);
                    else
                        valuesAreEqual = vValue.Equals(mValue);

                    if (!valuesAreEqual)
                    {
                        changes.Add(new CellChange
                        {
                            CellName     = cellName,
                            VanillaValue = vValue,
                            ModValue     = mValue
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Something unexpected happened reading cells
                // Don't crash — log it and return whatever changes we found so far
                warnings.Add(
                    $"[WARN] Error reading cells in {paramName} row {rowId}: {ex.Message}\n" +
                    $"       Partial cell diff may be inaccurate for this row.");
            }

            return changes;
        }
    }
}
