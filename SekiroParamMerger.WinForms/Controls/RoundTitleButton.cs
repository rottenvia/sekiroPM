using System.ComponentModel;

namespace SekiroParamMerger.WinForms
{
    /// <summary>
    /// A small circular title-bar button (— / ✕). The close button glows red
    /// on hover, the minimize button glows grey. The control is clipped to a
    /// circle via Region, so the corners outside it are truly transparent.
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
            BackColor  = Styling.TitleBarColor;
            Font       = new Font("Segoe UI", 11f, FontStyle.Bold);
            Cursor     = Cursors.Hand;
            TabStop    = false;
        }

        // ── Clip to a circle so corners are transparent ──────────────────────
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RecreateRegion();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecreateRegion();
        }

        private void RecreateRegion()
        {
            if (Width < 1 || Height < 1) return;
            using var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(1, 1, Width - 2, Height - 2);
            Region = new Region(path);
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true;  Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // corners are clipped by the region
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
