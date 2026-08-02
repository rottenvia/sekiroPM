using System.ComponentModel;

namespace SekiroParamMerger.WinForms
{
    /// <summary>
    /// A small circular title-bar button (— / ✕). The close button glows red
    /// on hover, the minimize button glows grey.
    /// </summary>
    public class RoundTitleButton : Control
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Glyph { get; set; } = "—";
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsClose { get; set; }

        private bool _hover;

        public RoundTitleButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.UserPaint
                   | ControlStyles.ResizeRedraw, true);
            Size       = new Size(32, 32);
            BackColor  = Color.Transparent;
            Font       = new Font("Segoe UI", 11f, FontStyle.Bold);
            Cursor     = Cursors.Hand;
            TabStop    = false;
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true;  Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // blank
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(2, 2, Width - 5, Height - 5);
            Color circle = IsClose ? Styling.AccentRed : Styling.BackgroundLight;

            using (var brush = new SolidBrush(_hover ? circle : Color.FromArgb(30, 30, 30)))
                g.FillEllipse(brush, rect);

            using (var pen = new Pen(_hover ? circle : Styling.BorderColor))
                g.DrawEllipse(pen, rect);

            TextRenderer.DrawText(
                g, Glyph, Font,
                new Rectangle(0, 0, Width, Height),
                _hover ? Color.White : Styling.TextPrimary,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }
    }
}
