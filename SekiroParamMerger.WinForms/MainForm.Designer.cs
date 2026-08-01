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
            // ── Title bar ───────────────────────────────────────────────────
            titleBar = new AppTitleBar { Heading = "SEKIRO PARAM MERGER" };
            titleBar.Location = new Point(0, 0);
            titleBar.Size = new Size(1000, Styling.TitleBarHeight);
            titleBar.CloseClicked += (_, _) => btnCloseWindow_Click();
            titleBar.MinimizeClicked += (_, _) => MinimizeToTray();

            // ── Header ──────────────────────────────────────────────────────
            lblHeader = new Label
            {
                Text = "SEKIRO PARAM MERGER",
                Font = Styling.FontAppTitle,
                ForeColor = Styling.TextPrimary,
                AutoSize = true,
                Location = new Point(18, 60),
                BackColor = Styling.BackgroundDark
            };

            lblSubtitle = new Label
            {
                Text = "Merge two gameparam files at cell level — both mods keep their changes.",
                Font = Styling.FontAppSub,
                ForeColor = Styling.TextSecondary,
                AutoSize = true,
                Location = new Point(20, 94),
                BackColor = Styling.BackgroundDark
            };

            // ══ GAME FOLDER card ═══════════════════════════════════════════
            pnlGame = new RoundedPanel { Location = new Point(18, 124), Size = new Size(964, 108) };

            lblGameSection = MakeHeader("GAME FOLDER", new Point(14, 10));
            lblGameFolderHint = MakeSecondary("Select once — the folder containing sekiro.exe. Vanilla gameparam is found automatically.",
                new Point(14, 34));

            txtGameFolder = new TextBox { Location = new Point(14, 56), Size = new Size(760, 30), ReadOnly = true, BackColor = Styling.BackgroundMid, ForeColor = Styling.TextPrimary, BorderStyle = BorderStyle.FixedSingle, Font = Styling.FontSmall };
            txtGameFolder.PlaceholderText = "No game folder selected";

            btnBrowseGame = new ModernButton { Text = "Browse…", Location = new Point(786, 54), Size = new Size(164, 34), BaseColor = Styling.BackgroundLight, HoverColor = Styling.ButtonHover };
            btnBrowseGame.Click += btnBrowseGame_Click;

            lblVanillaStatus = MakeSecondary("", new Point(14, 86));
            lblVanillaStatus.Size = new Size(936, 16);

            pnlGame.Controls.AddRange(new Control[] { lblGameSection, lblGameFolderHint, txtGameFolder, btnBrowseGame, lblVanillaStatus });

            // ══ MOD A card ═════════════════════════════════════════════════
            pnlModA = new RoundedPanel { Location = new Point(18, 244), Size = new Size(475, 208) };

            lblModASection = MakeHeader("MOD A", new Point(14, 10));
            lblModASection.ForeColor = Styling.ModAColor;
            lblModAPriority = MakeSecondary("Higher priority — wins unresolved conflicts", new Point(14, 38));

            lblModAName = MakeSecondary("Name:", new Point(14, 64));
            txtModAName = new TextBox { Location = new Point(14, 84), Size = new Size(447, 30), PlaceholderText = "e.g. Combat Overhaul", BackColor = Styling.BackgroundMid, ForeColor = Styling.TextPrimary, BorderStyle = BorderStyle.FixedSingle, Font = Styling.FontSmall };

            lblModAFile = MakeSecondary("PARAM FILE:", new Point(14, 122));
            txtModAPath = new TextBox { Location = new Point(14, 140), Size = new Size(327, 30), ReadOnly = true, PlaceholderText = "No file selected", BackColor = Styling.BackgroundMid, ForeColor = Styling.TextPrimary, BorderStyle = BorderStyle.FixedSingle, Font = Styling.FontSmall };

            btnBrowseModA = new ModernButton { Text = "Browse…", Location = new Point(353, 138), Size = new Size(108, 34), BaseColor = Styling.BackgroundLight, HoverColor = Styling.ButtonHover };
            btnBrowseModA.Click += btnBrowseModA_Click;

            lblModAPath = MakeSecondary("Select gameparam.parambnd.dcx from Mod A's folder", new Point(14, 176));
            lblModAPath.Size = new Size(447, 18);

            pnlModA.Controls.AddRange(new Control[] { lblModASection, lblModAPriority, lblModAName, txtModAName, lblModAFile, txtModAPath, btnBrowseModA, lblModAPath });

            // ══ MOD B card ═════════════════════════════════════════════════
            pnlModB = new RoundedPanel { Location = new Point(507, 244), Size = new Size(475, 208) };

            lblModBSection = MakeHeader("MOD B", new Point(14, 10));
            lblModBSection.ForeColor = Styling.ModBColor;
            lblModBPriority = MakeSecondary("Lower priority — you pick on each conflict", new Point(14, 38));

            lblModBName = MakeSecondary("Name:", new Point(14, 64));
            txtModBName = new TextBox { Location = new Point(14, 84), Size = new Size(447, 30), PlaceholderText = "e.g. Enemy Rebalance", BackColor = Styling.BackgroundMid, ForeColor = Styling.TextPrimary, BorderStyle = BorderStyle.FixedSingle, Font = Styling.FontSmall };

            lblModBFile = MakeSecondary("PARAM FILE:", new Point(14, 122));
            txtModBPath = new TextBox { Location = new Point(14, 140), Size = new Size(327, 30), ReadOnly = true, PlaceholderText = "No file selected", BackColor = Styling.BackgroundMid, ForeColor = Styling.TextPrimary, BorderStyle = BorderStyle.FixedSingle, Font = Styling.FontSmall };

            btnBrowseModB = new ModernButton { Text = "Browse…", Location = new Point(353, 138), Size = new Size(108, 34), BaseColor = Styling.BackgroundLight, HoverColor = Styling.ButtonHover };
            btnBrowseModB.Click += btnBrowseModB_Click;

            lblModBPath = MakeSecondary("Select gameparam.parambnd.dcx from Mod B's folder", new Point(14, 176));
            lblModBPath.Size = new Size(447, 18);

            pnlModB.Controls.AddRange(new Control[] { lblModBSection, lblModBPriority, lblModBName, txtModBName, lblModBFile, txtModBPath, btnBrowseModB, lblModBPath });

            // ══ OUTPUT FOLDER card ═════════════════════════════════════════
            pnlOutput = new RoundedPanel { Location = new Point(18, 464), Size = new Size(964, 124) };

            lblOutputSection = MakeHeader("OUTPUT FOLDER", new Point(14, 10));
            lblOutputHint = MakeSecondary("Merged file saves to [output]\\param\\gameparam\\gameparam.parambnd.dcx — remembered between sessions", new Point(14, 34));

            txtOutputFolder = new TextBox { Location = new Point(14, 54), Size = new Size(760, 30), ReadOnly = true, PlaceholderText = "No output folder selected", BackColor = Styling.BackgroundMid, ForeColor = Styling.TextPrimary, BorderStyle = BorderStyle.FixedSingle, Font = Styling.FontSmall };
            txtOutputFolder.TextChanged += txtOutputFolder_TextChanged;

            btnBrowseOutput = new ModernButton { Text = "Browse…", Location = new Point(786, 52), Size = new Size(164, 34), BaseColor = Styling.BackgroundLight, HoverColor = Styling.ButtonHover };
            btnBrowseOutput.Click += btnBrowseOutput_Click;

            chkKeepModFiles = new CheckBox
            {
                Text = "Keep Mod A & B param folders after merge (for future merges)",
                Location = new Point(14, 90),
                AutoSize = true,
                ForeColor = Styling.TextPrimary,
                Font = Styling.FontSmall,
                BackColor = Styling.BackgroundMid
            };
            chkKeepModFiles.CheckedChanged += chkKeepModFiles_CheckedChanged;

            pnlOutput.Controls.AddRange(new Control[] { lblOutputSection, lblOutputHint, txtOutputFolder, btnBrowseOutput, chkKeepModFiles });

            // ══ Progress card (hidden by default) ═══════════════════════════
            pnlProgress = new RoundedPanel { Location = new Point(18, 600), Size = new Size(964, 44), Visible = false };

            lblProgressText = MakeSecondary("", new Point(14, 4));
            lblProgressText.Size = new Size(560, 16);
            lblProgressText.ForeColor = Styling.TextWarning;

            progressBar = new DarkProgressBar { Location = new Point(14, 24), Size = new Size(720, 16) };

            lblEta = MakeSecondary("", new Point(742, 24));
            lblEta.Size = new Size(208, 16);
            lblEta.TextAlign = ContentAlignment.TopRight;

            pnlProgress.Controls.AddRange(new Control[] { lblProgressText, progressBar, lblEta });

            // ══ Status + Merge button ══════════════════════════════════════
            lblStatus = new Label
            {
                Text = "Select your game folder to begin.",
                Location = new Point(18, 654),
                Size = new Size(964, 28),
                ForeColor = Styling.TextSecondary,
                Font = Styling.FontSmall,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Styling.BackgroundDark
            };

            btnMerge = new ModernButton
            {
                Text = "⚔  MERGE MODS",
                Location = new Point(18, 690),
                Size = new Size(964, 50),
                BaseColor = Styling.AccentRed,
                HoverColor = Styling.AccentRedLight,
                BorderColor = Styling.AccentRed,
                TextColor = Color.White,
                Font = Styling.FontMedium,
                CornerRadius = 12,
                Enabled = false
            };
            btnMerge.Click += btnMerge_Click;

            Controls.AddRange(new Control[]
            {
                titleBar,
                lblHeader, lblSubtitle,
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
