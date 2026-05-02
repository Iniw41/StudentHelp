using TutoringMarketplace.Controls;

namespace TutoringMarketplace.Forms
{
    public class WalletTopUpDialog : Form
    {
        public decimal Amount { get; private set; }

        private NumericUpDown _numAmount = null!;
        private Label         _lblError  = null!;

        public WalletTopUpDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text            = "Top Up Wallet";
            Size            = new Size(400, 340);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = Theme.Background;
            Font            = Theme.FontBody;

            // Header
            var hdr = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Theme.GreenText };
            hdr.Controls.Add(new Label
            {
                Text = "💳  Add Funds to Your Wallet",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White, BackColor = Color.Transparent,
                Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter
            });
            Controls.Add(hdr);

            // Card
            var card = UIHelper.MakeCard(20, 80, 340, 200);
            Controls.Add(card);

            card.Controls.Add(new Label { Text = "Amount to Add (₱)", Font = Theme.FontBold, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(16, 18), BackColor = Color.Transparent });
            card.Controls.Add(new Label { Text = $"Current balance: ₱{Session.CurrentUser?.WalletBalance:N2}", Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(16, 42), BackColor = Color.Transparent });

            _numAmount = new NumericUpDown
            {
                Location    = new Point(16, 66),
                Size        = new Size(308, 34),
                Font        = new Font("Segoe UI", 13f),
                Minimum     = 100,
                Maximum     = 50000,
                Value       = 500,
                Increment   = 100,
                DecimalPlaces = 2,
                ThousandsSeparator = true
            };
            card.Controls.Add(_numAmount);

            // Quick amounts
            int qx = 16;
            foreach (var amt in new[] { 500, 1000, 2000, 5000 })
            {
                int a = amt;
                var qb = UIHelper.MakeButton($"₱{a:N0}", Theme.LightBlue, Theme.PrimaryBlue, 68, 28);
                qb.Location = new Point(qx, 112);
                qb.Click += (s, e) => _numAmount.Value = a;
                card.Controls.Add(qb);
                qx += 76;
            }

            _lblError = new Label { Text = "", Font = Theme.FontSmall, ForeColor = Theme.RedText, AutoSize = false, Size = new Size(308, 20), Location = new Point(16, 148), BackColor = Color.Transparent, Visible = false };
            card.Controls.Add(_lblError);

            var btnAdd = UIHelper.MakeButton("Add Funds", Theme.GreenText, Color.White, 308, 38);
            btnAdd.Location = new Point(16, 150);
            btnAdd.Click += (s, e) =>
            {
                Amount = _numAmount.Value;
                DialogResult = DialogResult.OK;
                Close();
            };
            card.Controls.Add(btnAdd);

            var btnCancel = new Button
            {
                Text = "Cancel", Size = new Size(340, 30),
                Location = new Point(20, 292), FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent, ForeColor = Theme.TextSecondary,
                Font = Theme.FontSmall
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnCancel);
        }
    }
}
