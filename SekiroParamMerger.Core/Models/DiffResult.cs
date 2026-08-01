namespace SekiroParamMerger.Core.Models
{
    /// <summary>
    /// The complete diff of one mod's gameparam.parambnd.dcx vs vanilla.
    /// This is the output of ParamDiffer and the input to ParamMerger (Phase 3).
    /// </summary>
    public class DiffResult
    {
        /// <summary>
        /// Full path to the mod file that was diffed.
        /// Used for display in conflict resolution so the user knows which mod a value came from.
        /// </summary>
        public string ModFilePath { get; set; } = string.Empty;

        /// <summary>
        /// A friendly display name for this mod — derived from the file path.
        /// e.g. "gameparam.parambnd.dcx (ModA)"
        /// </summary>
        public string ModDisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Diffs for every param that had ANY changes vs vanilla.
        /// Params identical to vanilla are not stored here.
        /// Keyed by param name e.g. "SpEffectParam".
        /// </summary>
        public Dictionary<string, ParamDiff> ChangedParams { get; set; } = new();

        /// <summary>
        /// Names of params that were skipped because no paramdef was available.
        /// These will pass through from the higher priority mod unchanged.
        /// </summary>
        public List<string> SkippedParams { get; set; } = new();

        /// <summary>
        /// Non-fatal warnings collected during diffing.
        /// </summary>
        public List<string> Warnings { get; set; } = new();

        /// <summary>
        /// Convenience — total number of changed rows across all params.
        /// </summary>
        public int TotalChangedRows => ChangedParams.Values.Sum(p => p.ChangedRows.Count);

        /// <summary>
        /// Convenience — total number of changed cells across all params and rows.
        /// </summary>
        public int TotalChangedCells => ChangedParams.Values
            .SelectMany(p => p.ChangedRows)
            .Sum(r => r.CellChanges.Count);
    }
}
