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
            this.Icon = AppIcon.Create();
            this.ClientSize = new Size(760, 580);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Shown += (_, _) => Styling.MakeWindowRounded(this, 20);
        }

        private void PopulateSummary()
        {
            _finalOutputPath = Path.Combine(
                _outputFolder, "param", "gameparam", "gameparam.parambnd.dcx");

            lblStatA.Text = $"Cells from {_modAName}: {_mergeResult.TotalCellsFromA}";
            lblStatB.Text = $"Cells from {_modBName}: {_mergeResult.TotalCellsFromB}";

            if (_mergeResult.TotalConflicts == 0)
            {
                lblConflicts.Text = "✓ No conflicts — both mods change different things.";
                lblConflicts.ForeColor = Styling.TextSuccess;
            }
            else
            {
                lblConflicts.Text =
                    $"⚠ {_mergeResult.TotalConflicts} conflict(s) resolved — " +
                    $"{_mergeResult.ResolvedConflicts.Count(c => c.ResolvedBy == _modAName)} won by {_modAName}, " +
                    $"{_mergeResult.ResolvedConflicts.Count(c => c.ResolvedBy == _modBName)} won by {_modBName}";
                lblConflicts.ForeColor = Styling.TextWarning;
            }

            lblOutputPath.Text =
                $"Output:\n{_finalOutputPath}";

            // ── Delete section ────────────────────────────────────────────────
            chkKeepFiles.Checked = _settings.KeepModFilesAfterMerge;

            string modAParamFolder = GetParamFolder(_modAPath);
            string modBParamFolder = GetParamFolder(_modBPath);

            lblDeleteInfo.Text =
                $"After saving, the tool can delete each mod's whole param folder to\n" +
                $"prevent conflicts with the new merged file:\n\n" +
                $"   • {modAParamFolder}\n" +
                $"   • {modBParamFolder}\n\n" +
                $"Uncheck the box below to keep them instead.";
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled  = false;
            btnClose.Enabled = false;
            lblSaveStatus.Text = "Saving…";
            lblSaveStatus.ForeColor = Styling.TextWarning;

            try
            {
                await Task.Run(() =>
                {
                    ApplyConflictResolutions();

                    var writer = new ParamWriter();
                    writer.Write(_vanillaPath, _mergeResult, _finalOutputPath);
                });

                lblSaveStatus.Text      = $"✓ Saved to: {_finalOutputPath}";
                lblSaveStatus.ForeColor = Styling.TextSuccess;

                if (!chkKeepFiles.Checked)
                    await DeleteModParamFoldersAsync();

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
            // ConflictItems already carry the user-chosen ResolvedValue set by
            // ConflictResolverForm, and ParamMerger baked those values into
            // MergeResult.MergedParamBytes. Nothing further to do here.
        }

        private async Task DeleteModParamFoldersAsync()
        {
            string modAParamFolder = GetParamFolder(_modAPath);
            string modBParamFolder = GetParamFolder(_modBPath);

            var deletedFolders = new List<string>();
            var failedFolders  = new List<string>();

            await Task.Run(() =>
            {
                TryDeleteFolder(modAParamFolder, deletedFolders, failedFolders);
                if (!string.Equals(modAParamFolder, modBParamFolder, StringComparison.OrdinalIgnoreCase))
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
        /// ...\SomeMod\param\gameparam\gameparam.parambnd.dcx  →  ...\SomeMod\param
        /// We delete the whole \param\ folder and everything inside it.
        /// </summary>
        private static string GetParamFolder(string paramFilePath)
        {
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
