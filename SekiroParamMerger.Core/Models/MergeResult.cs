namespace SekiroParamMerger.Core.Models
{
    /// <summary>
    /// The complete output of ParamMerger.
    /// Contains the final merged bytes for every param,
    /// plus a full report of what happened during the merge.
    /// ParamWriter uses MergedParamBytes to write the output file.
    /// </summary>
    public class MergeResult
    {
        /// <summary>Display name of Mod A (higher priority — wins all conflicts)</summary>
        public string ModADisplayName { get; set; } = string.Empty;

        /// <summary>Display name of Mod B</summary>
        public string ModBDisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Final merged param bytes keyed by param name e.g. "SpEffectParam".
        /// ParamWriter replaces each BND4 entry's bytes with these.
        /// Contains ALL params — not just changed ones.
        /// Unchanged params get vanilla bytes so the output file is complete.
        /// </summary>
        public Dictionary<string, byte[]> MergedParamBytes { get; set; } = new();

        /// <summary>
        /// All conflicts that were auto-resolved (Mod A wins by default in Phase 3).
        /// In Phase 4 WinForms these will be shown to the user for manual resolution.
        /// </summary>
        public List<ConflictItem> ResolvedConflicts { get; set; } = new();

        /// <summary>Non-fatal warnings collected during merging</summary>
        public List<string> Warnings { get; set; } = new();

        /// <summary>Number of params that went through cell-level merging</summary>
        public int TotalParamsMerged { get; set; }

        /// <summary>Number of cells where Mod A's value was used</summary>
        public int TotalCellsFromA { get; set; }

        /// <summary>Number of cells where Mod B's value was used</summary>
        public int TotalCellsFromB { get; set; }

        /// <summary>
        /// Number of true conflicts (same cell, different values).
        /// In Phase 3 these are auto-resolved with Mod A winning.
        /// </summary>
        public int TotalConflicts { get; set; }
    }
}
