using TutoringMarketplace.Controls;

namespace TutoringMarketplace.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox  txtUsername  = null!;
        private TextBox  txtPassword  = null!;
        private Button   btnLogin     = null!;
        private Button   btnRegister  = null!;
        private Button   btnGuest     = null!;
        private Button   btnAdminLogin= null!;
        private Label    lblError     = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            Text            = "StudentHelp — Login";
            Size            = new Size(420, 600);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox     = true;
            MinimumSize     = new Size(380, 520);
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = Theme.Background;
            Font            = Theme.FontBody;

            // Hero
            var hero = new Panel { Location = new Point(0, 0), Size = new Size(420, 160), BackColor = Theme.PrimaryBlue };
            hero.Paint += (s, e) =>
            {
                using var br = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Point(0, 0), new Point(420, 160), Theme.PrimaryBlue, Theme.DarkBlue);
                e.Graphics.FillRectangle(br, hero.ClientRectangle);
            };
            hero.Controls.Add(new Label
            {
                Text = "🎓  StudentHelp", Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = Color.White, BackColor = Color.Transparent,
                AutoSize = false, Size = new Size(420, 50), Location = new Point(0, 36),
                TextAlign = ContentAlignment.MiddleCenter
            });
            hero.Controls.Add(new Label
            {
                Text = "Student-to-Student Tutoring Marketplace",
                Font = new Font("Segoe UI", 10f), ForeColor = Color.FromArgb(219, 234, 254),
                BackColor = Color.Transparent, AutoSize = false, Size = new Size(420, 26),
                Location = new Point(0, 94), TextAlign = ContentAlignment.MiddleCenter
            });
            Controls.Add(hero);

            // Card
            var card = UIHelper.MakeCard(30, 178, 360, 330);
            Controls.Add(card);

            card.Controls.Add(new Label
            {
                Text = "Welcome Back", Font = Theme.FontH2, ForeColor = Theme.TextPrimary,
                AutoSize = true, Location = new Point(20, 18), BackColor = Color.Transparent
            });
            card.Controls.Add(new Label
            {
                Text = "Sign in to your account", Font = Theme.FontSmall, ForeColor = Theme.TextSecondary,
                AutoSize = true, Location = new Point(20, 46), BackColor = Color.Transparent
            });

            card.Controls.Add(new Label { Text = "Username", Font = Theme.FontBold, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(20, 78), BackColor = Color.Transparent });
            txtUsername = UIHelper.MakeTextBox("Username", 320, 34);
            txtUsername.Location = new Point(20, 98);
            card.Controls.Add(txtUsername);

            card.Controls.Add(new Label { Text = "Password", Font = Theme.FontBold, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(20, 142), BackColor = Color.Transparent });
            txtPassword = UIHelper.MakeTextBox("Password", 320, 34, password: true);
            txtPassword.Location = new Point(20, 162);
            card.Controls.Add(txtPassword);

            lblError = new Label
            {
                Text = "", Font = Theme.FontSmall, ForeColor = Theme.RedText,
                AutoSize = false, Size = new Size(320, 20), Location = new Point(20, 202),
                BackColor = Color.Transparent, Visible = false
            };
            card.Controls.Add(lblError);

            btnLogin = UIHelper.MakeButton("Login", Theme.PrimaryBlue, Color.White, 320, 40);
            btnLogin.Location = new Point(20, 228);
            btnLogin.Click += btnLogin_Click;
            card.Controls.Add(btnLogin);

            card.Controls.Add(new Label
            {
                Text = "── or ──", Font = Theme.FontSmall, ForeColor = Color.FromArgb(156, 163, 175),
                AutoSize = true, Location = new Point(140, 278), BackColor = Color.Transparent
            });

            btnRegister = UIHelper.MakeButton("Create Account", Color.White, Theme.PrimaryBlue, 148, 34);
            btnRegister.Location = new Point(20, 296);
            btnRegister.FlatAppearance.BorderSize = 1;
            btnRegister.FlatAppearance.BorderColor = Theme.PrimaryBlue;
            btnRegister.Click += btnRegister_Click;
            card.Controls.Add(btnRegister);

            btnGuest = UIHelper.MakeButton("Browse as Guest", Color.White, Theme.TextSecondary, 148, 34);
            btnGuest.Location = new Point(192, 296);
            btnGuest.FlatAppearance.BorderSize = 1;
            btnGuest.FlatAppearance.BorderColor = Theme.BorderColor;
            btnGuest.Click += btnGuest_Click;
            card.Controls.Add(btnGuest);

            // Admin login button at bottom of form
            btnAdminLogin = UIHelper.MakeButton("🔐  Admin Login", Color.FromArgb(30, 41, 59), Color.FromArgb(148, 163, 184), 360, 36);
            btnAdminLogin.Location = new Point(30, 522);
            btnAdminLogin.FlatAppearance.BorderSize = 1;
            btnAdminLogin.FlatAppearance.BorderColor = Color.FromArgb(71, 85, 105);
            btnAdminLogin.Click += btnAdminLogin_Click;
            Controls.Add(btnAdminLogin);

            Controls.Add(new Label
            {
                Text = "Demo: username = demouser  |  password = demo123",
                Font = Theme.FontSmall, ForeColor = Theme.TextSecondary,
                AutoSize = false, Size = new Size(360, 18), Location = new Point(30, 504),
                TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent
            });
        }
    }
}
