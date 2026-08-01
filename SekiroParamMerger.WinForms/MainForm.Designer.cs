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
            lblTitle            = new Label();
            pnlGame             = new Panel();
            lblGameSection      = new Label();
            lblGameFolderHint   = new Label();
            txtGameFolder       = new TextBox();
            btnBrowseGame       = new Button();
            lblVanillaStatus    = new Label();
            pnlModA             = new Panel();
            lblModASection      = new Label();
            lblModAName         = new Label();
            txtModAName         = new TextBox();
            lblModAPath         = new Label();
            txtModAPath         = new TextBox();
            btnBrowseModA       = new Button();
            lblModANameHint     = new Label();
            lblModAPriority     = new Label();
            pnlModB             = new Panel();
            lblModBSection      = new Label();
            lblModBName         = new Label();
            txtModBName         = new TextBox();
            lblModBPath         = new Label();
            txtModBPath         = new TextBox();
            btnBrowseModB       = new Button();
            lblModBNameHint     = new Label();
            pnlOutput           = new Panel();
            lblOutputSection    = new Label();
            lblOutputHint       = new Label();
            txtOutputFolder     = new TextBox();
            btnBrowseOutput     = new Button();
            chkKeepModFiles     = new CheckBox();
            pnlStatus           = new Panel();
            lblStatus           = new Label();
            btnMerge            = new Button();

            SuspendLayout();

            // ── Title ─────────────────────────────────────────────────────────
            lblTitle.Text      = "⚔  SEKIRO PARAM MERGER";
            lblTitle.Font      = Styling.FontLarge;
            lblTitle.ForeColor = Styling.AccentGold;
            lblTitle.Location  = new Point(16, 14);
            lblTitle.Size      = new Size(500, 30);
            lblTitle.AutoSize  = true;

            // ── Game Panel ────────────────────────────────────────────────────
            pnlGame.Location = new Point(12, 52);
            pnlGame.Size     = new Size(748, 90);
            pnlGame.Padding  = new Padding(10);

            lblGameSection.Text     = "GAME FOLDER";
            lblGameSection.Location = new Point(10, 8);
            lblGameSection.AutoSize = true;

            lblGameFolderHint.Text     = "Select once — the folder containing sekiro.exe";
            lblGameFolderHint.Location = new Point(10, 26);
            lblGameFolderHint.AutoSize = true;

            txtGameFolder.Location = new Point(10, 44);
            txtGameFolder.Size     = new Size(618, 24);
            txtGameFolder.ReadOnly = true;

            btnBrowseGame.Text     = "Browse";
            btnBrowseGame.Location = new Point(636, 43);
            btnBrowseGame.Size     = new Size(90, 26);
            btnBrowseGame.Click   += btnBrowseGame_Click;

            lblVanillaStatus.Text     = "";
            lblVanillaStatus.Location = new Point(10, 72);
            lblVanillaStatus.Size     = new Size(718, 14);

            pnlGame.Controls.AddRange(new Control[]
            {
                lblGameSection, lblGameFolderHint,
                txtGameFolder, btnBrowseGame, lblVanillaStatus
            });

            // ── Mod A Panel ───────────────────────────────────────────────────
            pnlModA.Location = new Point(12, 152);
            pnlModA.Size     = new Size(748, 100);
            pnlModA.Padding  = new Padding(10);

            lblModASection.Text     = "MOD A";
            lblModASection.Location = new Point(10, 8);
            lblModASection.AutoSize = true;

            lblModAPriority.Text     = "← Higher priority — wins conflicts if not resolved manually";
            lblModAPriority.Location = new Point(75, 10);
            lblModAPriority.AutoSize = true;

            lblModAName.Text     = "Name:";
            lblModAName.Location = new Point(10, 34);
            lblModAName.AutoSize = true;

            txtModAName.Location = new Point(55, 31);
            txtModAName.Size     = new Size(180, 24);
            txtModAName.PlaceholderText = "e.g. Combat Overhaul";

            lblModANameHint.Text = "  File:";
            lblModANameHint.Location = new Point(244, 34);
            lblModANameHint.AutoSize = true;

            txtModAPath.Location = new Point(284, 31);
            txtModAPath.Size     = new Size(344, 24);
            txtModAPath.ReadOnly = true;
            txtModAPath.PlaceholderText = "No file selected";

            btnBrowseModA.Text     = "Browse";
            btnBrowseModA.Location = new Point(636, 30);
            btnBrowseModA.Size     = new Size(90, 26);
            btnBrowseModA.Click   += btnBrowseModA_Click;

            lblModAPath.Text     = "Select the gameparam.parambnd.dcx file from Mod A's folder";
            lblModAPath.Location = new Point(10, 62);
            lblModAPath.AutoSize = true;
            lblModAPath.ForeColor = Styling.TextSecondary;
            lblModAPath.Font = Styling.FontTiny;

            pnlModA.Controls.AddRange(new Control[]
            {
                lblModASection, lblModAPriority,
                lblModAName, txtModAName,
                lblModANameHint, txtModAPath, btnBrowseModA, lblModAPath
            });

            // ── Mod B Panel ───────────────────────────────────────────────────
            pnlModB.Location = new Point(12, 262);
            pnlModB.Size     = new Size(748, 90);
            pnlModB.Padding  = new Padding(10);

            lblModBSection.Text     = "MOD B";
            lblModBSection.Location = new Point(10, 8);
            lblModBSection.AutoSize = true;

            lblModBName.Text     = "Name:";
            lblModBName.Location = new Point(10, 34);
            lblModBName.AutoSize = true;

            txtModBName.Location = new Point(55, 31);
            txtModBName.Size     = new Size(180, 24);
            txtModBName.PlaceholderText = "e.g. Enemy Rebalance";

            lblModBNameHint.Text = "  File:";
            lblModBNameHint.Location = new Point(244, 34);
            lblModBNameHint.AutoSize = true;

            txtModBPath.Location = new Point(284, 31);
            txtModBPath.Size     = new Size(344, 24);
            txtModBPath.ReadOnly = true;
            txtModBPath.PlaceholderText = "No file selected";

            btnBrowseModB.Text     = "Browse";
            btnBrowseModB.Location = new Point(636, 30);
            btnBrowseModB.Size     = new Size(90, 26);
            btnBrowseModB.Click   += btnBrowseModB_Click;

            lblModBPath.Text     = "Select the gameparam.parambnd.dcx file from Mod B's folder";
            lblModBPath.Location = new Point(10, 62);
            lblModBPath.AutoSize = true;
            lblModBPath.ForeColor = Styling.TextSecondary;
            lblModBPath.Font = Styling.FontTiny;

            pnlModB.Controls.AddRange(new Control[]
            {
                lblModBSection,
                lblModBName, txtModBName,
                lblModBNameHint, txtModBPath, btnBrowseModB, lblModBPath
            });

            // ── Output Panel ──────────────────────────────────────────────────
            pnlOutput.Location = new Point(12, 362);
            pnlOutput.Size     = new Size(748, 90);
            pnlOutput.Padding  = new Padding(10);

            lblOutputSection.Text     = "OUTPUT FOLDER";
            lblOutputSection.Location = new Point(10, 8);
            lblOutputSection.AutoSize = true;

            lblOutputHint.Text = "Merged file saves to [output]\\param\\gameparam\\gameparam.parambnd.dcx — remembered between sessions";
            lblOutputHint.Location = new Point(10, 26);
            lblOutputHint.AutoSize = true;

            txtOutputFolder.Location = new Point(10, 46);
            txtOutputFolder.Size     = new Size(618, 24);
            txtOutputFolder.ReadOnly = true;
            txtOutputFolder.TextChanged += txtOutputFolder_TextChanged;

            btnBrowseOutput.Text     = "Browse";
            btnBrowseOutput.Location = new Point(636, 45);
            btnBrowseOutput.Size     = new Size(90, 26);
            btnBrowseOutput.Click   += btnBrowseOutput_Click;

            chkKeepModFiles.Text     = "Keep Mod A and Mod B param files after merge (for future merges with other mods)";
            chkKeepModFiles.Location = new Point(10, 72);
            chkKeepModFiles.AutoSize = true;
            chkKeepModFiles.CheckedChanged += chkKeepModFiles_CheckedChanged;

            pnlOutput.Controls.AddRange(new Control[]
            {
                lblOutputSection, lblOutputHint,
                txtOutputFolder, btnBrowseOutput, chkKeepModFiles
            });

            // ── Status Panel ──────────────────────────────────────────────────
            pnlStatus.Location = new Point(12, 462);
            pnlStatus.Size     = new Size(748, 36);

            lblStatus.Text     = "Select your game folder to begin.";
            lblStatus.Location = new Point(10, 10);
            lblStatus.Size     = new Size(728, 18);
            lblStatus.ForeColor = Styling.TextSecondary;

            pnlStatus.Controls.Add(lblStatus);

            // ── Merge Button ──────────────────────────────────────────────────
            btnMerge.Text     = "⚔  MERGE MODS";
            btnMerge.Location = new Point(12, 506);
            btnMerge.Size     = new Size(748, 48);
            btnMerge.Enabled  = false;
            btnMerge.Click   += btnMerge_Click;

            // ── Add all to form ───────────────────────────────────────────────
            Controls.AddRange(new Control[]
            {
                lblTitle,
                pnlGame,
                pnlModA,
                pnlModB,
                pnlOutput,
                pnlStatus,
                btnMerge
            });

            ResumeLayout(false);
        }

        // ── Control declarations ──────────────────────────────────────────────
        private Label   lblTitle;
        private Panel   pnlGame;
        private Label   lblGameSection;
        private Label   lblGameFolderHint;
        private TextBox txtGameFolder;
        private Button  btnBrowseGame;
        private Label   lblVanillaStatus;
        private Panel   pnlModA;
        private Label   lblModASection;
        private Label   lblModAName;
        private TextBox txtModAName;
        private Label   lblModANameHint;
        private Label   lblModAPath;
        private TextBox txtModAPath;
        private Button  btnBrowseModA;
        private Label   lblModAPriority;
        private Panel   pnlModB;
        private Label   lblModBSection;
        private Label   lblModBName;
        private TextBox txtModBName;
        private Label   lblModBNameHint;
        private Label   lblModBPath;
        private TextBox txtModBPath;
        private Button  btnBrowseModB;
        private Panel   pnlOutput;
        private Label   lblOutputSection;
        private Label   lblOutputHint;
        private TextBox txtOutputFolder;
        private Button  btnBrowseOutput;
        private CheckBox chkKeepModFiles;
        private Panel   pnlStatus;
        private Label   lblStatus;
        private Button  btnMerge;
    }
}
