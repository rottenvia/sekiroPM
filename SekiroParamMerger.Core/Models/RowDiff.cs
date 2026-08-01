namespace SekiroParamMerger.Core.Models
{
    /// <summary>
    /// Represents what changed in a single param row compared to vanilla.
    /// </summary>
    public class RowDiff
    {
        /// <summary>
        /// The row's unique ID number.
        /// We match rows between vanilla and mod by this ID.
        /// </summary>
        public int RowId { get; set; }

        /// <summary>
        /// The row's name — usually empty in Sekiro (stripped before release).
        /// We store it anyway for display purposes.
        /// </summary>
        public string RowName { get; set; } = string.Empty;

        /// <summary>
        /// What kind of change this row represents.
        /// </summary>
        public RowDiffType DiffType { get; set; }

        /// <summary>
        /// The list of individual cell changes within this row.
        /// Only populated when DiffType is Modified.
        /// Empty for Added and Removed rows.
        /// </summary>
        public List<CellChange> CellChanges { get; set; } = new();
    }

    public enum RowDiffType
    {
        /// <summary>Row exists in both vanilla and mod but one or more cells differ.</summary>
        Modified,

        /// <summary>Row exists in mod but NOT in vanilla — mod added a new row.</summary>
        Added,

        /// <summary>Row exists in vanilla but NOT in mod — mod deleted a row.</summary>
        Removed
    }
}
