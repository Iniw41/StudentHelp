using TutoringMarketplace.Controls;

namespace TutoringMarketplace.Forms
{
    partial class RegisterForm
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox  txtFullName    = null!;
        private TextBox  txtUsername    = null!;
        private TextBox  txtEmail       = null!;
        private TextBox  txtPassword    = null!;
        private TextBox  txtConfirm     = null!;
        private TextBox  txtUniversity  = null!;
        private TextBox  txtMajor       = null!;
        private TextBox  txtContact     = null!;   // NEW — contact info
        private ComboBox cmbYear        = null!;
        private Button   btnRegister    = null!;
        private Button   btnCancel      = null!;
        private Label    lblError       = null!;

        private Panel    pnlHeader      = null!;
        private Panel    pnlCard        = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            Text            = "StudentHelp — Create Account";
            Size            = new Size(520, 780);
            MinimumSize     = new Size(480, 700);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox     = true;
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = Theme.Background;
            Font            = Theme.FontBody;

            // ── Scrollable body  ── added FIRST so Fill yields to Top header ──
            var pnlBody = new Panel
            {
                Dock       = DockStyle.Fill,
                AutoScroll = true,
                BackColor  = Theme.Background,
                Padding    = new Padding(0)
            };
            Controls.Add(pnlBody);   // Fill — must come before the Top header

            // ── Header ── added SECOND so DockStyle.Top is claimed first ──────
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Theme.PrimaryBlue };
            pnlHeader.Controls.Add(new Label
            {
                Text      = "🎓  Create Your Account",
                Font      = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            });
            Controls.Add(pnlHeader);   // Top

            // ── Card inside the scrollable body ─────────────────────────────
            int initCardW = 460;
            pnlCard = new Panel
            {
                BackColor = Theme.CardBg,
                Location  = new Point(20, 16),
                Width     = initCardW,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlCard.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var pen  = new Pen(Theme.BorderColor, 1);
                using var path = UIHelper.RoundRect(new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1), 8);
                g.DrawPath(pen, path);
            };
            pnlBody.Controls.Add(pnlCard);

            // Resize: stretch card to fill body width
            pnlBody.Resize += (s, e) =>
            {
                int newW = Math.Max(380, pnlBody.ClientSize.Width - 40);
                pnlCard.Width = newW;
                ResizeCardControls();
            };

            BuildCardControls();
        }

        private void BuildCardControls()
        {
            int cy     = 16;
            int innerW = pnlCard.Width - 32;

            void AddField(string labelText, ref TextBox tb, string placeholder, bool pw = false)
            {
                pnlCard.Controls.Add(new Label
                {
                    Text = labelText, Font = Theme.FontBold, ForeColor = Theme.TextPrimary,
                    AutoSize = true, Location = new Point(16, cy), BackColor = Color.Transparent
                });
                cy += 22;
                tb          = UIHelper.MakeTextBox(placeholder, innerW, 34, pw);
                tb.Location = new Point(16, cy);
                tb.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                pnlCard.Controls.Add(tb);
                cy += 44;
            }

            // Required fields — Full Name is ALWAYS the first visible field
            AddField("Full Name *",        ref txtFullName,  "Full Name");
            AddField("Username *",         ref txtUsername,  "Username");
            AddField("Email *",            ref txtEmail,     "Email");
            AddField("Password *",         ref txtPassword,  "Password",         pw: true);
            AddField("Confirm Password *", ref txtConfirm,   "Confirm Password", pw: true);

            // Optional fields
            pnlCard.Controls.Add(new Label
            {
                Text = "University (optional)", Font = Theme.FontBold, ForeColor = Theme.TextPrimary,
                AutoSize = true, Location = new Point(16, cy), BackColor = Color.Transparent
            });
            cy += 22;
            txtUniversity          = UIHelper.MakeTextBox("University (optional)", innerW, 34);
            txtUniversity.Location = new Point(16, cy);
            txtUniversity.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUniversity.Tag      = "stretch";
            pnlCard.Controls.Add(txtUniversity);
            cy += 44;

            // Major + Year row
            int majorW = Math.Max(80, innerW - 138);
            txtMajor          = UIHelper.MakeTextBox("Major / Course (optional)", majorW, 34);
            txtMajor.Location = new Point(16, cy);
            txtMajor.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMajor.Tag      = "major";
            pnlCard.Controls.Add(txtMajor);

            cmbYear = new ComboBox
            {
                Location      = new Point(pnlCard.Width - 16 - 130, cy),
                Size          = new Size(130, 34),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = Theme.FontBody,
                FlatStyle     = FlatStyle.Flat,
                Anchor        = AnchorStyles.Top | AnchorStyles.Right,
                Tag           = "year"
            };
            cmbYear.Items.AddRange(new object[] { "Year Level", "Freshman", "Sophomore", "Junior", "Senior", "Graduate" });
            cmbYear.SelectedIndex = 0;
            pnlCard.Controls.Add(cmbYear);
            cy += 44;

            // Contact info field (NEW)
            pnlCard.Controls.Add(new Label
            {
                Text = "Contact Info (optional)", Font = Theme.FontBold, ForeColor = Theme.TextPrimary,
                AutoSize = true, Location = new Point(16, cy), BackColor = Color.Transparent
            });
            cy += 22;
            pnlCard.Controls.Add(new Label
            {
                Text = "Phone number, Facebook, Messenger, etc. — shown to users who contact you.",
                Font = Theme.FontSmall, ForeColor = Theme.TextSecondary,
                AutoSize = false, Size = new Size(innerW, 28),
                Location = new Point(16, cy), BackColor = Color.Transparent,
                Tag = "stretch"
            });
            cy += 30;
            txtContact          = UIHelper.MakeTextBox("e.g. +63 912 345 6789 or fb.com/yourname", innerW, 34);
            txtContact.Location = new Point(16, cy);
            txtContact.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtContact.Tag      = "stretch";
            pnlCard.Controls.Add(txtContact);
            cy += 44;

            // Error label
            lblError = new Label
            {
                Text = "", Font = Theme.FontSmall, ForeColor = Theme.RedText,
                AutoSize = false, Size = new Size(innerW, 20),
                Location = new Point(16, cy),
                Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent, Visible = false, Tag = "error"
            };
            pnlCard.Controls.Add(lblError);
            cy += 26;

            btnRegister          = UIHelper.MakeButton("Create Account", Theme.PrimaryBlue, Color.White, innerW, 42);
            btnRegister.Location = new Point(16, cy);
            btnRegister.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnRegister.Tag      = "stretch";
            btnRegister.Click   += btnRegister_Click;
            pnlCard.Controls.Add(btnRegister);
            cy += 50;

            btnCancel          = UIHelper.MakeButton("Cancel", Color.White, Theme.TextSecondary, innerW, 34);
            btnCancel.Location = new Point(16, cy);
            btnCancel.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnCancel.Tag      = "stretch";
            btnCancel.FlatAppearance.BorderSize  = 1;
            btnCancel.FlatAppearance.BorderColor = Theme.BorderColor;
            btnCancel.Click += btnCancel_Click;
            pnlCard.Controls.Add(btnCancel);
            cy += 42;

            pnlCard.Height = cy + 16;
        }

        private void ResizeCardControls()
        {
            int innerW = pnlCard.Width - 32;
            foreach (Control c in pnlCard.Controls)
            {
                switch (c.Tag?.ToString())
                {
                    case "major":
                        c.Width = Math.Max(80, innerW - 138);
                        break;
                    case "year":
                        c.Left = pnlCard.Width - 16 - 130;
                        break;
                    case "error":
                    case "stretch":
                        c.Width = innerW;
                        break;
                }
                if (c is TextBox tb && tb.Tag?.ToString() is null or "")
                    tb.Width = innerW;
            }
            pnlCard.Invalidate();
        }
    }
}
