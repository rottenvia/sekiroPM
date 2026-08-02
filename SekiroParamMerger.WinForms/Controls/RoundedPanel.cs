using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace SekiroParamMerger.WinForms
{
    /// <summary>
    /// A panel that draws a rounded, bordered "card" — the CTk card look.
    /// The control is clipped to the rounded shape so its corners are
    /// transparent against the form background.
    /// </summary>
    public class RoundedPanel : Panel
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CornerRadius { get; set; } = Styling.CardRadius;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color FillColor { get; set; } = Styling.BackgroundMid;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BorderColor { get; set; } = Styling.BorderColor;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int BorderThickness { get; set; } = 1;

        public RoundedPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.UserPaint
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecreateRegion();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RecreateRegion();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RecreateRegion();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // intentionally blank — we paint everything ourselves
        }

        private void RecreateRegion()
        {
            if (Width < 1 || Height < 1) return;
            var bounds = new RectangleF(0, 0, Width, Height);
            using var path = Styling.RoundedRect(bounds, CornerRadius);
            Region = new Region(path);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            RectangleF rect = new RectangleF(0, 0, Width - 1f, Height - 1f);
            using var path = Styling.RoundedRect(rect, CornerRadius);

            using (var fill = new SolidBrush(FillColor))
                g.FillPath(fill, path);

            if (BorderThickness > 0)
            {
                // inset border so it stays fully inside the clipping region
                RectangleF border = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
                using var borderPath = Styling.RoundedRect(border, CornerRadius);
                using var pen = new Pen(BorderColor, BorderThickness);
                g.DrawPath(pen, borderPath);
            }
        }
    }
}
