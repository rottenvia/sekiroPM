using System.Drawing.Drawing2D;

namespace SekiroParamMerger.WinForms
{
    /// <summary>
    /// A smooth rounded progress bar with a determinate / indeterminate mode,
    /// matching the dark theme.
    /// </summary>
    public class DarkProgressBar : Control
    {
        public int     Value          { get; private set; }
        public int     Maximum        { get; set; } = 100;
        public bool    IsIndeterminate{ get; set; }
        public Color   TrackColor     { get; set; } = Styling.BackgroundLight;
        public Color   FillColor      { get; set; } = Styling.AccentRed;
        public int     CornerRadius   { get; set; } = 8;

        private int _animOffset;

        public DarkProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.UserPaint
                   | ControlStyles.ResizeRedraw, true);
            DoubleBuffered = true;
            Height = 14;
        }

        public void SetValue(int value)
        {
            Value = Math.Clamp(value, 0, Maximum);
            Invalidate();
        }

        /// <summary>Starts a lightweight animation loop for indeterminate mode.</summary>
        public void StartIndeterminate()
        {
            IsIndeterminate = true;
            _animOffset = 0;
            var timer = new System.Windows.Forms.Timer { Interval = 40 };
            timer.Tick += (_, _) =>
            {
                if (!IsIndeterminate) { timer.Stop(); return; }
                _animOffset += 6;
                if (_animOffset > Width) _animOffset = 0;
                Invalidate();
            };
            timer.Start();
            Invalidate();
        }

        public void StopIndeterminate() { IsIndeterminate = false; Invalidate(); }

        protected override void OnPaintBackground(PaintEventArgs e) { }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            RectangleF track = new RectangleF(0, 0, Width - 1f, Height - 1f);
            using (var trackPath = Styling.RoundedRect(track, CornerRadius))
            using (var trackBrush = new SolidBrush(TrackColor))
                g.FillPath(trackBrush, trackPath);

            if (Maximum <= 0) return;

            float fillWidth;
            RectangleF fill;
            if (IsIndeterminate)
            {
                int segW = Width / 3;
                fillWidth = segW;
                fill = new RectangleF(_animOffset, 1, segW, Height - 2);
            }
            else
            {
                double frac = (double)Value / Maximum;
                fillWidth = (float)((Width - 2) * frac);
                fill = new RectangleF(1, 1, fillWidth, Height - 2);
            }

            if (fillWidth <= 0) return;

            // keep fill inside the rounded track
            if (fill.Right > Width - 1) fill.Width = Width - 1 - fill.X;

            using (var fillPath = Styling.RoundedRect(fill, CornerRadius))
            using (var fillBrush = new SolidBrush(FillColor))
                g.FillPath(fillBrush, fillPath);
        }
    }
}
