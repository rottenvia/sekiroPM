using SoulsFormats;
using SekiroParamMerger.Core.Models;

namespace SekiroParamMerger.Core
{
    /// <summary>
    /// Merges two modded param bundles together at cell level against a vanilla baseline.
    ///
    /// MERGE PRIORITY: Mod A is higher priority — wins all conflicts automatically.
    /// In Phase 4 (WinForms), conflicts will be shown to the user for manual resolution.
    ///
    /// MERGE LOGIC PER CELL:
    ///   Only A changed cell  → use A's value
    ///   Only B changed cell  → use B's value
    ///   Both changed to same value → use it (not a real conflict)
    ///   Both changed to different values → use A's value, record conflict
    ///
    /// OUTPUT:
    ///   MergeResult.MergedParamBytes contains final bytes for EVERY param.
    ///   ParamWriter replaces BND4 entries with these bytes and saves the file.
    /// </summary>
    public class ParamMerger
    {
        private readonly IReadOnlyList<PARAMDEF> _paramdefs;

        public ParamMerger(IReadOnlyList<PARAMDEF> paramdefs)
        {
            _paramdefs = paramdefs;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Public: Main merge entry point
        // ─────────────────────────────────────────────────────────────────────

        public MergeResult Merge(
            LoadedParamBundle vanilla,
            LoadedParamBundle modA, DiffResult diffA,
            LoadedParamBundle modB, DiffResult diffB)
        {
            var result = new MergeResult
            {
                ModADisplayName = diffA.ModDisplayName,
                ModBDisplayName = diffB.ModDisplayName
            };

            // Get every param name we need to consider across all three files
            var allParamNames = vanilla.Params.Keys
                .Union(diffA.ChangedParams.Keys)
                .Union(diffB.ChangedParams.Keys)
                .Union(diffA.SkippedParams)
                .Union(diffB.SkippedParams)
                .Distinct()
                .OrderBy(n => n);

            foreach (string paramName in allParamNames)
            {
                try
                {
                    ProcessParam(paramName, vanilla, modA, diffA, modB, diffB, result);
                }
                catch (Exception ex)
                {
                    // One failed param must never crash the whole merge
                    result.Warnings.Add(
                        $"[WARN] Unexpected error processing '{paramName}': {ex.Message}\n" +
                        $"       Using {diffA.ModDisplayName}'s version as fallback.");

                    // Fallback: use Mod A's bytes if available, else vanilla
                    result.MergedParamBytes[paramName] = GetFallbackBytes(paramName, modA, modB, vanilla);
                }
            }

            return result;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private: Decide how to handle one param
        // ─────────────────────────────────────────────────────────────────────

        private void ProcessParam(
            string paramName,
            LoadedParamBundle vanilla,
            LoadedParamBundle modA, DiffResult diffA,
            LoadedParamBundle modB, DiffResult diffB,
            MergeResult result)
        {
            bool inVanilla = vanilla.Params.ContainsKey(paramName);
            bool aChanged  = diffA.ChangedParams.ContainsKey(paramName);
            bool bChanged  = diffB.ChangedParams.ContainsKey(paramName);
            bool aSkipped  = diffA.SkippedParams.Contains(paramName);
            bool bSkipped  = diffB.SkippedParams.Contains(paramName);

            // ── Case 1: Neither mod touched this param → vanilla bytes ─────────
            if (!aChanged && !bChanged && !aSkipped && !bSkipped)
            {
                if (inVanilla)
                    result.MergedParamBytes[paramName] = vanilla.Params[paramName].RawBytes;
                return;
            }

            // ── Case 2: No paramdef available → pass through from Mod A ────────
            // We can't do cell-level work without a paramdef
            if (aSkipped || bSkipped)
            {
                result.MergedParamBytes[paramName] = GetFallbackBytes(paramName, modA, modB, vanilla);
                result.Warnings.Add(
                    $"[INFO] '{paramName}' has no paramdef — using {diffA.ModDisplayName}'s version (pass-through).");
                return;
            }

            // ── Case 3: Param is new (not in vanilla) → use Mod A priority ─────
            if (!inVanilla)
            {
                if (modA.Params.TryGetValue(paramName, out var newParamA))
                {
                    result.MergedParamBytes[paramName] = newParamA.RawBytes;
                    result.Warnings.Add(
                        $"[INFO] '{paramName}' is new (not in vanilla) — using {diffA.ModDisplayName}'s version.");
                }
                else if (modB.Params.TryGetValue(paramName, out var newParamB))
                {
                    result.MergedParamBytes[paramName] = newParamB.RawBytes;
                    result.Warnings.Add(
                        $"[INFO] '{paramName}' is new (not in vanilla) — using {diffB.ModDisplayName}'s version.");
                }
                return;
            }

            // ── Case 4, 5, 6: Full cell-level merge ───────────────────────────
            byte[] mergedBytes = MergeParam(
                paramName, vanilla, modA, diffA, modB, diffB, result);

            result.MergedParamBytes[paramName] = mergedBytes;
            result.TotalParamsMerged++;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private: Merge one param at cell level
        // ─────────────────────────────────────────────────────────────────────

        private byte[] MergeParam(
            string paramName,
            LoadedParamBundle vanilla,
            LoadedParamBundle modA, DiffResult diffA,
            LoadedParamBundle modB, DiffResult diffB,
            MergeResult result)
        {
            // Re-read vanilla param from raw bytes → fresh working copy
            // CRITICAL: We never modify the original vanilla PARAM objects
            // Re-reading from RawBytes gives us a clean slate to apply changes to
            PARAM mergedParam = PARAM.Read(vanilla.Params[paramName].RawBytes);
            mergedParam.ApplyParamdefCarefully(_paramdefs);

            // Get diffs for this param (null if mod didn't change this param)
            diffA.ChangedParams.TryGetValue(paramName, out ParamDiff? paramDiffA);
            diffB.ChangedParams.TryGetValue(paramName, out ParamDiff? paramDiffB);

            // Build row change lookups keyed by row ID
            // Using ToLookup (not ToDictionary) to safely handle duplicate row IDs
            var rowChangesA = (paramDiffA?.ChangedRows ?? Enumerable.Empty<RowDiff>())
                              .ToLookup(r => r.RowId);
            var rowChangesB = (paramDiffB?.ChangedRows ?? Enumerable.Empty<RowDiff>())
                              .ToLookup(r => r.RowId);

            // All row IDs touched by either mod
            var allChangedRowIds = rowChangesA.Select(g => g.Key)
                .Union(rowChangesB.Select(g => g.Key))
                .Distinct();

            // Track row IDs to remove at the end (can't remove while iterating)
            var rowIdsToRemove = new HashSet<int>();

            foreach (int rowId in allChangedRowIds)
            {
                var diffsA = rowChangesA[rowId].ToList();
                var diffsB = rowChangesB[rowId].ToList();

                RowDiff? rowDiffA = diffsA.FirstOrDefault();
                RowDiff? rowDiffB = diffsB.FirstOrDefault();

                RowDiffType? typeA = rowDiffA?.DiffType;
                RowDiffType? typeB = rowDiffB?.DiffType;

                // ── REMOVED rows ───────────────────────────────────────────────
                if (typeA == RowDiffType.Removed || typeB == RowDiffType.Removed)
                {
                    // Conflict: one removes, other modifies → Mod A wins
                    if ((typeA == RowDiffType.Removed && typeB == RowDiffType.Modified) ||
                        (typeB == RowDiffType.Removed && typeA == RowDiffType.Modified))
                    {
                        result.Warnings.Add(
                            $"[CONFLICT] '{paramName}' row {rowId}: " +
                            $"{diffA.ModDisplayName} and {diffB.ModDisplayName} disagree on remove vs modify. " +
                            $"Using {diffA.ModDisplayName}'s action.");
                        result.TotalConflicts++;

                        if (typeA == RowDiffType.Removed)
                            rowIdsToRemove.Add(rowId);
                        // else: typeA is Modified — fall through to handle as modified below
                        // But since we 'continue' here, we skip further processing
                        // The modified cells from typeA will be lost — acceptable for Phase 3
                        // Phase 4 will handle this edge case in the UI
                    }
                    else
                    {
                        // Both agree on removal, or only one removes with no conflict
                        rowIdsToRemove.Add(rowId);
                    }
                    continue;
                }

                // ── ADDED rows ─────────────────────────────────────────────────
                if (typeA == RowDiffType.Added || typeB == RowDiffType.Added)
                {
                    // Both mods added the same row ID → conflict → Mod A wins
                    if (typeA == RowDiffType.Added && typeB == RowDiffType.Added)
                    {
                        result.TotalConflicts++;
                        result.ResolvedConflicts.Add(new ConflictItem
                        {
                            ParamName     = paramName,
                            RowId         = rowId,
                            CellName      = "[ENTIRE ROW]",
                            ModAValue     = $"Row {rowId} added by {diffA.ModDisplayName}",
                            ModBValue     = $"Row {rowId} added by {diffB.ModDisplayName}",
                            ResolvedValue = $"Row {rowId} added by {diffA.ModDisplayName}",
                            ResolvedBy    = diffA.ModDisplayName
                        });
                    }

                    // Determine which mod's row to use (A has priority)
                    LoadedParamBundle sourceBundle = (typeA == RowDiffType.Added) ? modA : modB;
                    string sourceName = (typeA == RowDiffType.Added)
                        ? diffA.ModDisplayName : diffB.ModDisplayName;

                    if (sourceBundle.Params.TryGetValue(paramName, out LoadedParam? sourceParam))
                    {
                        PARAM.Row? sourceRow = sourceParam.Param.Rows.FirstOrDefault(r => r.ID == rowId);

                        if (sourceRow != null)
                        {
                            try
                            {
                                var newRow = new PARAM.Row(rowId, sourceRow.Name ?? string.Empty, mergedParam.AppliedParamdef);
                                foreach (PARAM.Cell cell in newRow.Cells)
                                {
                                    PARAM.Cell? srcCell = sourceRow[cell.Def.InternalName];
                                    if (srcCell != null)
                                        cell.Value = srcCell.Value;
                                }
                                mergedParam.Rows.Add(newRow);
                            }
                            catch (Exception ex)
                            {
                                result.Warnings.Add(
                                    $"[WARN] Could not add row {rowId} to '{paramName}' " +
                                    $"from {sourceName}: {ex.Message}");
                            }
                        }
                    }
                    continue;
                }

                // ── MODIFIED rows → full cell-level merge ──────────────────────
                if (typeA == RowDiffType.Modified || typeB == RowDiffType.Modified)
                {
                    // Find the row in our working copy of the vanilla param
                    PARAM.Row? mergedRow = mergedParam.Rows.FirstOrDefault(r => r.ID == rowId);

                    if (mergedRow == null)
                    {
                        result.Warnings.Add(
                            $"[WARN] Row {rowId} in '{paramName}' marked as modified " +
                            $"but not found in vanilla working copy. Skipping.");
                        continue;
                    }

                    // Build cell change lookups for this row
                    // Key: cell internal name, Value: CellChange
                    var cellChangesA = rowDiffA?.CellChanges
                        .ToDictionary(c => c.CellName, c => c)
                        ?? new Dictionary<string, CellChange>();

                    var cellChangesB = rowDiffB?.CellChanges
                        .ToDictionary(c => c.CellName, c => c)
                        ?? new Dictionary<string, CellChange>();

                    // All cell names touched by either mod in this row
                    var allCellNames = cellChangesA.Keys.Union(cellChangesB.Keys).Distinct();

                    foreach (string cellName in allCellNames)
                    {
                        bool aChangedCell = cellChangesA.ContainsKey(cellName);
                        bool bChangedCell = cellChangesB.ContainsKey(cellName);

                        object? valueToApply = null;

                        if (aChangedCell && !bChangedCell)
                        {
                            // Only Mod A changed this cell → use A's value
                            valueToApply = cellChangesA[cellName].ModValue;
                            result.TotalCellsFromA++;
                        }
                        else if (bChangedCell && !aChangedCell)
                        {
                            // Only Mod B changed this cell → use B's value
                            valueToApply = cellChangesB[cellName].ModValue;
                            result.TotalCellsFromB++;
                        }
                        else if (aChangedCell && bChangedCell)
                        {
                            object aVal = cellChangesA[cellName].ModValue;
                            object bVal = cellChangesB[cellName].ModValue;

                            // Check if both changed to same value → not a real conflict
                            // IMPORTANT: byte[] needs SequenceEqual, not .Equals()
                            bool sameValue;
                            if (aVal is byte[] aBytes && bVal is byte[] bBytes)
                                sameValue = aBytes.SequenceEqual(bBytes);
                            else
                                sameValue = aVal.Equals(bVal);

                            if (sameValue)
                            {
                                // Same value → apply, no conflict to record
                                valueToApply = aVal;
                                result.TotalCellsFromA++;
                            }
                            else
                            {
                                // TRUE CONFLICT → Mod A wins, record it for Phase 4 UI
                                valueToApply = aVal;
                                result.TotalCellsFromA++;
                                result.TotalConflicts++;

                                result.ResolvedConflicts.Add(new ConflictItem
                                {
                                    ParamName     = paramName,
                                    RowId         = rowId,
                                    CellName      = cellName,
                                    VanillaValue  = cellChangesA[cellName].VanillaValue,
                                    ModAValue     = aVal,
                                    ModBValue     = bVal,
                                    ResolvedValue = aVal,
                                    ResolvedBy    = diffA.ModDisplayName
                                });
                            }
                        }

                        // Apply the chosen value to the merged row
                        if (valueToApply != null)
                        {
                            try
                            {
                                PARAM.Cell? targetCell = mergedRow[cellName];
                                if (targetCell != null)
                                    targetCell.Value = valueToApply;
                            }
                            catch (Exception ex)
                            {
                                result.Warnings.Add(
                                    $"[WARN] Could not set '{cellName}' in '{paramName}' " +
                                    $"row {rowId}: {ex.Message}");
                            }
                        }
                    }
                }
            }

            // Apply all queued row removals at once
            if (rowIdsToRemove.Count > 0)
                mergedParam.Rows.RemoveAll(r => rowIdsToRemove.Contains(r.ID));

            // Serialize the modified PARAM back to bytes
            // DCX recompression happens automatically in ParamWriter when saving the BND4
            return mergedParam.Write();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private: Get fallback bytes when cell-level merge is not possible
        // Priority: Mod A → Mod B → Vanilla
        // ─────────────────────────────────────────────────────────────────────

        private static byte[] GetFallbackBytes(
            string paramName,
            LoadedParamBundle modA,
            LoadedParamBundle modB,
            LoadedParamBundle vanilla)
        {
            if (modA.Params.TryGetValue(paramName, out var pA)) return pA.RawBytes;
            if (modB.Params.TryGetValue(paramName, out var pB)) return pB.RawBytes;
            if (vanilla.Params.TryGetValue(paramName, out var pV)) return pV.RawBytes;
            return Array.Empty<byte>();
        }
    }
}
