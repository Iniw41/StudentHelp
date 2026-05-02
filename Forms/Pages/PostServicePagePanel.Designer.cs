using TutoringMarketplace.Controls;
using TutoringMarketplace.Data;
using TutoringMarketplace.Database;

namespace TutoringMarketplace.Forms.Pages
{
    partial class PostServicePagePanel
    {
        private TextBox         _txtTitle    = null!;
        private TextBox         _txtPrice    = null!;
        private TextBox         _txtDuration = null!;
        private TextBox         _txtTagInput = null!;
        private TextBox         _txtContact  = null!;   // contact info shown to clients
        private RichTextBox     _rtfDesc     = null!;
        private ComboBox        _catBox      = null!;
        private FlowLayoutPanel _tagFlow     = null!;
        private Label           _lblError    = null!;

        private void InitializeComponent()
        {
            int pw = 1400;

            var backBtn = UIHelper.MakeButton("← Back", Color.White, Theme.PrimaryBlue, 100, 32);
            backBtn.Location = new Point(16, 12); backBtn.FlatAppearance.BorderSize = 1; backBtn.FlatAppearance.BorderColor = Theme.PrimaryBlue;
            backBtn.Click += OnBack;
            Controls.Add(backBtn);

            Controls.Add(new Label { Text = "Offer Your Service", Font = Theme.FontTitle, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(16, 52), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Share your expertise and help fellow students while earning pesos", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(16, 90), BackColor = Color.Transparent });

            int y = 122;

            // ── Basic Info ────────────────────────────────────────────────────
            y = AddCardHeader(y, pw, "Basic Information", "Tell students what you're offering");
            var basicCard = UIHelper.MakeCard(12, y, pw, 10); // height set at end
            Controls.Add(basicCard);
            int cy = 16;

            basicCard.Controls.Add(Lbl("Service Title *", 14, cy)); cy += 22;
            _txtTitle = UIHelper.MakeTextBox("e.g. Python Programming Help", pw - 52, 32);
            _txtTitle.Location = new Point(14, cy); basicCard.Controls.Add(_txtTitle); cy += 40;

            basicCard.Controls.Add(Lbl("Description *", 14, cy)); cy += 22;
            _rtfDesc = new RichTextBox { Location = new Point(14, cy), Size = new Size(pw - 52, 100), Font = Theme.FontBody, BorderStyle = BorderStyle.FixedSingle };
            basicCard.Controls.Add(_rtfDesc); cy += 108;
            basicCard.Controls.Add(SmallLbl("Minimum 30 characters. Be specific about what you offer.", 14, cy)); cy += 20;

            basicCard.Controls.Add(Lbl("Category *", 14, cy)); cy += 22;
            _catBox = new ComboBox { Location = new Point(14, cy), Size = new Size(300, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontBody, FlatStyle = FlatStyle.Flat };
            _catBox.Items.Add("-- Select a category --");
            foreach (var c in MockData.Categories) _catBox.Items.Add(c.Name);
            _catBox.SelectedIndex = 0;
            basicCard.Controls.Add(_catBox); cy += 42;

            basicCard.Controls.Add(Lbl("Tags (up to 5)", 14, cy)); cy += 22;
            _txtTagInput = new TextBox { Location = new Point(14, cy), Size = new Size(220, 28), Font = Theme.FontBody };
            _txtTagInput.KeyPress += (s, e) => { if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; AddTag(); } };
            basicCard.Controls.Add(_txtTagInput);
            var addTagBtn = UIHelper.MakeButton("+ Add", Theme.PrimaryBlue, Color.White, 72, 28);
            addTagBtn.Location = new Point(242, cy); addTagBtn.Click += (s, e) => AddTag();
            basicCard.Controls.Add(addTagBtn); cy += 38;
            _tagFlow = new FlowLayoutPanel { Location = new Point(14, cy), Size = new Size(pw - 52, 30), BackColor = Color.Transparent, AutoSize = true };
            basicCard.Controls.Add(_tagFlow); cy += 36;

            basicCard.Size = new Size(pw, cy + 14);
            y += cy + 20;

            // ── Pricing ───────────────────────────────────────────────────────
            y = AddCardHeader(y, pw, "Pricing & Duration", "Set your peso rate and time commitment");
            var priceCard = UIHelper.MakeCard(12, y, pw, 100);
            Controls.Add(priceCard);

            priceCard.Controls.Add(Lbl("Price per Session (₱) *", 14, 14));
            _txtPrice = new TextBox { Location = new Point(14, 36), Size = new Size(180, 28), Font = Theme.FontBody };
            priceCard.Controls.Add(_txtPrice);
            priceCard.Controls.Add(SmallLbl("Avg student rate: ₱850 – ₱1,500", 14, 68));

            priceCard.Controls.Add(Lbl("Typical Duration *", 214, 14));
            _txtDuration = new TextBox { Location = new Point(214, 36), Size = new Size(220, 28), Font = Theme.FontBody };
            priceCard.Controls.Add(_txtDuration);
            priceCard.Controls.Add(SmallLbl("e.g. 1 hour, 2-3 days", 214, 68));
            y += 110;

            // ── Contact Info ──────────────────────────────────────────────────
            y = AddCardHeader(y, pw, "Your Contact Info", "Let clients know how to reach you");
            var contactCard = UIHelper.MakeCard(12, y, pw, 110);
            Controls.Add(contactCard);

            contactCard.Controls.Add(Lbl("Contact Details (optional)", 14, 14));
            contactCard.Controls.Add(new Label
            {
                Text = "Shown to clients when they tap \"Contact\" on your service — phone, Messenger, Telegram, etc.",
                Font = Theme.FontSmall, ForeColor = Theme.TextSecondary,
                AutoSize = false, Size = new Size(pw - 52, 24),
                Location = new Point(14, 36), BackColor = Color.Transparent
            });

            // Pre-fill with whatever the logged-in user already has saved
            string existingContact = Session.IsLoggedIn
                ? (DatabaseManager.GetAccount(Session.CurrentUser!.Id)?.ContactInfo ?? "")
                : "";
            _txtContact = string.IsNullOrWhiteSpace(existingContact)
                ? UIHelper.MakeTextBox("e.g. +63 912 345 6789  /  fb.com/yourname  /  t.me/yourhandle", pw - 52, 34)
                : new TextBox { Text = existingContact, Font = Theme.FontBody, BorderStyle = BorderStyle.FixedSingle, Size = new Size(pw - 52, 34), ForeColor = Theme.TextPrimary };
            _txtContact.Location = new Point(14, 64);
            contactCard.Controls.Add(_txtContact);
            y += 120;
            y = AddCardHeader(y, pw, "Guidelines for Success", "");
            var guideCard = UIHelper.MakeCard(12, y, pw, 10);
            guideCard.BackColor = Color.FromArgb(239, 246, 255);
            Controls.Add(guideCard);
            cy = 14;
            foreach (var tip in new[] { "Be honest about your skills and experience level", "Set realistic prices in Philippine Peso", "Respond to messages within 24 hours", "Maintain a professional and helpful attitude", "Don't do others' work for them — help them learn" })
            {
                guideCard.Controls.Add(new Label { Text = $"•  {tip}", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(14, cy), BackColor = Color.Transparent });
                cy += 22;
            }
            guideCard.Size = new Size(pw, cy + 14);
            y += cy + 20;

            // ── Error + Buttons ───────────────────────────────────────────────
            _lblError = new Label { Text = "", Font = Theme.FontSmall, ForeColor = Theme.RedText, AutoSize = false, Size = new Size(pw - 24, 18), Location = new Point(16, y), BackColor = Color.Transparent, Visible = false };
            Controls.Add(_lblError);
            y += 22;

            var pubBtn = UIHelper.MakeButton("✔  Publish Service", Theme.PrimaryBlue, Color.White, 180, 40);
            pubBtn.Location = new Point(16, y); pubBtn.Click += OnPublish;
            Controls.Add(pubBtn);

            var cancelBtn = UIHelper.MakeButton("Cancel", Color.White, Theme.TextPrimary, 100, 40);
            cancelBtn.Location = new Point(204, y);
            cancelBtn.FlatAppearance.BorderSize = 1; cancelBtn.FlatAppearance.BorderColor = Theme.BorderColor;
            cancelBtn.Click += OnBack;
            Controls.Add(cancelBtn);
        }

        private int AddCardHeader(int y, int pw, string title, string sub)
        {
            Controls.Add(new Label { Text = title, Font = Theme.FontH3, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(16, y), BackColor = Color.Transparent });
            if (!string.IsNullOrEmpty(sub))
                Controls.Add(new Label { Text = sub, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(16, y + 22), BackColor = Color.Transparent });
            return y + (string.IsNullOrEmpty(sub) ? 28 : 46);
        }

        private static Label Lbl(string t, int x, int y) => new() { Text = t, Font = Theme.FontBold, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(x, y), BackColor = Color.Transparent };
        private static Label SmallLbl(string t, int x, int y) => new() { Text = t, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(x, y), BackColor = Color.Transparent };
    }
}
