using System.Drawing.Drawing2D;

namespace SekiroParamMerger.WinForms
{
    /// <summary>
    /// A flat, rounded, hover-highlighted button — the CTk-button look.
    /// </summary>
    public class ModernButton : Button
    {
        public int   CornerRadius { get; set; } = 8;
        public Color BaseColor    { get; set; } = Styling.ButtonBackground;
        public Color HoverColor   { get; set; } = Styling.ButtonHover;
        public Color BorderColor  { get; set; } = Styling.BorderColor;
        public Color TextColor    { get; set; } = Styling.TextPrimary;
        public int   BorderSize   { get; set; } = 1;

        private bool _hover;

        public ModernButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.UserPaint
                   | ControlStyles.ResizeRedraw, true);
            FlatStyle      = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ForeColor      = TextColor;
            BackColor      = Color.Transparent;
            Cursor         = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true;  Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }
        protected override void OnTextChanged(EventArgs e) { Invalidate(); base.OnTextChanged(e); }
        protected override void OnFontChanged(EventArgs e) { Invalidate(); base.OnFontChanged(e); }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // blank — we paint the background
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            RectangleF rect = new RectangleF(0, 0, Width - 1f, Height - 1f);
            using var path = Styling.RoundedRect(rect, CornerRadius);

            Color bg = !Enabled ? Styling.BackgroundDark
                     : _hover ? HoverColor : BaseColor;

            using (var brush = new SolidBrush(bg))
                g.FillPath(brush, path);

            if (BorderSize > 0 && BorderColor != Color.Transparent)
            {
                RectangleF border = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
                using var borderPath = Styling.RoundedRect(border, CornerRadius);
                using var pen = new Pen(Enabled ? BorderColor : Styling.BackgroundLight, BorderSize);
                g.DrawPath(pen, borderPath);
            }

            TextRenderer.DrawText(
                g, Text, Font, ClientRectangle,
                Enabled ? TextColor : Styling.TextSecondary,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }
    }
}
