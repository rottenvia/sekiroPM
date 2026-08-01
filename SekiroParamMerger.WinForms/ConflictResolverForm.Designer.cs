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
            lblConflictTitle   = new Label();
            lblProgress        = new Label();
            lblSummary         = new Label();
            pnlInfo            = new Panel();
            lblCategory        = new Label();
            lblFieldLabel      = new Label();
            lblTechnical       = new Label();
            lblPaddingWarning  = new Label();
            pnlValues          = new Panel();
            lblVanillaValue    = new Label();
            lblModAValue       = new Label();
            lblModAChange      = new Label();
            lblModBValue       = new Label();
            lblModBChange      = new Label();
            lblCurrentChoice   = new Label();
            btnUseModA         = new Button();
            btnUseModB         = new Button();
            pnlNav             = new Panel();
            btnPrevious        = new Button();
            btnSkip            = new Button();
            btnNext            = new Button();
            btnAllModA         = new Button();
            btnAllModB         = new Button();
            btnDone            = new Button();

            SuspendLayout();

            // ── Title row ─────────────────────────────────────────────────────
            lblConflictTitle.Text      = "⚔  CONFLICT RESOLVER";
            lblConflictTitle.Location  = new Point(16, 14);
            lblConflictTitle.AutoSize  = true;

            lblProgress.Text     = "";
            lblProgress.Location = new Point(16, 44);
            lblProgress.AutoSize = true;

            lblSummary.Text     = "";
            lblSummary.Location = new Point(16, 62);
            lblSummary.AutoSize = true;

            // ── Info Panel ────────────────────────────────────────────────────
            pnlInfo.Location = new Point(12, 88);
            pnlInfo.Size     = new Size(660, 120);
            pnlInfo.Padding  = new Padding(12);

            lblCategory.Text     = "";
            lblCategory.Location = new Point(12, 10);
            lblCategory.AutoSize = true;

            lblFieldLabel.Text     = "";
            lblFieldLabel.Location = new Point(12, 36);
            lblFieldLabel.AutoSize = true;
            lblFieldLabel.ForeColor = Styling.TextPrimary;

            lblTechnical.Text     = "";
            lblTechnical.Location = new Point(12, 62);
            lblTechnical.AutoSize = true;
            lblTechnical.Font = Styling.FontSmall;

            lblPaddingWarning.Text     = "";
            lblPaddingWarning.Location = new Point(12, 82);
            lblPaddingWarning.Size     = new Size(636, 30);
            lblPaddingWarning.ForeColor = Styling.TextWarning;
            lblPaddingWarning.Font = Styling.FontSmall;

            pnlInfo.Controls.AddRange(new Control[]
            { lblCategory, lblFieldLabel, lblTechnical, lblPaddingWarning });

            // ── Values Panel ──────────────────────────────────────────────────
            pnlValues.Location = new Point(12, 218);
            pnlValues.Size     = new Size(660, 160);
            pnlValues.Padding  = new Padding(12);

            lblVanillaValue.Text     = "";
            lblVanillaValue.Location = new Point(12, 10);
            lblVanillaValue.AutoSize = true;
            lblVanillaValue.ForeColor = Styling.TextSecondary;

            lblModAValue.Text     = "";
            lblModAValue.Location = new Point(12, 40);
            lblModAValue.AutoSize = true;
            lblModAValue.ForeColor = Styling.ModAColor;
            lblModAValue.Font = Styling.FontMedium;

            lblModAChange.Text     = "";
            lblModAChange.Location = new Point(12, 62);
            lblModAChange.AutoSize = true;

            lblModBValue.Text     = "";
            lblModBValue.Location = new Point(12, 92);
            lblModBValue.AutoSize = true;
            lblModBValue.ForeColor = Styling.ModBColor;
            lblModBValue.Font = Styling.FontMedium;

            lblModBChange.Text     = "";
            lblModBChange.Location = new Point(12, 114);
            lblModBChange.AutoSize = true;

            lblCurrentChoice.Text     = "";
            lblCurrentChoice.Location = new Point(12, 140);
            lblCurrentChoice.AutoSize = true;
            lblCurrentChoice.Font = Styling.FontSmall;

            pnlValues.Controls.AddRange(new Control[]
            {
                lblVanillaValue,
                lblModAValue, lblModAChange,
                lblModBValue, lblModBChange,
                lblCurrentChoice
            });

            // ── Choice Buttons ────────────────────────────────────────────────
            btnUseModA.Text     = "";
            btnUseModA.Location = new Point(12, 390);
            btnUseModA.Size     = new Size(318, 36);
            btnUseModA.Click   += btnUseModA_Click;

            btnUseModB.Text     = "";
            btnUseModB.Location = new Point(342, 390);
            btnUseModB.Size     = new Size(318, 36);
            btnUseModB.Click   += btnUseModB_Click;

            // ── Nav Panel ─────────────────────────────────────────────────────
            pnlNav.Location = new Point(12, 438);
            pnlNav.Size     = new Size(660, 44);

            btnPrevious.Text     = "◀ Previous";
            btnPrevious.Location = new Point(0, 8);
            btnPrevious.Size     = new Size(100, 28);
            btnPrevious.Click   += btnPrevious_Click;

            btnSkip.Text     = "Skip";
            btnSkip.Location = new Point(108, 8);
            btnSkip.Size     = new Size(70, 28);
            btnSkip.Click   += btnSkip_Click;

            btnNext.Text     = "Next ▶";
            btnNext.Location = new Point(186, 8);
            btnNext.Size     = new Size(100, 28);
            btnNext.Click   += btnNext_Click;

            btnAllModA.Text     = "Mod A wins ALL remaining";
            btnAllModA.Location = new Point(310, 8);
            btnAllModA.Size     = new Size(170, 28);
            btnAllModA.Click   += btnAllModA_Click;

            btnAllModB.Text     = "Mod B wins ALL remaining";
            btnAllModB.Location = new Point(488, 8);
            btnAllModB.Size     = new Size(170, 28);
            btnAllModB.Click   += btnAllModB_Click;

            pnlNav.Controls.AddRange(new Control[]
            { btnPrevious, btnSkip, btnNext, btnAllModA, btnAllModB });

            // ── Done Button ───────────────────────────────────────────────────
            btnDone.Text     = "✓  DONE — Apply All Resolutions";
            btnDone.Location = new Point(12, 492);
            btnDone.Size     = new Size(660, 40);
            btnDone.Click   += btnDone_Click;

            Controls.AddRange(new Control[]
            {
                lblConflictTitle, lblProgress, lblSummary,
                pnlInfo, pnlValues,
                btnUseModA, btnUseModB,
                pnlNav, btnDone
            });

            ResumeLayout(false);
        }

        private Label   lblConflictTitle;
        private Label   lblProgress;
        private Label   lblSummary;
        private Panel   pnlInfo;
        private Label   lblCategory;
        private Label   lblFieldLabel;
        private Label   lblTechnical;
        private Label   lblPaddingWarning;
        private Panel   pnlValues;
        private Label   lblVanillaValue;
        private Label   lblModAValue;
        private Label   lblModAChange;
        private Label   lblModBValue;
        private Label   lblModBChange;
        private Label   lblCurrentChoice;
        private Button  btnUseModA;
        private Button  btnUseModB;
        private Panel   pnlNav;
        private Button  btnPrevious;
        private Button  btnSkip;
        private Button  btnNext;
        private Button  btnAllModA;
        private Button  btnAllModB;
        private Button  btnDone;
    }
}
