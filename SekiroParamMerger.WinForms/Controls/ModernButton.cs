using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace SekiroParamMerger.WinForms
{
    /// <summary>
    /// A flat, rounded, hover-highlighted button — the CTk-button look.
    /// </summary>
    public class ModernButton : Button
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CornerRadius { get; set; } = 8;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BaseColor { get; set; } = Styling.ButtonBackground;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color HoverColor { get; set; } = Styling.ButtonHover;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BorderColor { get; set; } = Styling.BorderColor;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color TextColor { get; set; } = Styling.TextPrimary;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int BorderSize { get; set; } = 1;

        private bool _hover;
        private bool _pressed;

        public ModernButton()
        {
            // IMPORTANT: we do NOT use a transparent back color. A transparent
            // background on a custom-painted control produces dark fringes along
            // the rounded edges (the classic "black edges" artifact). Instead we
            // paint the whole control and let the corners blend into a parent-
            // matching opaque BackColor.
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.UserPaint
                   | ControlStyles.ResizeRedraw, true);
            FlatStyle      = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ForeColor      = TextColor;
            BackColor      = Styling.BackgroundMid; // opaque — override per container
            Cursor         = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true;  Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; _pressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { _pressed = true;  Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e)   { _pressed = false; Invalidate(); base.OnMouseUp(e); }
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

            // Fill the entire control rectangle with the container-matching color
            // FIRST so the corners outside the rounded shape blend seamlessly
            // (otherwise they show black/dark fringes). Then draw the rounded
            // shape on top.
            using (var bgBrush = new SolidBrush(BackColor))
                g.FillRectangle(bgBrush, ClientRectangle);

            RectangleF rect = new RectangleF(0, 0, Width - 1f, Height - 1f);
            using var path = Styling.RoundedRect(rect, CornerRadius);

            Color bg = !Enabled ? Styling.BackgroundDark
                     : _pressed ? Darken(HoverColor)
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

        private static Color Darken(Color c)
        {
            return Color.FromArgb(
                Math.Max(0, c.R - 25),
                Math.Max(0, c.G - 25),
                Math.Max(0, c.B - 25));
        }
    }
}
