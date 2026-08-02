namespace SekiroParamMerger.WinForms
{
    partial class PostMergeForm
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
            titleBar = new AppTitleBar { Heading = "MERGE COMPLETE" };
            titleBar.Location = new Point(0, 0);
            titleBar.Size = new Size(760, Styling.TitleBarHeight);
            titleBar.CloseClicked += (_, _) => Close();

            lblTitle = new Label
            {
                Text = "✓  MERGE COMPLETE — REVIEW & SAVE",
                Font = Styling.FontAppTitle,
                ForeColor = Styling.TextPrimary,
                AutoSize = true,
                Location = new Point(18, 62),
                BackColor = Styling.BackgroundDark
            };

            // ══ Summary card ═══════════════════════════════════════════════
            pnlSummary = new RoundedPanel { Location = new Point(18, 118), Size = new Size(724, 156) };

            lblStatA = new Label { Location = new Point(14, 10), AutoSize = true, Font = Styling.FontMedium, ForeColor = Styling.ModAColor, BackColor = Styling.BackgroundMid };
            lblStatB = new Label { Location = new Point(14, 38), AutoSize = true, Font = Styling.FontMedium, ForeColor = Styling.ModBColor, BackColor = Styling.BackgroundMid };

            lblConflicts = new Label { Location = new Point(14, 68), AutoSize = true, Font = Styling.FontNormal, ForeColor = Styling.TextPrimary, BackColor = Styling.BackgroundMid };

            lblOutputPath = new Label { Location = new Point(14, 100), Size = new Size(696, 44), Font = Styling.FontSmall, ForeColor = Styling.TextSecondary, BackColor = Styling.BackgroundMid };

            pnlSummary.Controls.AddRange(new Control[] { lblStatA, lblStatB, lblConflicts, lblOutputPath });

            // ══ Deletion card ══════════════════════════════════════════════
            pnlDelete = new RoundedPanel { Location = new Point(18, 286), Size = new Size(724, 150) };

            lblDeleteTitle = new Label
            {
                Text = "🗑  AUTO-CLEANUP OF MOD PARAM FOLDERS",
                Font = Styling.FontMedium,
                ForeColor = Styling.AccentGold,
                AutoSize = true,
                Location = new Point(14, 10),
                BackColor = Styling.BackgroundMid
            };

            lblDeleteInfo = new Label { Location = new Point(14, 36), Size = new Size(696, 80), Font = Styling.FontSmall, ForeColor = Styling.TextSecondary, BackColor = Styling.BackgroundMid };

            chkKeepFiles = new CheckBox
            {
                Text = "Keep Mod A & B param folders (do NOT delete them)",
                Location = new Point(14, 120),
                AutoSize = true,
                ForeColor = Styling.TextPrimary,
                Font = Styling.FontSmall,
                BackColor = Styling.BackgroundMid
            };
            chkKeepFiles.CheckedChanged += chkKeepFiles_CheckedChanged;

            pnlDelete.Controls.AddRange(new Control[] { lblDeleteTitle, lblDeleteInfo, chkKeepFiles });

            // ══ Save status ════════════════════════════════════════════════
            lblSaveStatus = new Label
            {
                Text = "Ready to save.",
                Location = new Point(18, 448),
                Size = new Size(724, 20),
                Font = Styling.FontSmall,
                ForeColor = Styling.TextSecondary,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Styling.BackgroundDark
            };

            // ══ Buttons ════════════════════════════════════════════════════
            btnSave = new ModernButton
            {
                BackColor = Styling.BackgroundDark,
                Text = "💾  SAVE MERGED FILE",
                Location = new Point(18, 476),
                Size = new Size(724, 46),
                BaseColor = Styling.AccentRed,
                HoverColor = Styling.AccentRedLight,
                BorderColor = Styling.AccentRed,
                TextColor = Color.White,
                Font = Styling.FontMedium,
                CornerRadius = 12
            };
            btnSave.Click += btnSave_Click;

            btnClose = new ModernButton
            {
                BackColor = Styling.BackgroundDark,
                Text = "Close",
                Location = new Point(18, 530),
                Size = new Size(724, 36),
                BaseColor = Styling.BackgroundLight,
                HoverColor = Styling.ButtonHover,
                BorderColor = Styling.BorderColor,
                TextColor = Styling.TextPrimary
            };
            btnClose.Click += btnClose_Click;

            Controls.AddRange(new Control[]
            {
                titleBar,
                lblTitle, pnlSummary, pnlDelete,
                lblSaveStatus, btnSave, btnClose
            });

            ResumeLayout(false);
        }

        // ── Control declarations ────────────────────────────────────────────
        private AppTitleBar   titleBar;
        private Label         lblTitle;
        private RoundedPanel  pnlSummary;
        private Label         lblStatA;
        private Label         lblStatB;
        private Label         lblConflicts;
        private Label         lblOutputPath;
        private RoundedPanel  pnlDelete;
        private Label         lblDeleteTitle;
        private Label         lblDeleteInfo;
        private CheckBox      chkKeepFiles;
        private Label         lblSaveStatus;
        private ModernButton  btnSave;
        private ModernButton  btnClose;
    }
}
