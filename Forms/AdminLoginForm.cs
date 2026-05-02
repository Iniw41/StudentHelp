using TutoringMarketplace.Controls;
using TutoringMarketplace.Database;

namespace TutoringMarketplace.Forms
{
    public class AdminLoginForm : Form
    {
        private TextBox txtUsername = null!;
        private TextBox txtPassword = null!;
        private Label   lblError    = null!;

        public AdminLoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text            = "StudentHelp — Admin Login";
            Size            = new Size(420, 560);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = Color.FromArgb(15, 23, 42);
            Font            = Theme.FontBody;

            // Dark hero
            var hero = new Panel { Location = new Point(0, 0), Size = new Size(420, 160), BackColor = Color.FromArgb(15, 23, 42) };
            hero.Paint += (s, e) =>
            {
                using var br = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Point(0, 0), new Point(420, 160),
                    Color.FromArgb(15, 23, 42), Color.FromArgb(30, 41, 59));
                e.Graphics.FillRectangle(br, hero.ClientRectangle);
            };
            hero.Controls.Add(new Label
            {
                Text = "🔐  Admin Portal", Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = Color.FromArgb(248, 250, 252), BackColor = Color.Transparent,
                AutoSize = false, Size = new Size(420, 50), Location = new Point(0, 36),
                TextAlign = ContentAlignment.MiddleCenter
            });
            hero.Controls.Add(new Label
            {
                Text = "StudentHelp Platform Administration",
                Font = new Font("Segoe UI", 10f), ForeColor = Color.FromArgb(148, 163, 184),
                BackColor = Color.Transparent, AutoSize = false, Size = new Size(420, 26),
                Location = new Point(0, 94), TextAlign = ContentAlignment.MiddleCenter
            });
            Controls.Add(hero);

            // Dark card
            var card = new Panel
            {
                Location  = new Point(30, 178),
                Size      = new Size(360, 290),
                BackColor = Color.FromArgb(30, 41, 59)
            };
            card.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(51, 65, 85), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };
            Controls.Add(card);

            card.Controls.Add(new Label
            {
                Text = "Restricted Access", Font = Theme.FontH2, ForeColor = Color.FromArgb(248, 250, 252),
                AutoSize = true, Location = new Point(20, 18), BackColor = Color.Transparent
            });
            card.Controls.Add(new Label
            {
                Text = "Administrator credentials required", Font = Theme.FontSmall, ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true, Location = new Point(20, 46), BackColor = Color.Transparent
            });

            card.Controls.Add(new Label { Text = "Username", Font = Theme.FontBold, ForeColor = Color.FromArgb(203, 213, 225), AutoSize = true, Location = new Point(20, 78), BackColor = Color.Transparent });
            txtUsername = new TextBox
            {
                PlaceholderText = "Admin username",
                Size      = new Size(320, 34),
                Location  = new Point(20, 98),
                Font      = Theme.FontBody,
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle
            };
            card.Controls.Add(txtUsername);

            card.Controls.Add(new Label { Text = "Password", Font = Theme.FontBold, ForeColor = Color.FromArgb(203, 213, 225), AutoSize = true, Location = new Point(20, 142), BackColor = Color.Transparent });
            txtPassword = new TextBox
            {
                PlaceholderText = "Admin password",
                UseSystemPasswordChar = true,
                Size      = new Size(320, 34),
                Location  = new Point(20, 162),
                Font      = Theme.FontBody,
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle
            };
            card.Controls.Add(txtPassword);

            lblError = new Label
            {
                Text = "", Font = Theme.FontSmall, ForeColor = Color.FromArgb(252, 165, 165),
                AutoSize = false, Size = new Size(320, 20), Location = new Point(20, 202),
                BackColor = Color.Transparent, Visible = false
            };
            card.Controls.Add(lblError);

            var btnLogin = new Button
            {
                Text      = "Access Admin Panel",
                Size      = new Size(320, 40),
                Location  = new Point(20, 228),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 38, 38),
                ForeColor = Color.White,
                Font      = Theme.FontBold,
                Cursor    = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += (s, e) =>
            {
                string user = txtUsername.Text.Trim();
                string pass = txtPassword.Text.Trim();
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                { lblError.Text = "Please enter credentials."; lblError.Visible = true; return; }

                var account = DatabaseManager.Login(user, pass);
                if (account == null || !account.IsAdmin)
                { lblError.Text = "Invalid admin credentials."; lblError.Visible = true; return; }

                Session.Login(account);
                DialogResult = DialogResult.OK;
                Close();
            };
            card.Controls.Add(btnLogin);

            // Back button
            var btnBack = new Button
            {
                Text = "← Back to User Login",
                Size = new Size(360, 30),
                Location = new Point(30, 484),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = Theme.FontSmall,
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnBack);
        }
    }
}
