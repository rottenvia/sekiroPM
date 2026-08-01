namespace SekiroParamMerger.WinForms
{
    partial class ConflictResolverForm
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
            titleBar = new AppTitleBar { Heading = "CONFLICT RESOLVER" };
            titleBar.Location = new Point(0, 0);
            titleBar.Size = new Size(820, Styling.TitleBarHeight);
            titleBar.CloseClicked += (_, _) => Close();

            lblProgress = new Label { Location = new Point(18, 62), AutoSize = true, ForeColor = Styling.TextSecondary, Font = Styling.FontMedium, BackColor = Styling.BackgroundDark };
            lblSummary  = new Label { Location = new Point(18, 88), AutoSize = true, ForeColor = Styling.TextSecondary, Font = Styling.FontSmall, BackColor = Styling.BackgroundDark };

            // ══ Info card ══════════════════════════════════════════════════
            pnlInfo = new RoundedPanel { Location = new Point(18, 114), Size = new Size(784, 140) };

            lblCategory = MakeHeader("", new Point(14, 10));
            lblFieldLabel = MakeHeader("", new Point(14, 42));
            lblFieldLabel.ForeColor = Styling.TextPrimary;
            lblFieldLabel.Font = Styling.FontNormal;

            lblTechnical = MakeSecondary("", new Point(14, 70));
            lblTechnical.Size = new Size(756, 18);
            lblTechnical.Font = Styling.FontMono;

            lblPaddingWarning = MakeSecondary("", new Point(14, 92));
            lblPaddingWarning.Size = new Size(756, 40);
            lblPaddingWarning.ForeColor = Styling.TextWarning;

            pnlInfo.Controls.AddRange(new Control[] { lblCategory, lblFieldLabel, lblTechnical, lblPaddingWarning });

            // ══ Values card ════════════════════════════════════════════════
            pnlValues = new RoundedPanel { Location = new Point(18, 266), Size = new Size(784, 172) };

            lblVanillaValue = MakeSecondary("", new Point(14, 10));

            lblModAValue = MakeHeader("", new Point(14, 42));
            lblModAValue.ForeColor = Styling.ModAColor;
            lblModAChange = new Label { Location = new Point(14, 66), AutoSize = true, ForeColor = Styling.ModAColor, Font = Styling.FontNormal, BackColor = Styling.BackgroundMid };

            lblModBValue = MakeHeader("", new Point(14, 100));
            lblModBValue.ForeColor = Styling.ModBColor;
            lblModBChange = new Label { Location = new Point(14, 124), AutoSize = true, ForeColor = Styling.ModBColor, Font = Styling.FontNormal, BackColor = Styling.BackgroundMid };

            lblCurrentChoice = MakeSecondary("", new Point(14, 150));
            lblCurrentChoice.Font = Styling.FontSmall;

            pnlValues.Controls.AddRange(new Control[] { lblVanillaValue, lblModAValue, lblModAChange, lblModBValue, lblModBChange, lblCurrentChoice });

            // ══ Choice buttons ═════════════════════════════════════════════
            btnUseModA = new ModernButton
            {
                Location = new Point(18, 450),
                Size = new Size(384, 42),
                BaseColor = Styling.ModAColor,
                HoverColor = Color.FromArgb(90, 165, 245),
                BorderColor = Styling.ModAColor,
                TextColor = Color.White,
                Font = Styling.FontMedium,
                CornerRadius = 10
            };
            btnUseModA.Click += btnUseModA_Click;

            btnUseModB = new ModernButton
            {
                Location = new Point(418, 450),
                Size = new Size(384, 42),
                BaseColor = Styling.ModBColor,
                HoverColor = Color.FromArgb(110, 220, 155),
                BorderColor = Styling.ModBColor,
                TextColor = Color.White,
                Font = Styling.FontMedium,
                CornerRadius = 10
            };
            btnUseModB.Click += btnUseModB_Click;

            // ══ Nav panel ══════════════════════════════════════════════════
            pnlNav = new RoundedPanel { Location = new Point(18, 504), Size = new Size(784, 50) };

            btnPrevious = MakeNavButton("◀ Previous", new Point(14, 10), 110);
            btnPrevious.Click += btnPrevious_Click;

            btnSkip = MakeNavButton("Skip", new Point(132, 10), 76);
            btnSkip.Click += btnSkip_Click;

            btnNext = MakeNavButton("Next ▶", new Point(216, 10), 110);
            btnNext.Click += btnNext_Click;

            btnAllModA = new ModernButton
            {
                Text = "Mod A wins ALL remaining",
                Location = new Point(342, 10),
                Size = new Size(214, 30),
                BaseColor = Color.FromArgb(50, 80, 120),
                HoverColor = Styling.ModAColor,
                BorderColor = Styling.ModAColor,
                TextColor = Styling.TextPrimary,
                Font = Styling.FontSmall
            };
            btnAllModA.Click += btnAllModA_Click;

            btnAllModB = new ModernButton
            {
                Text = "Mod B wins ALL remaining",
                Location = new Point(564, 10),
                Size = new Size(206, 30),
                BaseColor = Color.FromArgb(50, 100, 80),
                HoverColor = Styling.ModBColor,
                BorderColor = Styling.ModBColor,
                TextColor = Styling.TextPrimary,
                Font = Styling.FontSmall
            };
            btnAllModB.Click += btnAllModB_Click;

            pnlNav.Controls.AddRange(new Control[] { btnPrevious, btnSkip, btnNext, btnAllModA, btnAllModB });

            // ══ Done button ════════════════════════════════════════════════
            btnDone = new ModernButton
            {
                Text = "✓  DONE — APPLY ALL RESOLUTIONS",
                Location = new Point(18, 566),
                Size = new Size(784, 44),
                BaseColor = Styling.AccentRed,
                HoverColor = Styling.AccentRedLight,
                BorderColor = Styling.AccentRed,
                TextColor = Color.White,
                Font = Styling.FontMedium,
                CornerRadius = 12
            };
            btnDone.Click += btnDone_Click;

            Controls.AddRange(new Control[]
            {
                titleBar,
                lblProgress, lblSummary,
                pnlInfo, pnlValues,
                btnUseModA, btnUseModB,
                pnlNav, btnDone
            });

            ResumeLayout(false);
        }

        private static Label MakeHeader(string text, Point location)
            => new Label { Text = text, Font = Styling.FontMedium, ForeColor = Styling.AccentGold, AutoSize = true, Location = location, BackColor = Styling.BackgroundMid };

        private static Label MakeSecondary(string text, Point location)
            => new Label { Text = text, Font = Styling.FontSmall, ForeColor = Styling.TextSecondary, AutoSize = true, Location = location, BackColor = Styling.BackgroundMid };

        private static ModernButton MakeNavButton(string text, Point location, int width)
            => new ModernButton
            {
                Text = text,
                Location = location,
                Size = new Size(width, 30),
                BaseColor = Styling.BackgroundLight,
                HoverColor = Styling.ButtonHover,
                BorderColor = Styling.BorderColor,
                TextColor = Styling.TextPrimary,
                Font = Styling.FontSmall
            };

        // ── Control declarations ────────────────────────────────────────────
        private AppTitleBar   titleBar;
        private Label         lblProgress;
        private Label         lblSummary;
        private RoundedPanel  pnlInfo;
        private Label         lblCategory;
        private Label         lblFieldLabel;
        private Label         lblTechnical;
        private Label         lblPaddingWarning;
        private RoundedPanel  pnlValues;
        private Label         lblVanillaValue;
        private Label         lblModAValue;
        private Label         lblModAChange;
        private Label         lblModBValue;
        private Label         lblModBChange;
        private Label         lblCurrentChoice;
        private ModernButton  btnUseModA;
        private ModernButton  btnUseModB;
        private RoundedPanel  pnlNav;
        private ModernButton  btnPrevious;
        private ModernButton  btnSkip;
        private ModernButton  btnNext;
        private ModernButton  btnAllModA;
        private ModernButton  btnAllModB;
        private ModernButton  btnDone;
    }
}
