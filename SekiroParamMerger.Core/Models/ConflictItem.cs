namespace SekiroParamMerger.Core.Models
{
    /// <summary>
    /// Represents a single cell that both mods changed to different values.
    /// In Phase 3 (console), Mod A wins all conflicts automatically.
    /// In Phase 4 (WinForms), these will be shown to the user for manual resolution.
    /// </summary>
    public class ConflictItem
    {
        /// <summary>Which param this conflict is in e.g. "SpEffectParam"</summary>
        public string ParamName { get; set; } = string.Empty;

        /// <summary>The row ID where the conflict occurred</summary>
        public int RowId { get; set; }

        /// <summary>The cell field name e.g. "damage", "stamina"</summary>
        public string CellName { get; set; } = string.Empty;

        /// <summary>The original vanilla value</summary>
        public object? VanillaValue { get; set; }

        /// <summary>The value Mod A wanted to set</summary>
        public object? ModAValue { get; set; }

        /// <summary>The value Mod B wanted to set</summary>
        public object? ModBValue { get; set; }

        /// <summary>The value that was actually used in the output</summary>
        public object? ResolvedValue { get; set; }

        /// <summary>Which mod's value was used e.g. "Combat Overhaul" or "ModB"</summary>
        public string ResolvedBy { get; set; } = string.Empty;
    }
}
