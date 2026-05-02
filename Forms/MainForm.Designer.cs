using TutoringMarketplace.Controls;

namespace TutoringMarketplace.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel  _contentArea = null!;
        private Panel  _navBar      = null!;
        private Panel  _pnlUserArea = null!;
        private Button _btnHome      = null!;
        private Button _btnBrowse    = null!;
        private Button _btnPost      = null!;
        private Button _btnDashboard = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            Text            = "StudentHelp — Student Tutoring Marketplace (PH)";
            Size            = new Size(1920, 1080);
            MinimumSize     = new Size(1920, 1080);
            MaximumSize     = new Size(1920, 1080);   // locked unless maximised
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = Theme.Background;
            Font            = Theme.FontBody;

            // Disable manual resizing but keep the Maximise (fullscreen) button.
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = true;
            MinimizeBox     = true;

            // When the window is maximised/restored, temporarily lift the MaximumSize
            // constraint so the form can fill the screen.
            SizeChanged += (s, e) =>
            {
                if (WindowState == FormWindowState.Maximized)
                    MaximumSize = Size.Empty;
                else
                    MaximumSize = new Size(1920, 1080);
            };

            // ── Content area (added FIRST so DockStyle.Fill yields to Top dock) ─
            _contentArea = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };
            Controls.Add(_contentArea);

            // ── Nav bar (added SECOND so DockStyle.Top is processed first in layout) ─
            _navBar = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Color.White };
            _navBar.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Theme.BorderColor), 0, _navBar.Height - 1, _navBar.Width, _navBar.Height - 1);

            var logo = new Label
            {
                Text = "🎓  StudentHelp", Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Theme.PrimaryBlue, AutoSize = true, Location = new Point(16, 14),
                BackColor = Color.Transparent, Cursor = Cursors.Hand
            };
            logo.Click += (s, e) => NavigateTo("home");
            _navBar.Controls.Add(logo);

            _btnHome      = MakeNavBtn("Home",          "home");
            _btnBrowse    = MakeNavBtn("Browse",        "browse");
            _btnPost      = MakeNavBtn("Post Service",  "post");
            _btnDashboard = MakeNavBtn("My Dashboard",  "dashboard");

            int bx = 220;
            foreach (var btn in new[] { _btnHome, _btnBrowse, _btnPost, _btnDashboard })
            {
                btn.Location = new Point(bx, 0);
                _navBar.Controls.Add(btn);
                bx += btn.Width;
            }

            // user area (right side — anchored)
            _pnlUserArea = new Panel
            {
                Size = new Size(300, 56), BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _pnlUserArea.Location = new Point(_navBar.Width - 310, 0);
            _navBar.SizeChanged += (s, e) => _pnlUserArea.Location = new Point(_navBar.Width - 310, 0);
            _navBar.Controls.Add(_pnlUserArea);
            Controls.Add(_navBar);
        }

        private Button MakeNavBtn(string text, string page)
        {
            int w = TextRenderer.MeasureText(text, Theme.FontNav).Width + 28;
            var btn = new Button
            {
                Text = text, Size = new Size(w, 56), Font = Theme.FontNav,
                ForeColor = Theme.TextSecondary, BackColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Tag = page
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(243, 244, 246);
            btn.Click += (s, e) => NavigateTo(page);
            return btn;
        }
    }
}
