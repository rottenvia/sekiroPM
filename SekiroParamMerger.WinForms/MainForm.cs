using System.Diagnostics;
using SekiroParamMerger.Core;
using SekiroParamMerger.Core.Models;

namespace SekiroParamMerger.WinForms
{
    public partial class MainForm : Form
    {
        private readonly AppSettings _settings;
        private ParamLoader? _loader;

        // ── State ─────────────────────────────────────────────────────────────
        private string _modAPath = string.Empty;
        private string _modAName = string.Empty;
        private string _modBPath = string.Empty;
        private string _modBName = string.Empty;

        // ── Tray ──────────────────────────────────────────────────────────────
        private NotifyIcon? _trayIcon;
        private bool _exiting;

        // ── Progress ──────────────────────────────────────────────────────────
        private readonly Stopwatch _stopwatch = new();
        private int _totalSteps = 5;

        public MainForm()
        {
            Program.Log("MainForm: constructor start");
            InitializeComponent();
            Program.Log("MainForm: InitializeComponent done");
            _settings = AppSettings.Load();
            ApplyStyling();
            LoadSettings();
            CheckFirstRun();
            ShowOodleWarning();
            Program.Log("MainForm: constructor end");
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            Program.Log("MainForm: OnHandleCreated");
            base.OnHandleCreated(e);
            try
            {
                SetupTray();
            }
            catch
            {
                // A tray failure must never prevent the window from showing.
                _trayIcon = null;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            Program.Log($"MainForm: OnShown (Visible={Visible})");
            base.OnShown(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Program.Log($"MainForm: OnFormClosing (reason={e.CloseReason}, exiting={_exiting})");
            if (!_exiting)
            {
                // The custom close button always exits; any other path (Alt+F4)
                // minimizes to tray instead to avoid losing work.
                e.Cancel = true;
                MinimizeToTray();
                return;
            }
            _trayIcon?.Dispose();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// If the oo2core DLL is missing, surface it clearly in the status bar so
        /// the window always opens and the user always knows why merging is blocked.
        /// </summary>
        private void ShowOodleWarning()
        {
            if (Program.OodleReady) return;
            SetStatus(
                "⚠ " + (string.IsNullOrWhiteSpace(Program.OodleMessage)
                    ? "oo2core_6_win64.dll is missing."
                    : Program.OodleMessage),
                Styling.TextWarning);
        }

        // ── Styling ───────────────────────────────────────────────────────────

        private void ApplyStyling()
        {
            Styling.ApplyDarkTheme(this);
            this.Text = "Sekiro Param Merger";
            this.Icon = AppIcon.Create();
            this.ClientSize = new Size(1000, 760);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Styling.StyleTextBox(txtGameFolder);
            Styling.StyleTextBox(txtModAPath);
            Styling.StyleTextBox(txtModBPath);
            Styling.StyleTextBox(txtOutputFolder);

            this.Shown += (_, _) => Styling.MakeWindowRounded(this, 20);
        }

        // ── Settings ──────────────────────────────────────────────────────────

        private void LoadSettings()
        {
            txtGameFolder.Text   = _settings.GameFolderPath;
            txtOutputFolder.Text = _settings.OutputFolderPath;
            chkKeepModFiles.Checked = _settings.KeepModFilesAfterMerge;
            UpdateVanillaStatus();
        }

        private void SaveSettings()
        {
            _settings.GameFolderPath         = txtGameFolder.Text.Trim();
            _settings.OutputFolderPath       = txtOutputFolder.Text.Trim();
            _settings.KeepModFilesAfterMerge = chkKeepModFiles.Checked;
            _settings.Save();
        }

        // ── First Run ─────────────────────────────────────────────────────────

        private void CheckFirstRun()
        {
            if (_settings.IsFirstRun)
            {
                SetStatus("Welcome! Select your Sekiro game folder to get started.", Styling.TextWarning);
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
                SetVanillaStatus("No game folder selected", Styling.TextSecondary);
                return;
            }

            if (!_settings.ParamFolderExists)
            {
                SetVanillaStatus(
                    "⚠ Game files not unpacked — run UXM Selective Unpacker first.\n" +
                    "   github.com/Nordgaren/UXM-Selective-Unpack/releases  →  Point at sekiro.exe → Unpack.",
                    Styling.TextWarning);
            }
            else if (!_settings.VanillaFileExists)
            {
                SetVanillaStatus(
                    "⚠ param\\gameparam found but gameparam.parambnd.dcx is missing — run UXM Unpack again.",
                    Styling.TextWarning);
            }
            else
            {
                SetVanillaStatus($"✓ Vanilla file found: {_settings.VanillaParamPath}", Styling.TextSuccess);
            }
        }

        private void SetVanillaStatus(string text, Color color)
        {
            lblVanillaStatus.Text = text;
            lblVanillaStatus.ForeColor = color;
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

            if (!File.Exists(Path.Combine(folder, "sekiro.exe")))
            {
                MessageBox.Show(
                    "The selected folder does not contain sekiro.exe.\n\nPlease select the correct Sekiro game folder.",
                    "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            txtGameFolder.Text = folder;
            _settings.GameFolderPath = folder;

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
            lblModAName.Text = SuggestModName(path) ?? "Mod A";

            SetStatus("Mod A selected.", Styling.TextSuccess);
            UpdateMergeButtonState();
        }

        private void btnBrowseModB_Click(object sender, EventArgs e)
        {
            string? path = BrowseForParamFile("Select Mod B — gameparam.parambnd.dcx");
            if (path == null) return;

            _modBPath = path;
            txtModBPath.Text = path;
            lblModBName.Text = SuggestModName(path) ?? "Mod B";

            SetStatus("Mod B selected.", Styling.TextSuccess);
            UpdateMergeButtonState();
        }

        private static string? SuggestModName(string path)
        {
            try
            {
                // ...\ModName\param\gameparam\gameparam.parambnd.dcx  →  ModName
                return Path.GetFileName(Path.GetDirectoryName(
                    Path.GetDirectoryName(Path.GetDirectoryName(path))));
            }
            catch { return null; }
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
                _loader != null &&
                Program.OodleReady;

            if (btnMerge.Enabled)
                SetStatus("Ready to merge. Click MERGE when ready.", Styling.TextSuccess);
        }

        private async void btnMerge_Click(object sender, EventArgs e)
        {
            string modAName = string.IsNullOrWhiteSpace(lblModAName.Text) || lblModAName.Text == "—" ? "Mod A" : lblModAName.Text.Trim();
            string modBName = string.IsNullOrWhiteSpace(lblModBName.Text) || lblModBName.Text == "—" ? "Mod B" : lblModBName.Text.Trim();

            if (!File.Exists(_modAPath)) { ShowError($"Mod A file not found:\n{_modAPath}"); return; }
            if (!File.Exists(_modBPath)) { ShowError($"Mod B file not found:\n{_modBPath}"); return; }
            if (!Directory.Exists(txtOutputFolder.Text.Trim())) { ShowError($"Output folder does not exist:\n{txtOutputFolder.Text}"); return; }

            SaveSettings();

            SetFormEnabled(false);
            SetLoading(true);

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
                SetLoading(false);
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

            int step = 0;
            _stopwatch.Restart();
            _totalSteps = 5;

            await Task.Run(() =>
            {
                UpdateProgress(step++, _totalSteps, "Loading vanilla file…");

                vanillaBundle = _loader!.LoadParamBundle(vanillaPath);

                UpdateProgress(step++, _totalSteps, $"Loading {modAName}…");
                modABundle = _loader.LoadParamBundle(modAPath);

                UpdateProgress(step++, _totalSteps, $"Loading {modBName}…");
                modBBundle = _loader.LoadParamBundle(modBPath);

                UpdateProgress(step++, _totalSteps, "Analysing differences…");
                var differ = new ParamDiffer();
                diffA = differ.Diff(vanillaBundle, modABundle, modAName);
                diffB = differ.Diff(vanillaBundle, modBBundle, modBName);

                UpdateProgress(step++, _totalSteps, "Merging at cell level…");
                var merger = new ParamMerger(_loader.Paramdefs);
                mergeResult = merger.Merge(vanillaBundle, modABundle, diffA, modBBundle, diffB);
            });

            UpdateProgress(_totalSteps, _totalSteps, "Merge complete — reviewing conflicts…", final: true);

            if (mergeResult.ResolvedConflicts.Count > 0)
            {
                using var conflictForm = new ConflictResolverForm(mergeResult, modAName, modBName);
                if (conflictForm.ShowDialog(this) != DialogResult.OK)
                {
                    SetStatus("Merge cancelled during conflict resolution.", Styling.TextSecondary);
                    return;
                }
            }

            using var postForm = new PostMergeForm(
                mergeResult, vanillaPath, outputFolder,
                modAPath, modBPath,
                modAName, modBName,
                _loader!, _settings);

            postForm.ShowDialog(this);

            _modAPath = string.Empty;
            _modBPath = string.Empty;
            txtModAPath.Text = string.Empty;
            txtModBPath.Text = string.Empty;
            lblModAName.Text = "—";
            lblModBName.Text = "—";

            UpdateMergeButtonState();
        }

        // ── Loading / Progress ────────────────────────────────────────────────

        private void SetLoading(bool loading)
        {
            if (InvokeRequired) { Invoke(() => SetLoading(loading)); return; }

            pnlProgress.Visible = loading;
            if (loading)
            {
                progressBar.IsIndeterminate = false;
                progressBar.SetValue(0);
                lblProgressText.Text = "Preparing…";
                lblEta.Text = "";
                _stopwatch.Reset();
            }
            else
            {
                progressBar.IsIndeterminate = false;
                _stopwatch.Stop();
            }
        }

        private void UpdateProgress(int stepsDone, int totalSteps, string message, bool final = false)
        {
            if (InvokeRequired) { Invoke(() => UpdateProgress(stepsDone, totalSteps, message, final)); return; }

            double fraction = totalSteps <= 0 ? 1.0 : (double)stepsDone / totalSteps;
            int percent = (int)Math.Round(fraction * 100);

            progressBar.IsIndeterminate = false;
            progressBar.SetValue(percent);
            lblProgressText.Text = message;

            if (final)
            {
                lblEta.Text = "Done";
                lblProgressText.ForeColor = Styling.TextSuccess;
            }
            else
            {
                // ETA estimate based on elapsed time vs fraction done
                long elapsedMs = Math.Max(1, _stopwatch.ElapsedMilliseconds);
                double etaMs = fraction > 0.001
                    ? (elapsedMs / fraction) - elapsedMs
                    : 0;
                lblEta.Text = etaMs > 0 ? $"ETA {FormatEta(etaMs)}" : "…";
            }
        }

        private static string FormatEta(double ms)
        {
            var ts = TimeSpan.FromMilliseconds(ms);
            if (ts.TotalSeconds < 60) return $"{Math.Max(0, (int)ts.TotalSeconds)}s";
            return $"{(int)ts.TotalMinutes}m {Math.Max(0, ts.Seconds)}s";
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private void SetStatus(string message, Color color)
        {
            if (InvokeRequired) { Invoke(() => SetStatus(message, color)); return; }
            lblStatus.Text = message;
            lblStatus.ForeColor = color;
        }

        private void SetFormEnabled(bool enabled)
        {
            if (InvokeRequired) { Invoke(() => SetFormEnabled(enabled)); return; }
            btnMerge.Enabled        = enabled && _loader != null && _settings.VanillaFileExists
                                      && !string.IsNullOrWhiteSpace(_modAPath)
                                      && !string.IsNullOrWhiteSpace(_modBPath)
                                      && !string.IsNullOrWhiteSpace(txtOutputFolder.Text)
                                      && Program.OodleReady;
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

        // ── Tray (minimize to hidden icons, CTk-style) ────────────────────────

        private void SetupTray()
        {
            _trayIcon = new NotifyIcon
            {
                Icon = AppIcon.Create(),
                Text = "Sekiro Param Merger",
                Visible = true
            };

            var menu = new ContextMenuStrip();
            menu.BackColor = Styling.BackgroundMid;
            menu.ForeColor = Styling.TextPrimary;
            menu.Renderer = new ToolStripProfessionalRenderer(new TrayColorTable());

            menu.Items.Add("Show", null, (_, _) => ShowFromTray());
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Exit", null, (_, _) => ExitApplication());

            _trayIcon.ContextMenuStrip = menu;
            _trayIcon.DoubleClick += (_, _) => ShowFromTray();
        }

        private void MinimizeToTray()
        {
            Hide();
            ShowFromTrayIfFirstTime();
        }

        private bool _trayBalloonShown;
        private void ShowFromTrayIfFirstTime()
        {
            if (_trayBalloonShown) return;
            _trayBalloonShown = true;
            _trayIcon?.ShowBalloonTip(2000, "Sekiro Param Merger",
                "Still running — double-click the tray icon to reopen.", ToolTipIcon.Info);
        }

        private void ShowFromTray()
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void ExitApplication()
        {
            _exiting = true;
            _trayIcon?.Dispose();
            _trayIcon = null;
            Close();
        }

        private void btnCloseWindow_Click()
        {
            // CTk-style: closing the window exits the app (no maximize button).
            ExitApplication();
        }

        private void chkKeepModFiles_CheckedChanged(object sender, EventArgs e) => SaveSettings();

        private void txtOutputFolder_TextChanged(object sender, EventArgs e)
        {
            SaveSettings();
            UpdateMergeButtonState();
        }
    }

    /// <summary>Colour table so the tray context menu matches the dark theme.</summary>
    internal sealed class TrayColorTable : ProfessionalColorTable
    {
        public override Color ToolStripDropDownBackground => Styling.BackgroundMid;
        public override Color MenuBorder => Styling.BorderColor;
        public override Color MenuItemBorder => Styling.BorderColor;
        public override Color MenuItemSelected => Styling.BackgroundLight;
        public override Color ImageMarginGradientBegin => Styling.BackgroundMid;
        public override Color ImageMarginGradientMiddle => Styling.BackgroundMid;
        public override Color ImageMarginGradientEnd => Styling.BackgroundMid;
        public override Color SeparatorDark => Styling.BorderColor;
        public override Color SeparatorLight => Styling.BackgroundMid;
    }
}
