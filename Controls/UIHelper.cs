namespace TutoringMarketplace.Controls
{
    public static class Theme
    {
        public static readonly Color PrimaryBlue   = Color.FromArgb(37, 99, 235);
        public static readonly Color LightBlue     = Color.FromArgb(219, 234, 254);
        public static readonly Color DarkBlue      = Color.FromArgb(30, 64, 175);
        public static readonly Color Background    = Color.FromArgb(249, 250, 251);
        public static readonly Color CardBg        = Color.White;
        public static readonly Color TextPrimary   = Color.FromArgb(17, 24, 39);
        public static readonly Color TextSecondary = Color.FromArgb(107, 114, 128);
        public static readonly Color BorderColor   = Color.FromArgb(229, 231, 235);
        public static readonly Color StarColor     = Color.FromArgb(251, 191, 36);
        public static readonly Color GreenLight    = Color.FromArgb(220, 252, 231);
        public static readonly Color GreenText     = Color.FromArgb(22, 163, 74);
        public static readonly Color PurpleLight   = Color.FromArgb(237, 233, 254);
        public static readonly Color PurpleText    = Color.FromArgb(124, 58, 237);
        public static readonly Color OrangeLight   = Color.FromArgb(255, 237, 213);
        public static readonly Color OrangeText    = Color.FromArgb(234, 88, 12);
        public static readonly Color RedLight      = Color.FromArgb(254, 226, 226);
        public static readonly Color RedText       = Color.FromArgb(185, 28, 28);
        public static readonly Font  FontTitle     = new("Segoe UI", 22f, FontStyle.Bold);
        public static readonly Font  FontH2        = new("Segoe UI", 16f, FontStyle.Bold);
        public static readonly Font  FontH3        = new("Segoe UI", 12f, FontStyle.Bold);
        public static readonly Font  FontBody      = new("Segoe UI", 10f);
        public static readonly Font  FontSmall     = new("Segoe UI", 9f);
        public static readonly Font  FontBold      = new("Segoe UI", 10f, FontStyle.Bold);
        public static readonly Font  FontNav       = new("Segoe UI", 11f, FontStyle.Bold);
        public const string          PesoSign      = "₱";
    }

    public static class UIHelper
    {
        public static Panel MakeCard(int x, int y, int w, int h, int radius = 8)
        {
            var p = new Panel { Location = new Point(x, y), Size = new Size(w, h), BackColor = Theme.CardBg };
            p.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var pen  = new Pen(Theme.BorderColor, 1);
                using var path = RoundRect(new Rectangle(0, 0, p.Width - 1, p.Height - 1), radius);
                g.DrawPath(pen, path);
            };
            return p;
        }

        public static System.Drawing.Drawing2D.GraphicsPath RoundRect(Rectangle r, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.Left, r.Top, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        public static Button MakeButton(string text, Color bg, Color fg, int w = 130, int h = 38)
        {
            var btn = new Button
            {
                Text = text, Size = new Size(w, h), BackColor = bg, ForeColor = fg,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = Theme.FontBold
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(bg, 0.08f);
            return btn;
        }

        public static Label MakeLabel(string text, Font font, Color color) =>
            new() { Text = text, Font = font, ForeColor = color, AutoSize = true, BackColor = Color.Transparent };

        public static TextBox MakeTextBox(string placeholder, int w, int h = 32, bool password = false)
        {
            var tb = new TextBox
            {
                Size = new Size(w, h), Font = Theme.FontBody, BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Theme.TextSecondary, Text = placeholder
            };
            // Always start unmasked so placeholder text is readable
            tb.UseSystemPasswordChar = false;
            bool placeholding = true;
            tb.GotFocus += (s, e) =>
            {
                if (placeholding)
                {
                    // Enable masking BEFORE changing Text to avoid the password-char rendering error
                    if (password) tb.UseSystemPasswordChar = true;
                    tb.Text      = "";
                    tb.ForeColor = Theme.TextPrimary;
                    placeholding = false;
                }
            };
            tb.LostFocus += (s, e) =>
            {
                if (tb.Text == "")
                {
                    // Disable masking BEFORE restoring placeholder so it shows as plain text
                    if (password) tb.UseSystemPasswordChar = false;
                    tb.Text      = placeholder;
                    tb.ForeColor = Theme.TextSecondary;
                    placeholding = true;
                }
            };
            return tb;
        }

        public static string StarString(double rating, int max = 5)
        {
            int filled = (int)Math.Round(rating);
            return new string('★', Math.Clamp(filled, 0, max)) + new string('☆', Math.Clamp(max - filled, 0, max));
        }

        public static Panel MakeBadge(string text, Color bg, Color fg)
        {
            var sz  = TextRenderer.MeasureText(text, Theme.FontSmall);
            var p   = new Panel { Size = new Size(sz.Width + 16, 22), BackColor = bg };
            p.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var br   = new SolidBrush(bg);
                using var path = RoundRect(new Rectangle(0, 0, p.Width - 1, p.Height - 1), 10);
                e.Graphics.FillPath(br, path);
                e.Graphics.DrawString(text, Theme.FontSmall, new SolidBrush(fg),
                    new RectangleF(0, 0, p.Width, p.Height),
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            };
            return p;
        }

        public static Panel MakeStatusBadge(string status)
        {
            var (bg, fg) = status switch
            {
                "Completed" => (Theme.GreenLight,  Theme.GreenText),
                "Active"    => (Theme.LightBlue,   Theme.PrimaryBlue),
                "Cancelled" => (Theme.RedLight,     Theme.RedText),
                _           => (Theme.OrangeLight,  Theme.OrangeText)
            };
            return MakeBadge(status, bg, fg);
        }

        public static Panel MakeAvatar(string initials, Color bg, int size = 48)
        {
            var text = initials.Length >= 2 ? initials[..2].ToUpper() : initials.ToUpper();
            var p    = new Panel { Size = new Size(size, size), BackColor = Color.Transparent };
            p.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var br = new SolidBrush(bg);
                e.Graphics.FillEllipse(br, 0, 0, size - 1, size - 1);
                using var f  = new Font("Segoe UI", size * 0.28f, FontStyle.Bold);
                e.Graphics.DrawString(text, f, Brushes.White, new RectangleF(0, 0, size, size),
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            };
            return p;
        }

        static readonly Color[] AvatarColors =
        {
            Color.FromArgb(59,130,246), Color.FromArgb(16,185,129),
            Color.FromArgb(245,101,101), Color.FromArgb(139,92,246),
            Color.FromArgb(251,146,60),  Color.FromArgb(20,184,166),
        };
        public static Color AvatarColor(string name) =>
            AvatarColors[Math.Abs(name.GetHashCode()) % AvatarColors.Length];

        public static string FormatPeso(decimal amount) => $"₱{amount:N0}";

        public static void RoundCorners(Panel p, int radius)
        {
            p.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, p.Width, p.Height, radius, radius));
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
    }
}

// Extension for drawing rounded rectangles
namespace System.Drawing.Drawing2D
{
    internal static class GraphicsPathExtensions
    {
        public static void AddRoundedRectangle(this GraphicsPath path, Rectangle bounds, int radius)
        {
            int d = radius * 2;
            path.AddArc(bounds.Left, bounds.Top, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Top, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
        }
    }
}
