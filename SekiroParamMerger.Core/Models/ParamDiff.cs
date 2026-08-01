namespace SekiroParamMerger.Core.Models
{
    /// <summary>
    /// The full diff of one param file (e.g. SpEffectParam) between vanilla and a mod.
    /// </summary>
    public class ParamDiff
    {
        /// <summary>
        /// Name of the param e.g. "SpEffectParam", "AtkParam_Pc".
        /// </summary>
        public string ParamName { get; set; } = string.Empty;

        /// <summary>
        /// All rows that differ from vanilla in any way.
        /// Rows that are identical to vanilla are NOT stored here — no point.
        /// </summary>
        public List<RowDiff> ChangedRows { get; set; } = new();

        /// <summary>
        /// True if this param had no paramdef applied and could not be diffed at cell level.
        /// When true, ChangedRows will be empty and this param passes through unchanged.
        /// </summary>
        public bool WasSkipped { get; set; }

        /// <summary>
        /// True if this param exists in the mod but not at all in vanilla.
        /// The entire param is treated as new and included as-is.
        /// </summary>
        public bool IsNewParam { get; set; }

        /// <summary>
        /// Convenience property — true if this param actually has differences.
        /// </summary>
        public bool HasChanges => ChangedRows.Count > 0 || IsNewParam;
    }
}
