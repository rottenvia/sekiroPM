namespace SekiroParamMerger.Core.Models
{
    public class LoadedParamBundle
    {
        /// <summary>
        /// Full path to the source file this bundle was loaded from.
        /// Used in conflict reporting so the user knows which mod a value came from.
        /// </summary>
        public string SourceFilePath { get; set; } = string.Empty;

        /// <summary>
        /// All successfully loaded params keyed by param name.
        /// Example key: "SpEffectParam"
        /// Only contains params where PARAM.Read() succeeded.
        /// </summary>
        public Dictionary<string, LoadedParam> Params { get; set; } = new();

        /// <summary>
        /// Non-fatal warnings collected during loading.
        /// Logged to the user but do not stop the process.
        /// Examples: missing paramdef, empty bytes, duplicate param name.
        /// </summary>
        public List<string> Warnings { get; set; } = new();

        /// <summary>
        /// Names of params that loaded but had no matching paramdef.
        /// These pass through unchanged during merge — not merged at cell level.
        /// </summary>
        public List<string> SkippedParams { get; set; } = new();
    }
}