using System.ComponentModel;

namespace SekiroParamMerger.WinForms
{
    /// <summary>
    /// The custom title bar — a CTk-style header with the "GHOST" + "MOD ENGINE"
    /// brand on the left, an optional heading, and round minimize/close buttons
    /// on the right. Supports dragging the window by grabbing anywhere on it.
    /// </summary>
    public class AppTitleBar : Panel
    {
        public event EventHandler? CloseClicked;
        public event EventHandler? MinimizeClicked;

        private readonly Label _brand;
        private readonly Label _brandSub;
        private readonly Label _heading;
        private readonly RoundTitleButton _btnMin;
        private readonly RoundTitleButton _btnClose;

        private bool _dragging;
        private Point _dragStart;

        /// <summary>Optional heading text shown between the brand and the buttons.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Heading
        {
            get => _heading.Text;
            set { _heading.Text = value; }
        }

        public AppTitleBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.UserPaint
                   | ControlStyles.ResizeRedraw, true);
            Height   = Styling.TitleBarHeight;
            BackColor = Styling.TitleBarColor;

            _brand = MakeLabel("GHOST", Styling.FontBrand, Styling.AccentGold);
            _brandSub = MakeLabel("MOD ENGINE", Styling.FontBrandSub, Styling.TextSecondary);
            _heading = MakeLabel("", Styling.FontSmall, Styling.TextSecondary);

            _btnMin   = new RoundTitleButton { Glyph = "—" };
            _btnClose = new RoundTitleButton { Glyph = "✕", IsClose = true };

            _btnMin.Click   += (_, _) => MinimizeClicked?.Invoke(this, EventArgs.Empty);
            _btnClose.Click += (_, _) => CloseClicked?.Invoke(this, EventArgs.Empty);

            Controls.AddRange(new Control[] { _brand, _brandSub, _heading, _btnMin, _btnClose });

            // dragging: grab the bar background or the brand labels
            ApplyDrag(this);
            ApplyDrag(_brand);
            ApplyDrag(_brandSub);
            ApplyDrag(_heading);
        }

        private static Label MakeLabel(string text, Font font, Color color)
        {
            return new Label
            {
                Text = text,
                Font = font,
                ForeColor = color,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
        }

        private void ApplyDrag(Control c)
        {
            c.MouseDown += TitleBar_MouseDown;
            c.MouseMove += TitleBar_MouseMove;
            c.MouseUp   += TitleBar_MouseUp;
        }

        private void TitleBar_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            _dragging = true;
            _dragStart = new Point(e.X, e.Y);
        }

        private void TitleBar_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_dragging) return;
            Form? form = FindForm();
            if (form == null) return;
            form.Location = new Point(
                form.Left + (e.X - _dragStart.X),
                form.Top  + (e.Y - _dragStart.Y));
        }

        private void TitleBar_MouseUp(object? sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int pad = 18;
            int y = (Height - 20) / 2;

            // brand
            _brand.Location  = new Point(pad, y);
            _brandSub.Location = new Point(pad + _brand.Width + 8, y + 6);

            // heading (right-aligned block sits before buttons)
            _btnClose.Location = new Point(Width - pad - 32, (Height - 32) / 2);
            _btnMin.Location   = new Point(_btnClose.Left - 32 - 8, (Height - 32) / 2);

            int headingRight = _btnMin.Left - 14;
            int headingLeft  = _brandSub.Right + 20;
            if (headingRight - headingLeft < 40)
            {
                _heading.Visible = false;
            }
            else
            {
                _heading.Visible = true;
                _heading.Location = new Point(headingLeft, y + 3);
                _heading.MaximumSize = new Size(Math.Max(0, headingRight - headingLeft), 20);
            }
        }
    }
}
