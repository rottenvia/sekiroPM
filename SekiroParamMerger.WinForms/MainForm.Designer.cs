namespace SekiroParamMerger.WinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // ── Title bar (sekiroPM + made by eggyeg) ───────────────────────
            titleBar = new AppTitleBar { Heading = "" };
            titleBar.Location = new Point(0, 0);
            titleBar.Size = new Size(1000, Styling.TitleBarHeight);
            titleBar.CloseClicked += (_, _) => btnCloseWindow_Click();
            titleBar.MinimizeClicked += (_, _) => MinimizeToTray();

            // ── Header (compact, replaces the old big title) ────────────────
            pnlHeader = new RoundedPanel
            {
                Location = new Point(18, 62),
                Size = new Size(964, 62),
                CornerRadius = 14,
                FillColor = Styling.BackgroundMid,
                BorderColor = Styling.BorderColor
            };

            lblHeader = MakeHeader("Merge two gameparam files", new Point(18, 8));
            lblHeader.ForeColor = Styling.TextPrimary;
            lblHeader.Font = Styling.FontAppTitle;

            lblSubtitle = MakeSecondary("Both mods keep their changes — no more overriding each other.", new Point(19, 38));
            lblSubtitle.Size = new Size(900, 16);

            pnlHeader.Controls.AddRange(new Control[] { lblHeader, lblSubtitle });

            // ══ GAME FOLDER card ═══════════════════════════════════════════
            pnlGame = new RoundedPanel { Location = new Point(18, 136), Size = new Size(964, 108) };

            lblGameSection = MakeHeader("GAME FOLDER", new Point(16, 12));
            lblGameFolderHint = MakeSecondary("Select once — the folder containing sekiro.exe. Vanilla gameparam is found automatically.", new Point(16, 36));

            txtGameFolder = new TextBox { Location = new Point(16, 58), Size = new Size(748, 30), ReadOnly = true, BackColor = Styling.BackgroundLight, ForeColor = Styling.TextPrimary, BorderStyle = BorderStyle.FixedSingle, Font = Styling.FontSmall };
            txtGameFolder.PlaceholderText = "No game folder selected";

            btnBrowseGame = new ModernButton { Text = "Browse…", Location = new Point(776, 56), Size = new Size(172, 34), BaseColor = Styling.ButtonBackground, HoverColor = Styling.ButtonHover, CornerRadius = 10 };
            btnBrowseGame.Click += btnBrowseGame_Click;

            lblVanillaStatus = MakeSecondary("", new Point(16, 88));
            lblVanillaStatus.Size = new Size(932, 16);

            pnlGame.Controls.AddRange(new Control[] { lblGameSection, lblGameFolderHint, txtGameFolder, btnBrowseGame, lblVanillaStatus });

            // ══ MOD A card ═════════════════════════════════════════════════
            pnlModA = new RoundedPanel { Location = new Point(18, 256), Size = new Size(475, 208) };

            lblModASection = MakeHeader("MOD A", new Point(16, 12));
            lblModASection.ForeColor = Styling.ModAColor;
            lblModAPriority = MakeSecondary("Higher priority — wins unresolved conflicts", new Point(16, 40));

            lblModAName = MakeSecondary("Name:", new Point(16, 66));
            txtModAName = new TextBox { Location = new Point(16, 86), Size = new Size(443, 30), PlaceholderText = "e.g. Combat Overhaul", BackColor = Styling.BackgroundLight, ForeColor = Styling.TextPrimary, BorderStyle = BorderStyle.FixedSingle, Font = Styling.FontSmall };

            lblModAFile = MakeSecondary("PARAM FILE:", new Point(16, 124));
            txtModAPath = new TextBox { Location = new Point(16, 142), Size = new Size(327, 30), ReadOnly = true, PlaceholderText = "No file selected", BackColor = Styling.BackgroundLight, ForeColor = Styling.TextPrimary, BorderStyle = BorderStyle.FixedSingle, Font = Styling.FontSmall };

            btnBrowseModA = new ModernButton { Text = "Browse…", Location = new Point(355, 140), Size = new Size(104, 34), BaseColor = Styling.ButtonBackground, HoverColor = Styling.ButtonHover, CornerRadius = 10 };
            btnBrowseModA.Click += btnBrowseModA_Click;

            lblModAPath = MakeSecondary("Select gameparam.parambnd.dcx from Mod A's folder", new Point(16, 180));
            lblModAPath.Size = new Size(443, 18);

            pnlModA.Controls.AddRange(new Control[] { lblModASection, lblModAPriority, lblModAName, txtModAName, lblModAFile, txtModAPath, btnBrowseModA, lblModAPath });

            // ══ MOD B card ═════════════════════════════════════════════════
            pnlModB = new RoundedPanel { Location = new Point(507, 256), Size = new Size(475, 208) };

            lblModBSection = MakeHeader("MOD B", new Point(16, 12));
            lblModBSection.ForeColor = Styling.ModBColor;
            lblModBPriority = MakeSecondary("Lower priority — you pick on each conflict", new Point(16, 40));

            lblModBName = MakeSecondary("Name:", new Point(16, 66));
            txtModBName = new TextBox { Location = new Point(16, 86), Size = new Size(443, 30), PlaceholderText = "e.g. Enemy Rebalance", BackColor = Styling.BackgroundLight, ForeColor = Styling.TextPrimary, BorderStyle = BorderStyle.FixedSingle, Font = Styling.FontSmall };

            lblModBFile = MakeSecondary("PARAM FILE:", new Point(16, 124));
            txtModBPath = new TextBox { Location = new Point(16, 142), Size = new Size(327, 30), ReadOnly = true, PlaceholderText = "No file selected", BackColor = Styling.BackgroundLight, ForeColor = Styling.TextPrimary, BorderStyle = BorderStyle.FixedSingle, Font = Styling.FontSmall };

            btnBrowseModB = new ModernButton { Text = "Browse…", Location = new Point(355, 140), Size = new Size(104, 34), BaseColor = Styling.ButtonBackground, HoverColor = Styling.ButtonHover, CornerRadius = 10 };
            btnBrowseModB.Click += btnBrowseModB_Click;

            lblModBPath = MakeSecondary("Select gameparam.parambnd.dcx from Mod B's folder", new Point(16, 180));
            lblModBPath.Size = new Size(443, 18);

            pnlModB.Controls.AddRange(new Control[] { lblModBSection, lblModBPriority, lblModBName, txtModBName, lblModBFile, txtModBPath, btnBrowseModB, lblModBPath });

            // ══ OUTPUT FOLDER card ═════════════════════════════════════════
            pnlOutput = new RoundedPanel { Location = new Point(18, 476), Size = new Size(964, 118) };

            lblOutputSection = MakeHeader("OUTPUT FOLDER", new Point(16, 12));
            lblOutputHint = MakeSecondary("Merged file saves to [output]\\param\\gameparam\\gameparam.parambnd.dcx — remembered between sessions", new Point(16, 36));

            txtOutputFolder = new TextBox { Location = new Point(16, 56), Size = new Size(748, 30), ReadOnly = true, PlaceholderText = "No output folder selected", BackColor = Styling.BackgroundLight, ForeColor = Styling.TextPrimary, BorderStyle = BorderStyle.FixedSingle, Font = Styling.FontSmall };
            txtOutputFolder.TextChanged += txtOutputFolder_TextChanged;

            btnBrowseOutput = new ModernButton { Text = "Browse…", Location = new Point(776, 54), Size = new Size(172, 34), BaseColor = Styling.ButtonBackground, HoverColor = Styling.ButtonHover, CornerRadius = 10 };
            btnBrowseOutput.Click += btnBrowseOutput_Click;

            chkKeepModFiles = new CheckBox
            {
                Text = "Keep Mod A & B param folders after merge (for future merges)",
                Location = new Point(16, 90),
                AutoSize = true,
                ForeColor = Styling.TextPrimary,
                Font = Styling.FontSmall,
                BackColor = Styling.BackgroundMid
            };
            chkKeepModFiles.CheckedChanged += chkKeepModFiles_CheckedChanged;

            pnlOutput.Controls.AddRange(new Control[] { lblOutputSection, lblOutputHint, txtOutputFolder, btnBrowseOutput, chkKeepModFiles });

            // ══ Progress card (hidden by default) ═══════════════════════════
            pnlProgress = new RoundedPanel { Location = new Point(18, 606), Size = new Size(964, 46), Visible = false };

            lblProgressText = MakeSecondary("", new Point(16, 6));
            lblProgressText.Size = new Size(560, 16);
            lblProgressText.ForeColor = Styling.TextWarning;

            progressBar = new DarkProgressBar { Location = new Point(16, 26), Size = new Size(716, 16) };

            lblEta = MakeSecondary("", new Point(744, 26));
            lblEta.Size = new Size(204, 16);
            lblEta.TextAlign = ContentAlignment.TopRight;

            pnlProgress.Controls.AddRange(new Control[] { lblProgressText, progressBar, lblEta });

            // ══ Status + Merge button ══════════════════════════════════════
            lblStatus = new Label
            {
                Text = "Select your game folder to begin.",
                Location = new Point(18, 660),
                Size = new Size(964, 28),
                ForeColor = Styling.TextSecondary,
                Font = Styling.FontSmall,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Styling.BackgroundDark
            };

            btnMerge = new ModernButton
            {
                Text = "⚔  MERGE MODS",
                Location = new Point(18, 696),
                Size = new Size(964, 50),
                BaseColor = Styling.AccentRed,
                HoverColor = Styling.AccentRedLight,
                BorderColor = Styling.AccentRed,
                TextColor = Color.White,
                Font = Styling.FontMedium,
                CornerRadius = 14,
                Enabled = false
            };
            btnMerge.Click += btnMerge_Click;

            Controls.AddRange(new Control[]
            {
                titleBar,
                pnlHeader,
                pnlGame, pnlModA, pnlModB, pnlOutput,
                pnlProgress,
                lblStatus, btnMerge
            });

            ResumeLayout(false);
        }

        private static Label MakeHeader(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Font = Styling.FontMedium,
                ForeColor = Styling.AccentGold,
                AutoSize = true,
                Location = location,
                BackColor = Styling.BackgroundMid
            };
        }

        private static Label MakeSecondary(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Font = Styling.FontSmall,
                ForeColor = Styling.TextSecondary,
                AutoSize = true,
                Location = location,
                BackColor = Styling.BackgroundMid
            };
        }

        // ── Control declarations ────────────────────────────────────────────
        private AppTitleBar    titleBar;
        private RoundedPanel   pnlHeader;
        private Label          lblHeader;
        private Label          lblSubtitle;
        private RoundedPanel   pnlGame;
        private Label          lblGameSection;
        private Label          lblGameFolderHint;
        private TextBox        txtGameFolder;
        private ModernButton   btnBrowseGame;
        private Label          lblVanillaStatus;
        private RoundedPanel   pnlModA;
        private Label          lblModASection;
        private Label          lblModAPriority;
        private Label          lblModAName;
        private TextBox        txtModAName;
        private Label          lblModAFile;
        private TextBox        txtModAPath;
        private ModernButton   btnBrowseModA;
        private Label          lblModAPath;
        private RoundedPanel   pnlModB;
        private Label          lblModBSection;
        private Label          lblModBPriority;
        private Label          lblModBName;
        private TextBox        txtModBName;
        private Label          lblModBFile;
        private TextBox        txtModBPath;
        private ModernButton   btnBrowseModB;
        private Label          lblModBPath;
        private RoundedPanel   pnlOutput;
        private Label          lblOutputSection;
        private Label          lblOutputHint;
        private TextBox        txtOutputFolder;
        private ModernButton   btnBrowseOutput;
        private CheckBox       chkKeepModFiles;
        private RoundedPanel   pnlProgress;
        private Label          lblProgressText;
        private DarkProgressBar progressBar;
        private Label          lblEta;
        private Label          lblStatus;
        private ModernButton   btnMerge;
    }
}
