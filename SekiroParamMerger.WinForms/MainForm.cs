using SekiroParamMerger.Core;
using SekiroParamMerger.Core.Models;

namespace SekiroParamMerger.WinForms
{
    public partial class MainForm : Form
    {
        private AppSettings _settings;
        private ParamLoader? _loader;

        // ── State ─────────────────────────────────────────────────────────────
        private string _modAPath = string.Empty;
        private string _modAName = string.Empty;
        private string _modBPath = string.Empty;
        private string _modBName = string.Empty;

        public MainForm()
        {
            InitializeComponent();
            _settings = AppSettings.Load();
            ApplyStyling();
            LoadSettings();
            CheckFirstRun();
        }

        // ── Styling ───────────────────────────────────────────────────────────

        private void ApplyStyling()
        {
            Styling.ApplyDarkTheme(this);
            this.Text = "Sekiro Param Merger";
            this.MinimumSize = new Size(780, 620);
            this.Size = new Size(780, 660);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Icon = SystemIcons.Application;

            Styling.StyleHeader(lblTitle);
            Styling.StyleHeader(lblGameSection);
            Styling.StyleHeader(lblModASection);
            Styling.StyleHeader(lblModBSection);
            Styling.StyleHeader(lblOutputSection);

            Styling.StyleSecondary(lblVanillaStatus);
            Styling.StyleSecondary(lblGameFolderHint);
            Styling.StyleSecondary(lblOutputHint);

            Styling.StyleTextBox(txtGameFolder);
            Styling.StyleTextBox(txtModAPath);
            Styling.StyleTextBox(txtModAName);
            Styling.StyleTextBox(txtModBPath);
            Styling.StyleTextBox(txtModBName);
            Styling.StyleTextBox(txtOutputFolder);

            Styling.StyleButton(btnBrowseGame);
            Styling.StyleButton(btnBrowseModA);
            Styling.StyleButton(btnBrowseModB);
            Styling.StyleButton(btnBrowseOutput);
            Styling.StyleButton(btnMerge, isPrimary: true);

            Styling.StyleCheckBox(chkKeepModFiles);
            Styling.StyleCard(pnlGame);
            Styling.StyleCard(pnlModA);
            Styling.StyleCard(pnlModB);
            Styling.StyleCard(pnlOutput);
            Styling.StyleCard(pnlStatus);

            // Mod A accent
            lblModASection.ForeColor = Styling.ModAColor;
            lblModAPriority.ForeColor = Styling.ModAColor;
            lblModAPriority.Font = Styling.FontSmall;

            // Mod B accent
            lblModBSection.ForeColor = Styling.ModBColor;

            // Merge button — make it prominent
            btnMerge.Height = 48;
            btnMerge.Font = Styling.FontMedium;
        }

        // ── Settings ──────────────────────────────────────────────────────────

        private void LoadSettings()
        {
            txtGameFolder.Text  = _settings.GameFolderPath;
            txtOutputFolder.Text = _settings.OutputFolderPath;
            chkKeepModFiles.Checked = _settings.KeepModFilesAfterMerge;
            UpdateVanillaStatus();
        }

        private void SaveSettings()
        {
            _settings.GameFolderPath        = txtGameFolder.Text.Trim();
            _settings.OutputFolderPath      = txtOutputFolder.Text.Trim();
            _settings.KeepModFilesAfterMerge = chkKeepModFiles.Checked;
            _settings.Save();
        }

        // ── First Run ─────────────────────────────────────────────────────────

        private void CheckFirstRun()
        {
            if (_settings.IsFirstRun)
            {
                lblStatus.Text = "Welcome! Please select your Sekiro game folder to get started.";
                lblStatus.ForeColor = Styling.TextWarning;
                txtGameFolder.Focus();
            }
            else
            {
                InitializeLoader();
            }
        }

        private void InitializeLoader()
        {
            try
            {
                string paramdefDir = Path.Combine(AppContext.BaseDirectory, "Resources", "Paramdefs");
                _loader = new ParamLoader(paramdefDir);
                UpdateVanillaStatus();
            }
            catch (Exception ex)
            {
                SetStatus($"Error loading paramdefs: {ex.Message}", Styling.TextDanger);
            }
        }

        // ── Vanilla Status ────────────────────────────────────────────────────

        private void UpdateVanillaStatus()
        {
            if (string.IsNullOrWhiteSpace(_settings.GameFolderPath))
            {
                lblVanillaStatus.Text = "No game folder selected";
                lblVanillaStatus.ForeColor = Styling.TextSecondary;
                return;
            }

            if (!_settings.ParamFolderExists)
            {
                lblVanillaStatus.Text =
                    "⚠ Game files not unpacked! You need to run UXM Selective Unpacker first.\n" +
                    "Download from: github.com/Nordgaren/UXM-Selective-Unpack/releases\n" +
                    "Point it to sekiro.exe and click Unpack.";
                lblVanillaStatus.ForeColor = Styling.TextWarning;
            }
            else if (!_settings.VanillaFileExists)
            {
                lblVanillaStatus.Text =
                    "⚠ param\\gameparam folder found but gameparam.parambnd.dcx is missing.\n" +
                    "Please run UXM Unpack again to restore the vanilla param file.";
                lblVanillaStatus.ForeColor = Styling.TextWarning;
            }
            else
            {
                lblVanillaStatus.Text = $"✓ Vanilla file found: {_settings.VanillaParamPath}";
                lblVanillaStatus.ForeColor = Styling.TextSuccess;
            }
        }

        // ── Browse Buttons ────────────────────────────────────────────────────

        private void btnBrowseGame_Click(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select your Sekiro game folder (contains sekiro.exe)",
                UseDescriptionForTitle = true
            };

            if (!string.IsNullOrWhiteSpace(_settings.GameFolderPath)
                && Directory.Exists(_settings.GameFolderPath))
                dialog.InitialDirectory = _settings.GameFolderPath;

            if (dialog.ShowDialog() != DialogResult.OK) return;

            string folder = dialog.SelectedPath;

            // Validate: must contain sekiro.exe
            if (!File.Exists(Path.Combine(folder, "sekiro.exe")))
            {
                MessageBox.Show(
                    "The selected folder does not contain sekiro.exe.\n\n" +
                    "Please select the correct Sekiro game folder.",
                    "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            txtGameFolder.Text = folder;
            _settings.GameFolderPath = folder;

            // Set default output to mods folder if it exists
            string modsFolder = Path.Combine(folder, "mods");
            if (Directory.Exists(modsFolder) && string.IsNullOrWhiteSpace(_settings.OutputFolderPath))
            {
                txtOutputFolder.Text = modsFolder;
                _settings.OutputFolderPath = modsFolder;
            }

            SaveSettings();
            UpdateVanillaStatus();
            InitializeLoader();
            SetStatus("Game folder set. Now select your two mod files.", Styling.TextSuccess);
        }

        private void btnBrowseModA_Click(object sender, EventArgs e)
        {
            string? path = BrowseForParamFile("Select Mod A — gameparam.parambnd.dcx (HIGHER PRIORITY)");
            if (path == null) return;

            _modAPath = path;
            txtModAPath.Text = path;

            // Auto-suggest name from folder path
            if (string.IsNullOrWhiteSpace(txtModAName.Text))
            {
                string? parent = Path.GetFileName(Path.GetDirectoryName(
                    Path.GetDirectoryName(Path.GetDirectoryName(path))));
                txtModAName.Text = parent ?? "Mod A";
            }

            SetStatus("Mod A selected.", Styling.TextSuccess);
            UpdateMergeButtonState();
        }

        private void btnBrowseModB_Click(object sender, EventArgs e)
        {
            string? path = BrowseForParamFile("Select Mod B — gameparam.parambnd.dcx");
            if (path == null) return;

            _modBPath = path;
            txtModBPath.Text = path;

            if (string.IsNullOrWhiteSpace(txtModBName.Text))
            {
                string? parent = Path.GetFileName(Path.GetDirectoryName(
                    Path.GetDirectoryName(Path.GetDirectoryName(path))));
                txtModBName.Text = parent ?? "Mod B";
            }

            SetStatus("Mod B selected.", Styling.TextSuccess);
            UpdateMergeButtonState();
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select output folder (merged file goes into param\\gameparam\\ inside this folder)",
                UseDescriptionForTitle = true
            };

            if (!string.IsNullOrWhiteSpace(_settings.OutputFolderPath)
                && Directory.Exists(_settings.OutputFolderPath))
                dialog.InitialDirectory = _settings.OutputFolderPath;

            if (dialog.ShowDialog() != DialogResult.OK) return;

            txtOutputFolder.Text = dialog.SelectedPath;
            SaveSettings();
        }

        private string? BrowseForParamFile(string title)
        {
            using var dialog = new OpenFileDialog
            {
                Title = title,
                Filter = "Sekiro Param Bundle|gameparam.parambnd.dcx|All files|*.*",
                FileName = "gameparam.parambnd.dcx"
            };

            // Try to start in game mods folder
            if (!string.IsNullOrWhiteSpace(_settings.GameFolderPath))
            {
                string modsParam = Path.Combine(_settings.GameFolderPath, "mods");
                if (Directory.Exists(modsParam))
                    dialog.InitialDirectory = modsParam;
            }

            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
        }

        // ── Merge Button ──────────────────────────────────────────────────────

        private void UpdateMergeButtonState()
        {
            btnMerge.Enabled =
                !string.IsNullOrWhiteSpace(_modAPath) &&
                !string.IsNullOrWhiteSpace(_modBPath) &&
                !string.IsNullOrWhiteSpace(txtOutputFolder.Text) &&
                _settings.VanillaFileExists &&
                _loader != null;

            if (btnMerge.Enabled)
                SetStatus("Ready to merge. Click MERGE when ready.", Styling.TextSuccess);
        }

        private async void btnMerge_Click(object sender, EventArgs e)
        {
            // ── Validate all inputs ───────────────────────────────────────────
            string modAName = string.IsNullOrWhiteSpace(txtModAName.Text) ? "Mod A" : txtModAName.Text.Trim();
            string modBName = string.IsNullOrWhiteSpace(txtModBName.Text) ? "Mod B" : txtModBName.Text.Trim();

            if (!File.Exists(_modAPath))
            {
                ShowError($"Mod A file not found:\n{_modAPath}");
                return;
            }

            if (!File.Exists(_modBPath))
            {
                ShowError($"Mod B file not found:\n{_modBPath}");
                return;
            }

            if (!Directory.Exists(txtOutputFolder.Text.Trim()))
            {
                ShowError($"Output folder does not exist:\n{txtOutputFolder.Text}");
                return;
            }

            SaveSettings();

            // ── Disable UI during merge ───────────────────────────────────────
            SetFormEnabled(false);
            SetStatus("Loading param files...", Styling.TextWarning);

            try
            {
                await RunMergeAsync(modAName, modBName);
            }
            catch (Exception ex)
            {
                ShowError($"Unexpected error during merge:\n{ex.Message}");
            }
            finally
            {
                SetFormEnabled(true);
            }
        }

        private async Task RunMergeAsync(string modAName, string modBName)
        {
            string vanillaPath  = _settings.VanillaParamPath;
            string modAPath     = _modAPath;
            string modBPath     = _modBPath;
            string outputFolder = txtOutputFolder.Text.Trim();

            LoadedParamBundle vanillaBundle = null!;
            LoadedParamBundle modABundle    = null!;
            LoadedParamBundle modBBundle    = null!;
            DiffResult diffA = null!;
            DiffResult diffB = null!;
            MergeResult mergeResult = null!;

            // ── Load all three files ──────────────────────────────────────────
            await Task.Run(() =>
            {
                SetStatus("Loading vanilla file...", Styling.TextWarning);
                vanillaBundle = _loader!.LoadParamBundle(vanillaPath);

                SetStatus($"Loading {modAName}...", Styling.TextWarning);
                modABundle = _loader.LoadParamBundle(modAPath);

                SetStatus($"Loading {modBName}...", Styling.TextWarning);
                modBBundle = _loader.LoadParamBundle(modBPath);

                // ── Diff both against vanilla ─────────────────────────────────
                SetStatus("Analysing differences...", Styling.TextWarning);
                var differ = new ParamDiffer();
                diffA = differ.Diff(vanillaBundle, modABundle, modAName);
                diffB = differ.Diff(vanillaBundle, modBBundle, modBName);

                // ── Merge ─────────────────────────────────────────────────────
                SetStatus("Merging at cell level...", Styling.TextWarning);
                var merger = new ParamMerger(_loader.Paramdefs);
                mergeResult = merger.Merge(vanillaBundle, modABundle, diffA, modBBundle, diffB);
            });

            SetStatus("Merge complete. Reviewing conflicts...", Styling.TextSuccess);

            // ── Show conflict resolver if there are conflicts ─────────────────
            if (mergeResult.ResolvedConflicts.Count > 0)
            {
                using var conflictForm = new ConflictResolverForm(mergeResult, modAName, modBName);
                if (conflictForm.ShowDialog(this) != DialogResult.OK)
                {
                    SetStatus("Merge cancelled during conflict resolution.", Styling.TextSecondary);
                    return;
                }
                // Conflict resolutions are applied inside ConflictResolverForm
            }

            // ── Show post-merge form ──────────────────────────────────────────
            using var postForm = new PostMergeForm(
                mergeResult, vanillaPath, outputFolder,
                modAPath, modBPath,
                modAName, modBName,
                _loader!, _settings);

            postForm.ShowDialog(this);

            // ── Clear mod paths ───────────────────────────────────────────────
            _modAPath = string.Empty;
            _modBPath = string.Empty;
            txtModAPath.Text = string.Empty;
            txtModBPath.Text = string.Empty;
            txtModAName.Text = string.Empty;
            txtModBName.Text = string.Empty;

            UpdateMergeButtonState();
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private void SetStatus(string message, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(() => SetStatus(message, color));
                return;
            }
            lblStatus.Text = message;
            lblStatus.ForeColor = color;
        }

        private void SetFormEnabled(bool enabled)
        {
            if (InvokeRequired) { Invoke(() => SetFormEnabled(enabled)); return; }
            btnMerge.Enabled        = enabled;
            btnBrowseModA.Enabled   = enabled;
            btnBrowseModB.Enabled   = enabled;
            btnBrowseGame.Enabled   = enabled;
            btnBrowseOutput.Enabled = enabled;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetStatus("Error — see dialog.", Styling.TextDanger);
        }

        private void chkKeepModFiles_CheckedChanged(object sender, EventArgs e) => SaveSettings();
        private void txtOutputFolder_TextChanged(object sender, EventArgs e)
        {
            SaveSettings();
            UpdateMergeButtonState();
        }
    }
}
