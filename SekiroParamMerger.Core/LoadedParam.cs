using SoulsFormats;

namespace SekiroParamMerger.Core.Models
{
    public class LoadedParam
    {
        /// <summary>
        /// The param's name e.g. "SpEffectParam", "AtkParam_Pc".
        /// Derived from the filename inside the BND4 archive.
        /// </summary>
        public string ParamName { get; set; } = string.Empty;

        /// <summary>
        /// The fully parsed PARAM object with paramdef applied.
        /// ONLY access .Rows and cell values if ParamdefApplied is TRUE.
        /// Accessing cells when ParamdefApplied is false will throw or give garbage.
        /// </summary>
        public PARAM Param { get; set; } = null!;

        /// <summary>
        /// True if a matching paramdef was found and successfully applied.
        /// False means we have no type info — this param passes through unchanged.
        /// </summary>
        public bool ParamdefApplied { get; set; }

        /// <summary>
        /// The internal BND4 path e.g. "\param\gameparam\SpEffectParam.param".
        /// Needed to write the file back into the correct slot in the archive.
        /// </summary>
        public string BinderPath { get; set; } = string.Empty;

        /// <summary>
        /// The original raw bytes of this param entry.
        /// Used as fallback when ParamdefApplied is false — passed through unchanged.
        /// </summary>
        public byte[] RawBytes { get; set; } = Array.Empty<byte>();
    }
}