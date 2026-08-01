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
            lblTitle      = new Label();
            pnlSummary    = new Panel();
            lblStatA      = new Label();
            lblStatB      = new Label();
            lblConflicts  = new Label();
            lblOutputPath = new Label();
            pnlDelete     = new Panel();
            lblDeleteInfo = new Label();
            chkKeepFiles  = new CheckBox();
            lblSaveStatus = new Label();
            btnSave       = new Button();
            btnClose      = new Button();

            SuspendLayout();

            // ── Title ─────────────────────────────────────────────────────────
            lblTitle.Text     = "✓  MERGE COMPLETE — REVIEW & SAVE";
            lblTitle.Location = new Point(16, 14);
            lblTitle.AutoSize = true;

            // ── Summary Panel ─────────────────────────────────────────────────
            pnlSummary.Location = new Point(12, 52);
            pnlSummary.Size     = new Size(644, 120);
            pnlSummary.Padding  = new Padding(12);

            lblStatA.Location = new Point(12, 10);
            lblStatA.AutoSize = true;
            lblStatA.Font     = Styling.FontMedium;

            lblStatB.Location = new Point(12, 36);
            lblStatB.AutoSize = true;
            lblStatB.Font     = Styling.FontMedium;

            lblConflicts.Location = new Point(12, 62);
            lblConflicts.AutoSize = true;

            lblOutputPath.Location = new Point(12, 94);
            lblOutputPath.Size     = new Size(620, 18);
            lblOutputPath.Font     = Styling.FontSmall;

            pnlSummary.Controls.AddRange(new Control[]
            { lblStatA, lblStatB, lblConflicts, lblOutputPath });

            // ── Delete Panel ──────────────────────────────────────────────────
            pnlDelete.Location = new Point(12, 182);
            pnlDelete.Size     = new Size(644, 160);
            pnlDelete.Padding  = new Padding(12);

            lblDeleteInfo.Location = new Point(12, 10);
            lblDeleteInfo.Size     = new Size(620, 120);
            lblDeleteInfo.Font     = Styling.FontSmall;

            chkKeepFiles.Text     = "Keep Mod A and Mod B param files (do NOT delete them)";
            chkKeepFiles.Location = new Point(12, 130);
            chkKeepFiles.AutoSize = true;
            chkKeepFiles.CheckedChanged += chkKeepFiles_CheckedChanged;

            pnlDelete.Controls.AddRange(new Control[]
            { lblDeleteInfo, chkKeepFiles });

            // ── Save Status ───────────────────────────────────────────────────
            lblSaveStatus.Text     = "Ready to save.";
            lblSaveStatus.Location = new Point(12, 352);
            lblSaveStatus.Size     = new Size(644, 18);
            lblSaveStatus.ForeColor = Styling.TextSecondary;

            // ── Buttons ───────────────────────────────────────────────────────
            btnSave.Text     = "💾  SAVE MERGED FILE";
            btnSave.Location = new Point(12, 378);
            btnSave.Size     = new Size(644, 44);
            btnSave.Click   += btnSave_Click;

            btnClose.Text     = "Close";
            btnClose.Location = new Point(12, 430);
            btnClose.Size     = new Size(644, 32);
            btnClose.Click   += btnClose_Click;

            Controls.AddRange(new Control[]
            {
                lblTitle, pnlSummary, pnlDelete,
                lblSaveStatus, btnSave, btnClose
            });

            ResumeLayout(false);
        }

        private Label    lblTitle;
        private Panel    pnlSummary;
        private Label    lblStatA;
        private Label    lblStatB;
        private Label    lblConflicts;
        private Label    lblOutputPath;
        private Panel    pnlDelete;
        private Label    lblDeleteInfo;
        private CheckBox chkKeepFiles;
        private Label    lblSaveStatus;
        private Button   btnSave;
        private Button   btnClose;
    }
}
