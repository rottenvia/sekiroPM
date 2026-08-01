using SekiroParamMerger.Core;
using SekiroParamMerger.Core.Models;

namespace SekiroParamMerger.WinForms
{
    public partial class ConflictResolverForm : Form
    {
        private readonly MergeResult _mergeResult;
        private readonly string _modAName;
        private readonly string _modBName;
        private readonly List<ConflictItem> _conflicts;
        private int _currentIndex = 0;

        public ConflictResolverForm(MergeResult mergeResult, string modAName, string modBName)
        {
            _mergeResult = mergeResult;
            _modAName    = modAName;
            _modBName    = modBName;
            _conflicts   = mergeResult.ResolvedConflicts;

            InitializeComponent();
            ApplyStyling();
            ShowConflict(_currentIndex);
        }

        private void ApplyStyling()
        {
            Styling.ApplyDarkTheme(this);
            this.Text = $"Conflict Resolver — {_conflicts.Count} conflict(s)";
            this.Icon = AppIcon.Create();
            this.ClientSize = new Size(820, 620);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Shown += (_, _) => Styling.MakeWindowRounded(this, 20);
        }

        private void ShowConflict(int index)
        {
            if (index < 0 || index >= _conflicts.Count) return;

            var conflict = _conflicts[index];

            // ── Progress ──────────────────────────────────────────────────────
            lblProgress.Text = $"⚔  Conflict {index + 1} of {_conflicts.Count}";

            // ── Category ──────────────────────────────────────────────────────
            string category = ParamDescriptions.GetParamCategory(conflict.ParamName);
            lblCategory.Text = $"📁 {category}";

            // ── Field description ─────────────────────────────────────────────
            string fieldDesc = ParamDescriptions.GetCellDescription(conflict.CellName);
            lblFieldLabel.Text = $"🔧 {fieldDesc}";

            bool isPadding = ParamDescriptions.IsPaddingField(conflict.CellName);

            // ── Technical details ─────────────────────────────────────────────
            lblTechnical.Text =
                $"Param: {conflict.ParamName}   |   " +
                $"Row ID: {conflict.RowId}   |   " +
                $"Field: {conflict.CellName}";

            lblPaddingWarning.Text = isPadding
                ? "⚠ This is a padding/technical field. Safe to ignore — it does not affect gameplay."
                : string.Empty;
            lblPaddingWarning.Visible = isPadding;

            // ── Vanilla value ─────────────────────────────────────────────────
            lblVanillaValue.Text = $"Vanilla was:   {FormatValue(conflict.VanillaValue)}";

            // ── Mod A value ───────────────────────────────────────────────────
            string changeDescA = ParamDescriptions.DescribeChange(conflict.VanillaValue, conflict.ModAValue);
            lblModAValue.Text  = $"{_modAName}:";
            lblModAChange.Text = $"{FormatValue(conflict.ModAValue)}   →   {changeDescA}";

            // ── Mod B value ───────────────────────────────────────────────────
            string changeDescB = ParamDescriptions.DescribeChange(conflict.VanillaValue, conflict.ModBValue);
            lblModBValue.Text  = $"{_modBName}:";
            lblModBChange.Text = $"{FormatValue(conflict.ModBValue)}   →   {changeDescB}";

            // ── Current resolution ────────────────────────────────────────────
            lblCurrentChoice.Text =
                $"Currently using: {conflict.ResolvedBy}  ({FormatValue(conflict.ResolvedValue)})";
            lblCurrentChoice.ForeColor = conflict.ResolvedBy == _modAName
                ? Styling.ModAColor : Styling.ModBColor;

            // ── Button labels ─────────────────────────────────────────────────
            btnUseModA.Text = $"✓ Use {_modAName}  ({FormatValue(conflict.ModAValue)})";
            btnUseModB.Text = $"✓ Use {_modBName}  ({FormatValue(conflict.ModBValue)})";

            // ── Navigation ────────────────────────────────────────────────────
            btnPrevious.Enabled = index > 0;
            btnNext.Enabled     = index < _conflicts.Count - 1;
            btnDone.Enabled     = true;

            UpdateProgressSummary();
        }

        private void UpdateProgressSummary()
        {
            int resolvedA = _conflicts.Count(c => c.ResolvedBy == _modAName);
            int resolvedB = _conflicts.Count(c => c.ResolvedBy == _modBName);
            lblSummary.Text =
                $"Resolved: {_modAName} winning {resolvedA}   |   {_modBName} winning {resolvedB}";
        }

        // ── Button handlers ───────────────────────────────────────────────────

        private void btnUseModA_Click(object sender, EventArgs e)
        {
            _conflicts[_currentIndex].ResolvedValue = _conflicts[_currentIndex].ModAValue;
            _conflicts[_currentIndex].ResolvedBy    = _modAName;
            ShowConflict(_currentIndex);
            if (_currentIndex < _conflicts.Count - 1) MoveToNext();
        }

        private void btnUseModB_Click(object sender, EventArgs e)
        {
            _conflicts[_currentIndex].ResolvedValue = _conflicts[_currentIndex].ModBValue;
            _conflicts[_currentIndex].ResolvedBy    = _modBName;
            ShowConflict(_currentIndex);
            if (_currentIndex < _conflicts.Count - 1) MoveToNext();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentIndex > 0) { _currentIndex--; ShowConflict(_currentIndex); }
        }

        private void btnNext_Click(object sender, EventArgs e) => MoveToNext();

        private void btnSkip_Click(object sender, EventArgs e) => MoveToNext();

        private void MoveToNext()
        {
            if (_currentIndex < _conflicts.Count - 1)
            { _currentIndex++; ShowConflict(_currentIndex); }
        }

        private void btnAllModA_Click(object sender, EventArgs e)
        {
            if (!ConfirmBulk($"{_modAName}")) return;
            for (int i = _currentIndex; i < _conflicts.Count; i++)
            {
                _conflicts[i].ResolvedValue = _conflicts[i].ModAValue;
                _conflicts[i].ResolvedBy    = _modAName;
            }
            ShowConflict(_currentIndex);
        }

        private void btnAllModB_Click(object sender, EventArgs e)
        {
            if (!ConfirmBulk($"{_modBName}")) return;
            for (int i = _currentIndex; i < _conflicts.Count; i++)
            {
                _conflicts[i].ResolvedValue = _conflicts[i].ModBValue;
                _conflicts[i].ResolvedBy    = _modBName;
            }
            ShowConflict(_currentIndex);
        }

        private bool ConfirmBulk(string winner)
        {
            return MessageBox.Show(
                $"Set ALL remaining conflicts to use {winner}?\n\n" +
                $"This affects {_conflicts.Count - _currentIndex} conflict(s).",
                "Bulk Resolve", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            // The ConflictItems in _mergeResult.ResolvedConflicts are the same
            // objects we modified — nothing else to persist.
            DialogResult = DialogResult.OK;
            Close();
        }

        private static string FormatValue(object? value)
        {
            if (value == null) return "null";
            if (value is byte[] bytes) return $"[{bytes.Length} bytes]";
            return value.ToString() ?? "null";
        }
    }
}
