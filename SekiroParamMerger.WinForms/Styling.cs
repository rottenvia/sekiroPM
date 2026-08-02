using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace SekiroParamMerger.WinForms
{
    /// <summary>
    /// Centralized styling for the entire application.
    /// All colors, fonts, and sizes defined in one place.
    /// Dark theme — matches the Sekiro aesthetic, in the style of a
    /// customtkinter ("CTk") modern dark UI.
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
        public static readonly Color TitleBarColor     = Color.FromArgb(22, 22, 22);

        // ── Fonts ─────────────────────────────────────────────────────────────
        public static readonly Font FontLarge    = new Font("Segoe UI", 14f, FontStyle.Bold);
        public static readonly Font FontMedium   = new Font("Segoe UI", 11f, FontStyle.Bold);
        public static readonly Font FontNormal   = new Font("Segoe UI", 10f, FontStyle.Regular);
        public static readonly Font FontSmall    = new Font("Segoe UI", 9f, FontStyle.Regular);
        public static readonly Font FontTiny     = new Font("Segoe UI", 8f, FontStyle.Regular);
        public static readonly Font FontMono     = new Font("Consolas", 9f, FontStyle.Regular);

        // Branding — "sekiroPM" custom title bar
        public static readonly Font FontBrand    = new Font("Segoe UI", 15f, FontStyle.Bold);
        public static readonly Font FontCredit   = new Font("Segoe UI", 8f, FontStyle.Regular);
        public static readonly Font FontAppTitle = new Font("Segoe UI", 19f, FontStyle.Bold);
        public static readonly Font FontAppSub   = new Font("Segoe UI", 9.5f, FontStyle.Regular);

        // Polished UI fonts
        public static readonly Font FontDisplay  = new Font("Segoe UI Semibold", 13f, FontStyle.Bold);   // mod names
        public static readonly Font FontLabel    = new Font("Segoe UI Semibold", 9f, FontStyle.Bold);     // small field labels

        // ── Sizes ─────────────────────────────────────────────────────────────
        public static readonly int CornerRadius  = 6;
        public static readonly int CardRadius    = 16;
        public static readonly int Padding       = 16;
        public static readonly int SmallPadding  = 8;
        public static readonly int TitleBarHeight = 48;

        // ── Native Win32 helpers for rounded windows / controls ───────────────

        public static class Native
        {
            [DllImport("user32.dll")]
            public static extern IntPtr CreateRoundRectRgn(int left, int top, int right, int bottom, int width, int height);

            [DllImport("user32.dll")]
            public static extern IntPtr CreateEllipticRgn(int left, int top, int right, int bottom);

            [DllImport("user32.dll")]
            public static extern bool SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

            [DllImport("user32.dll")]
            public static extern bool ReleaseCapture();

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        }

        /// <summary>
        /// Applies a rounded shape to an entire window (the modern CTk look).
        /// Call after the form handle exists (from the Load event).
        /// </summary>
        public static void MakeWindowRounded(Form form, int radius)
        {
            if (form.Handle == IntPtr.Zero)
            {
                form.HandleCreated += (_, _) => ApplyWindowRegion(form, radius);
                return;
            }
            ApplyWindowRegion(form, radius);
        }

        private static void ApplyWindowRegion(Form form, int radius)
        {
            try
            {
                if (form.Width < 1 || form.Height < 1) return;
                IntPtr region = Native.CreateRoundRectRgn(0, 0, form.Width + 1, form.Height + 1, radius, radius);
                Native.SetWindowRgn(form.Handle, region, true);
            }
            catch
            {
                // If the region fails, keep the window square and visible rather
                // than risk making it invisible.
            }
        }

        /// <summary>Rounded-rectangle GraphicsPath helper used by custom controls.</summary>
        public static GraphicsPath RoundedRect(RectangleF bounds, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
            {
                path.AddRectangle(bounds);
                path.CloseFigure();
                return path;
            }

            float d = radius * 2f;
            if (d > bounds.Width) d = bounds.Width;
            if (d > bounds.Height) d = bounds.Height;

            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        /// <summary>Applies dark theme base settings to any form.</summary>
        public static void ApplyDarkTheme(Form form)
        {
            form.BackColor = BackgroundDark;
            form.ForeColor = TextPrimary;
            form.Font      = FontNormal;
        }

        /// <summary>Standard dark text box.</summary>
        public static void StyleTextBox(TextBox tb)
        {
            tb.BackColor   = BackgroundLight;
            tb.ForeColor   = TextPrimary;
            tb.BorderStyle = BorderStyle.FixedSingle;
            tb.Font        = FontNormal;
        }

        /// <summary>Section header inside a card (gold).</summary>
        public static void StyleHeader(Label lbl)
        {
            lbl.Font      = FontMedium;
            lbl.ForeColor = AccentGold;
        }

        /// <summary>Secondary / dim text.</summary>
        public static void StyleSecondary(Label lbl)
        {
            lbl.Font      = FontSmall;
            lbl.ForeColor = TextSecondary;
        }

        /// <summary>Dark checkbox.</summary>
        public static void StyleCheckBox(CheckBox cb)
        {
            cb.ForeColor = TextPrimary;
            cb.Font      = FontNormal;
            cb.BackColor = BackgroundMid;
        }
    }
}
