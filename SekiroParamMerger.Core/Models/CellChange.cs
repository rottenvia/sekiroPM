namespace SekiroParamMerger.Core.Models
{
    /// <summary>
    /// A single cell that was changed in a modded param row vs vanilla.
    /// </summary>
    public class CellChange
    {
        /// <summary>
        /// Internal field name e.g. "damage", "weight", "stamina".
        /// Comes from cell.Def.InternalName in SoulsFormats.
        /// </summary>
        public string CellName { get; set; } = string.Empty;

        /// <summary>
        /// Value in the vanilla file.
        /// Type is object — could be int, float, byte, short, bool, uint etc.
        /// Always compare using .Equals(), never ==
        /// </summary>
        public object VanillaValue { get; set; } = null!;

        /// <summary>
        /// Value in the modded file.
        /// </summary>
        public object ModValue { get; set; } = null!;
    }
}
