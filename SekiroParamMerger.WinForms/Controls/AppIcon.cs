using System.Drawing.Drawing2D;

namespace SekiroParamMerger.WinForms
{
    /// <summary>
    /// Builds the application icon at runtime (no external .ico file needed).
    /// A dark rounded tile with a gold katana silhouette on a red sun disc.
    /// </summary>
    public static class AppIcon
    {
        private static Icon? _cached;

        public static Icon Create()
        {
            if (_cached != null) return _cached;
            try
            {
                _cached = Build();
            }
            catch
            {
                // Never let icon creation crash the app or prevent the window.
                _cached = SystemIcons.Application;
            }
            return _cached;
        }

        private static Icon Build()
        {
            using var bmp = new Bitmap(64, 64);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // dark rounded tile
                var tile = new RectangleF(2, 2, 60, 60);
                using (var tilePath = Styling.RoundedRect(tile, 14))
                using (var tileBrush = new SolidBrush(Color.FromArgb(30, 30, 30)))
                {
                    g.FillPath(tileBrush, tilePath);
                }

                // red sun disc (upper right)
                g.FillEllipse(new SolidBrush(Styling.AccentRed), 34, 8, 20, 20);

                // gold katana blade (diagonal)
                using (var blade = new Pen(Styling.AccentGold, 6f))
                using (var edge = new Pen(Color.FromArgb(255, 230, 170), 2.5f))
                {
                    var p1 = new PointF(12, 54);
                    var p2 = new PointF(44, 16);
                    g.DrawLine(blade, p1, p2);
                    // edge highlight
                    g.DrawLine(edge, p1.X - 2, p1.Y + 2, p2.X - 2, p2.Y + 2);
                }

                // guard (tsuba) near bottom
                using (var guard = new Pen(Styling.AccentGold, 5f))
                {
                    g.DrawEllipse(guard, 6, 42, 12, 12);
                }
            }

            return Icon.FromHandle(bmp.GetHicon());
        }
    }
}
