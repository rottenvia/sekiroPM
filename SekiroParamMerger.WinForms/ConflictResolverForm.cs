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
            this.Size = new Size(700, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(660, 520);

            Styling.StyleButton(btnUseModA, isPrimary: false);
            Styling.StyleButton(btnUseModB, isPrimary: false);
            Styling.StyleButton(btnPrevious);
            Styling.StyleButton(btnNext);
            Styling.StyleButton(btnSkip);
            Styling.StyleButton(btnAllModA, isPrimary: true);
            Styling.StyleButton(btnAllModB, isPrimary: false);
            Styling.StyleButton(btnDone, isPrimary: true);
            Styling.StyleCard(pnlInfo);
            Styling.StyleCard(pnlValues);
            Styling.StyleCard(pnlNav);

            btnUseModA.BackColor = Styling.ModAColor;
            btnUseModA.FlatAppearance.BorderColor = Styling.ModAColor;
            btnUseModB.BackColor = Styling.ModBColor;
            btnUseModB.FlatAppearance.BorderColor = Styling.ModBColor;

            Styling.StyleHeader(lblConflictTitle);
            Styling.StyleHeader(lblCategory);
            Styling.StyleHeader(lblFieldLabel);
        }

        private void ShowConflict(int index)
        {
            if (index < 0 || index >= _conflicts.Count) return;

            var conflict = _conflicts[index];

            // ── Progress ──────────────────────────────────────────────────────
            lblProgress.Text = $"Conflict {index + 1} of {_conflicts.Count}";
            lblProgress.ForeColor = Styling.TextSecondary;

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
            lblTechnical.ForeColor = Styling.TextSecondary;

            // ── Padding warning ───────────────────────────────────────────────
            if (isPadding)
            {
                lblPaddingWarning.Text    = "⚠ This is a padding/technical field. It is safe to ignore — it does not affect gameplay.";
                lblPaddingWarning.Visible = true;
            }
            else
            {
                lblPaddingWarning.Visible = false;
            }

            // ── Vanilla value ─────────────────────────────────────────────────
            lblVanillaValue.Text = $"Vanilla was:   {FormatValue(conflict.VanillaValue)}";

            // ── Mod A value ───────────────────────────────────────────────────
            string changeDescA = ParamDescriptions.DescribeChange(conflict.VanillaValue, conflict.ModAValue);
            lblModAValue.Text  = $"{_modAName}:";
            lblModAChange.Text = $"{FormatValue(conflict.ModAValue)}  →  {changeDescA}";
            lblModAChange.ForeColor = Styling.ModAColor;

            // ── Mod B value ───────────────────────────────────────────────────
            string changeDescB = ParamDescriptions.DescribeChange(conflict.VanillaValue, conflict.ModBValue);
            lblModBValue.Text  = $"{_modBName}:";
            lblModBChange.Text = $"{FormatValue(conflict.ModBValue)}  →  {changeDescB}";
            lblModBChange.ForeColor = Styling.ModBColor;

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

            UpdateProgress();
        }

        private void UpdateProgress()
        {
            int resolvedA = _conflicts.Count(c => c.ResolvedBy == _modAName);
            int resolvedB = _conflicts.Count(c => c.ResolvedBy == _modBName);
            lblSummary.Text =
                $"Resolved: {_modAName} winning {resolvedA} | {_modBName} winning {resolvedB}";
            lblSummary.ForeColor = Styling.TextSecondary;
        }

        // ── Button handlers ───────────────────────────────────────────────────

        private void btnUseModA_Click(object sender, EventArgs e)
        {
            _conflicts[_currentIndex].ResolvedValue = _conflicts[_currentIndex].ModAValue;
            _conflicts[_currentIndex].ResolvedBy    = _modAName;
            ShowConflict(_currentIndex);
            if (_currentIndex < _conflicts.Count - 1)
                MoveToNext();
        }

        private void btnUseModB_Click(object sender, EventArgs e)
        {
            _conflicts[_currentIndex].ResolvedValue = _conflicts[_currentIndex].ModBValue;
            _conflicts[_currentIndex].ResolvedBy    = _modBName;
            ShowConflict(_currentIndex);
            if (_currentIndex < _conflicts.Count - 1)
                MoveToNext();
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
            var confirm = MessageBox.Show(
                $"Set ALL remaining conflicts to use {_modAName}?\n\n" +
                $"This affects {_conflicts.Count - _currentIndex} conflict(s).",
                "Bulk Resolve", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            for (int i = _currentIndex; i < _conflicts.Count; i++)
            {
                _conflicts[i].ResolvedValue = _conflicts[i].ModAValue;
                _conflicts[i].ResolvedBy    = _modAName;
            }
            ShowConflict(_currentIndex);
        }

        private void btnAllModB_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                $"Set ALL remaining conflicts to use {_modBName}?\n\n" +
                $"This affects {_conflicts.Count - _currentIndex} conflict(s).",
                "Bulk Resolve", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            for (int i = _currentIndex; i < _conflicts.Count; i++)
            {
                _conflicts[i].ResolvedValue = _conflicts[i].ModBValue;
                _conflicts[i].ResolvedBy    = _modBName;
            }
            ShowConflict(_currentIndex);
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            // Apply all user resolutions to the merge result
            // The conflict items in _mergeResult.ResolvedConflicts are the same objects
            // we've been modifying — no extra step needed
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
