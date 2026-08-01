using SekiroParamMerger.Core;
using SekiroParamMerger.Core.Models;

namespace SekiroParamMerger.WinForms
{
    public partial class PostMergeForm : Form
    {
        private readonly MergeResult   _mergeResult;
        private readonly string        _vanillaPath;
        private readonly string        _outputFolder;
        private readonly string        _modAPath;
        private readonly string        _modBPath;
        private readonly string        _modAName;
        private readonly string        _modBName;
        private readonly ParamLoader   _loader;
        private readonly AppSettings   _settings;

        private string _finalOutputPath = string.Empty;

        public PostMergeForm(
            MergeResult mergeResult,
            string vanillaPath,
            string outputFolder,
            string modAPath, string modBPath,
            string modAName, string modBName,
            ParamLoader loader,
            AppSettings settings)
        {
            _mergeResult  = mergeResult;
            _vanillaPath  = vanillaPath;
            _outputFolder = outputFolder;
            _modAPath     = modAPath;
            _modBPath     = modBPath;
            _modAName     = modAName;
            _modBName     = modBName;
            _loader       = loader;
            _settings     = settings;

            InitializeComponent();
            ApplyStyling();
            PopulateSummary();
        }

        private void ApplyStyling()
        {
            Styling.ApplyDarkTheme(this);
            this.Text = "Merge Complete";
            this.Size = new Size(680, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize   = new Size(640, 500);

            Styling.StyleHeader(lblTitle);
            Styling.StyleCard(pnlSummary);
            Styling.StyleCard(pnlDelete);
            Styling.StyleCheckBox(chkKeepFiles);
            Styling.StyleButton(btnSave, isPrimary: true);
            Styling.StyleButton(btnClose);
        }

        private void PopulateSummary()
        {
            // ── Output path ───────────────────────────────────────────────────
            _finalOutputPath = Path.Combine(
                _outputFolder, "param", "gameparam", "gameparam.parambnd.dcx");

            lblOutputPath.Text = $"Output: {_finalOutputPath}";
            lblOutputPath.ForeColor = Styling.TextSecondary;

            // ── Stats ─────────────────────────────────────────────────────────
            lblStatA.Text      = $"Cells from {_modAName}: {_mergeResult.TotalCellsFromA}";
            lblStatA.ForeColor = Styling.ModAColor;

            lblStatB.Text      = $"Cells from {_modBName}: {_mergeResult.TotalCellsFromB}";
            lblStatB.ForeColor = Styling.ModBColor;

            if (_mergeResult.TotalConflicts == 0)
            {
                lblConflicts.Text      = "✓ No conflicts — both mods change different things";
                lblConflicts.ForeColor = Styling.TextSuccess;
            }
            else
            {
                lblConflicts.Text =
                    $"⚠ {_mergeResult.TotalConflicts} conflict(s) resolved — " +
                    $"{_mergeResult.ResolvedConflicts.Count(c => c.ResolvedBy == _modAName)} " +
                    $"won by {_modAName}, " +
                    $"{_mergeResult.ResolvedConflicts.Count(c => c.ResolvedBy == _modBName)} " +
                    $"won by {_modBName}";
                lblConflicts.ForeColor = Styling.TextWarning;
            }

            // ── Delete section ────────────────────────────────────────────────
            chkKeepFiles.Checked = _settings.KeepModFilesAfterMerge;

            string modAParamFolder = GetParamFolder(_modAPath);
            string modBParamFolder = GetParamFolder(_modBPath);

            lblDeleteInfo.Text =
                $"After saving, the tool can delete the individual mod param folders\n" +
                $"to prevent conflicts with the new merged file:\n\n" +
                $"  • {modAParamFolder}\n" +
                $"  • {modBParamFolder}\n\n" +
                $"Uncheck the box below to keep them instead.";
            lblDeleteInfo.ForeColor = Styling.TextSecondary;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled  = false;
            btnClose.Enabled = false;
            lblSaveStatus.Text = "Saving...";
            lblSaveStatus.ForeColor = Styling.TextWarning;

            try
            {
                await Task.Run(() =>
                {
                    // ── Apply conflict resolutions to merged param bytes ───────
                    ApplyConflictResolutions();

                    // ── Write the merged file ─────────────────────────────────
                    var writer = new ParamWriter();
                    writer.Write(_vanillaPath, _mergeResult, _finalOutputPath);
                });

                lblSaveStatus.Text      = $"✓ Saved to: {_finalOutputPath}";
                lblSaveStatus.ForeColor = Styling.TextSuccess;

                // ── Handle deletion ───────────────────────────────────────────
                if (!chkKeepFiles.Checked)
                {
                    await DeleteModParamFoldersAsync();
                }

                // ── Final success message ─────────────────────────────────────
                string message =
                    $"Merge complete!\n\n" +
                    $"Saved to:\n{_finalOutputPath}\n\n" +
                    $"Cells from {_modAName}: {_mergeResult.TotalCellsFromA}\n" +
                    $"Cells from {_modBName}: {_mergeResult.TotalCellsFromB}\n" +
                    $"Conflicts resolved: {_mergeResult.TotalConflicts}\n\n" +
                    (!chkKeepFiles.Checked
                        ? "Mod A and Mod B param folders have been deleted.\n"
                        : "Mod param folders were kept (as requested).\n") +
                    "\nThe merged file is ready — launch Sekiro with Mod Engine!";

                MessageBox.Show(message, "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnClose.Enabled = true;
            }
            catch (Exception ex)
            {
                lblSaveStatus.Text      = $"✗ Save failed: {ex.Message}";
                lblSaveStatus.ForeColor = Styling.TextDanger;
                MessageBox.Show(
                    $"Could not save merged file:\n\n{ex.Message}",
                    "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled  = true;
                btnClose.Enabled = true;
            }
        }

        private void ApplyConflictResolutions()
        {
            // Re-apply all user-chosen conflict resolutions to the merged param bytes
            // For now the MergeResult already has the right bytes from Phase 3 logic
            // In a future version this is where we'd re-run the write with chosen values
            // The conflict items already have ResolvedValue set by ConflictResolverForm
        }

        private async Task DeleteModParamFoldersAsync()
        {
            string modAParamFolder = GetParamFolder(_modAPath);
            string modBParamFolder = GetParamFolder(_modBPath);

            var deletedFolders = new List<string>();
            var failedFolders  = new List<string>();

            await Task.Run(() =>
            {
                // Delete Mod A's param folder
                TryDeleteFolder(modAParamFolder, deletedFolders, failedFolders);

                // Delete Mod B's param folder (only if different from Mod A)
                if (!string.Equals(modAParamFolder, modBParamFolder,
                    StringComparison.OrdinalIgnoreCase))
                    TryDeleteFolder(modBParamFolder, deletedFolders, failedFolders);
            });

            if (failedFolders.Count > 0)
            {
                MessageBox.Show(
                    $"Could not delete these folders (they may be in use):\n\n" +
                    string.Join("\n", failedFolders) + "\n\n" +
                    "Please delete them manually to avoid conflicts.",
                    "Partial Deletion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static void TryDeleteFolder(string folder, List<string> deleted, List<string> failed)
        {
            try
            {
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, recursive: true);
                    deleted.Add(folder);
                }
            }
            catch
            {
                failed.Add(folder);
            }
        }

        /// <summary>
        /// Gets the 'param' folder containing a gameparam.parambnd.dcx file.
        /// e.g. C:\mods\SomeMod\param\gameparam\gameparam.parambnd.dcx
        ///   → C:\mods\SomeMod\param
        /// We delete the whole \param\ folder and everything inside it.
        /// </summary>
        private static string GetParamFolder(string paramFilePath)
        {
            // Go up: gameparam.parambnd.dcx → gameparam → param
            string? gameparamDir = Path.GetDirectoryName(paramFilePath);
            string? paramDir     = Path.GetDirectoryName(gameparamDir);
            return paramDir ?? string.Empty;
        }

        private void chkKeepFiles_CheckedChanged(object sender, EventArgs e)
        {
            _settings.KeepModFilesAfterMerge = chkKeepFiles.Checked;
            _settings.Save();
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
