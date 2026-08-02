using System.ComponentModel;

namespace SekiroParamMerger.WinForms
{
    /// <summary>
    /// The custom title bar — a CTk-style header with the "sekiroPM" brand and a
    /// small "made by eggyeg" credit on the left, round minimize/close buttons on
    /// the right. Supports dragging the window by grabbing anywhere on it.
    /// </summary>
    public class AppTitleBar : Panel
    {
        public event EventHandler? CloseClicked;
        public event EventHandler? MinimizeClicked;

        private readonly Label _brand;
        private readonly Label _credit;
        private readonly Label _heading;
        private readonly RoundTitleButton _btnMin;
        private readonly RoundTitleButton _btnClose;

        private bool _dragging;
        private Point _dragStart;

        /// <summary>
        /// Optional heading text shown after the brand (e.g. "CONFLICT RESOLVER").
        /// Empty string hides it — used by the main window which only shows the brand.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Heading
        {
            get => _heading.Text;
            set { _heading.Text = value; Invalidate(); }
        }

        public AppTitleBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.UserPaint
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.SupportsTransparentBackColor, true);

            // Create all child controls BEFORE setting Height. Setting Height fires
            // OnResize/OnSizeChanged, and the layout code touches these children — if
            // they are still null we get a NullReferenceException at startup.
            _brand   = MakeLabel("sekiroPM", Styling.FontBrand, Styling.TextPrimary);
            _credit  = MakeLabel("made by eggyeg", Styling.FontCredit, Styling.TextSecondary);
            _heading = MakeLabel("", Styling.FontSmall, Styling.TextSecondary);

            _btnMin   = new RoundTitleButton { Glyph = "—" };
            _btnClose = new RoundTitleButton { Glyph = "✕", IsClose = true };

            _btnMin.Click   += (_, _) => MinimizeClicked?.Invoke(this, EventArgs.Empty);
            _btnClose.Click += (_, _) => CloseClicked?.Invoke(this, EventArgs.Empty);

            BackColor = Styling.TitleBarColor;
            Height    = Styling.TitleBarHeight;

            Controls.AddRange(new Control[] { _brand, _credit, _heading, _btnMin, _btnClose });

            // dragging: grab the bar background or the brand labels
            ApplyDrag(this);
            ApplyDrag(_brand);
            ApplyDrag(_credit);
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

            // Defensive: OnResize can fire before the constructor has assigned all
            // children (e.g. while setting Height). Layout is a no-op until they exist.
            if (_brand == null || _credit == null || _heading == null
                || _btnMin == null || _btnClose == null)
                return;

            int pad = 18;

            // buttons on the right, vertically centered
            _btnClose.Location = new Point(Width - pad - 32, (Height - 32) / 2);
            _btnMin.Location   = new Point(_btnClose.Left - 32 - 8, (Height - 32) / 2);

            // brand + credit on one line, vertically centered against the brand
            _brand.Location = new Point(pad, (Height - _brand.Height) / 2);

            // vertically center the credit against the brand's middle
            int brandMid = _brand.Top + (_brand.Height / 2);
            int creditTop = brandMid - (_credit.Height / 2);
            _credit.Location = new Point(_brand.Right + 10, creditTop);

            // optional heading between brand and buttons
            int headingRight = _btnMin.Left - 14;
            int headingLeft  = _credit.Right + 22;
            if (string.IsNullOrEmpty(_heading.Text) || headingRight - headingLeft < 40)
            {
                _heading.Visible = false;
            }
            else
            {
                _heading.Visible = true;
                int headingMid = brandMid;
                _heading.Location = new Point(headingLeft, headingMid - (_heading.Height / 2));
                _heading.MaximumSize = new Size(Math.Max(0, headingRight - headingLeft), _heading.Height);
            }
        }
    }
}
