namespace SekiroParamMerger.WinForms
{
    /// <summary>
    /// Centralized styling for the entire application.
    /// All colors, fonts, and sizes defined in one place.
    /// Dark theme — matches the Sekiro aesthetic.
    /// </summary>
    public static class Styling
    {
        // ── Colors ────────────────────────────────────────────────────────────
        public static readonly Color BackgroundDark    = Color.FromArgb(18, 18, 18);
        public static readonly Color BackgroundMid     = Color.FromArgb(28, 28, 28);
        public static readonly Color BackgroundLight   = Color.FromArgb(40, 40, 40);
        public static readonly Color AccentRed         = Color.FromArgb(180, 40, 40);
        public static readonly Color AccentRedLight    = Color.FromArgb(220, 60, 60);
        public static readonly Color AccentGold        = Color.FromArgb(200, 160, 60);
        public static readonly Color TextPrimary       = Color.FromArgb(230, 230, 230);
        public static readonly Color TextSecondary     = Color.FromArgb(150, 150, 150);
        public static readonly Color TextSuccess       = Color.FromArgb(80, 200, 100);
        public static readonly Color TextWarning       = Color.FromArgb(220, 180, 50);
        public static readonly Color TextDanger        = Color.FromArgb(220, 70, 70);
        public static readonly Color BorderColor       = Color.FromArgb(60, 60, 60);
        public static readonly Color ButtonBackground  = Color.FromArgb(50, 50, 50);
        public static readonly Color ButtonHover       = Color.FromArgb(70, 70, 70);
        public static readonly Color ModAColor         = Color.FromArgb(60, 140, 220);
        public static readonly Color ModBColor         = Color.FromArgb(80, 200, 130);

        // ── Fonts ─────────────────────────────────────────────────────────────
        public static readonly Font FontLarge    = new Font("Segoe UI", 14f, FontStyle.Bold);
        public static readonly Font FontMedium   = new Font("Segoe UI", 11f, FontStyle.Bold);
        public static readonly Font FontNormal   = new Font("Segoe UI", 10f, FontStyle.Regular);
        public static readonly Font FontSmall    = new Font("Segoe UI", 9f, FontStyle.Regular);
        public static readonly Font FontTiny     = new Font("Segoe UI", 8f, FontStyle.Regular);
        public static readonly Font FontMono     = new Font("Consolas", 9f, FontStyle.Regular);

        // ── Sizes ─────────────────────────────────────────────────────────────
        public static readonly int CornerRadius  = 6;
        public static readonly int Padding       = 16;
        public static readonly int SmallPadding  = 8;

        // ── Helpers ───────────────────────────────────────────────────────────

        /// <summary>Applies dark theme base settings to any form</summary>
        public static void ApplyDarkTheme(Form form)
        {
            form.BackColor = BackgroundDark;
            form.ForeColor = TextPrimary;
            form.Font      = FontNormal;
        }

        /// <summary>Styles a button with our dark theme</summary>
        public static void StyleButton(Button btn, bool isPrimary = false)
        {
            btn.FlatStyle             = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = isPrimary ? AccentRed : BorderColor;
            btn.FlatAppearance.BorderSize  = 1;
            btn.BackColor             = isPrimary ? AccentRed : ButtonBackground;
            btn.ForeColor             = TextPrimary;
            btn.Font                  = FontNormal;
            btn.Cursor                = Cursors.Hand;
            btn.FlatAppearance.MouseOverBackColor = isPrimary ? AccentRedLight : ButtonHover;
        }

        /// <summary>Styles a text box with dark theme</summary>
        public static void StyleTextBox(TextBox tb)
        {
            tb.BackColor  = BackgroundLight;
            tb.ForeColor  = TextPrimary;
            tb.BorderStyle = BorderStyle.FixedSingle;
            tb.Font       = FontNormal;
        }

        /// <summary>Styles a label as a section header</summary>
        public static void StyleHeader(Label lbl)
        {
            lbl.Font      = FontMedium;
            lbl.ForeColor = AccentGold;
        }

        /// <summary>Styles a label as secondary/dim text</summary>
        public static void StyleSecondary(Label lbl)
        {
            lbl.Font      = FontSmall;
            lbl.ForeColor = TextSecondary;
        }

        /// <summary>Styles a panel as a card/section</summary>
        public static void StyleCard(Panel panel)
        {
            panel.BackColor = BackgroundMid;
        }

        /// <summary>Styles a checkbox with dark theme</summary>
        public static void StyleCheckBox(CheckBox cb)
        {
            cb.ForeColor = TextPrimary;
            cb.Font      = FontNormal;
            cb.BackColor = Color.Transparent;
        }
    }
}
