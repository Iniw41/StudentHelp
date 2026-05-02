using TutoringMarketplace.Controls;
using TutoringMarketplace.Models;

namespace TutoringMarketplace.Forms
{
    public class ReceiptDialog : Form
    {
        public bool Confirmed { get; private set; }

        private readonly Service _service;
        private readonly Account _tutor;
        private readonly Account _client;
        private readonly string _referenceNumber;
        private readonly decimal _basePrice;
        private readonly decimal _systemFee;
        private readonly decimal _totalAmount;

        public ReceiptDialog(Service service, Account tutor, Account client, string referenceNumber)
        {
            _service = service;
            _tutor = tutor;
            _client = client;
            _referenceNumber = referenceNumber;
            _basePrice = service.Price;
            _systemFee = service.Price * 0.05m;
            _totalAmount = _basePrice + _systemFee;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Booking Receipt";
            Size = new Size(520, 780);
            MinimumSize = new Size(520, 780);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Theme.Background;
            Font = Theme.FontBody;
            AutoScroll = true;

            // Header
            var hdr = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Theme.PrimaryBlue
            };
            hdr.Controls.Add(new Label
            {
                Text = "📋 Booking Receipt",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            });
            Controls.Add(hdr);

            int y = 90;
            int contentWidth = 440;
            int leftX = 20;

            // Reference Number Card
            var refCard = UIHelper.MakeCard(leftX, y, contentWidth, 70);
            refCard.Controls.Add(new Label
            {
                Text = "Reference Number",
                Font = Theme.FontSmall,
                ForeColor = Theme.TextSecondary,
                AutoSize = true,
                Location = new Point(16, 10),
                BackColor = Color.Transparent
            });
            refCard.Controls.Add(new Label
            {
                Text = _referenceNumber,
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = Theme.PrimaryBlue,
                AutoSize = true,
                Location = new Point(16, 32),
                BackColor = Color.Transparent
            });
            Controls.Add(refCard);
            y += 80;

            // Tutor Info Card
            var tutorCard = UIHelper.MakeCard(leftX, y, contentWidth, 140);
            tutorCard.Controls.Add(new Label
            {
                Text = "Tutor Information",
                Font = Theme.FontH3,
                ForeColor = Theme.TextPrimary,
                AutoSize = true,
                Location = new Point(16, 12),
                BackColor = Color.Transparent
            });

            var avatar = UIHelper.MakeAvatar(_tutor.FullName, UIHelper.AvatarColor(_tutor.FullName), 48);
            avatar.Location = new Point(16, 44);
            tutorCard.Controls.Add(avatar);

            tutorCard.Controls.Add(new Label
            {
                Text = _tutor.FullName,
                Font = Theme.FontBold,
                ForeColor = Theme.TextPrimary,
                AutoSize = true,
                Location = new Point(76, 44),
                BackColor = Color.Transparent
            });
            tutorCard.Controls.Add(new Label
            {
                Text = $"{_tutor.Major} · {_tutor.Year}",
                Font = Theme.FontSmall,
                ForeColor = Theme.TextSecondary,
                AutoSize = true,
                Location = new Point(76, 66),
                BackColor = Color.Transparent
            });
            tutorCard.Controls.Add(new Label
            {
                Text = _tutor.University,
                Font = Theme.FontSmall,
                ForeColor = Theme.TextSecondary,
                AutoSize = true,
                Location = new Point(76, 84),
                BackColor = Color.Transparent
            });
            tutorCard.Controls.Add(new Label
            {
                Text = $"{UIHelper.StarString(_tutor.Rating)} {_tutor.Rating:0.0} ({_tutor.CompletedJobs} completed jobs)",
                Font = Theme.FontSmall,
                ForeColor = Theme.StarColor,
                AutoSize = true,
                Location = new Point(76, 104),
                BackColor = Color.Transparent
            });
            Controls.Add(tutorCard);
            y += 150;

            // Service Details Card
            var svcCard = UIHelper.MakeCard(leftX, y, contentWidth, 120);
            svcCard.Controls.Add(new Label
            {
                Text = "Service Details",
                Font = Theme.FontH3,
                ForeColor = Theme.TextPrimary,
                AutoSize = true,
                Location = new Point(16, 12),
                BackColor = Color.Transparent
            });
            svcCard.Controls.Add(new Label
            {
                Text = _service.Title,
                Font = Theme.FontBold,
                ForeColor = Theme.TextPrimary,
                AutoSize = true,
                Location = new Point(16, 38),
                BackColor = Color.Transparent
            });
            svcCard.Controls.Add(new Label
            {
                Text = _service.Description,
                Font = Theme.FontSmall,
                ForeColor = Theme.TextSecondary,
                Size = new Size(contentWidth - 32, 50),
                Location = new Point(16, 62),
                BackColor = Color.Transparent
            });
            Controls.Add(svcCard);
            y += 130;

            // Fee Breakdown Card
            var feeCard = UIHelper.MakeCard(leftX, y, contentWidth, 150);
            feeCard.Controls.Add(new Label
            {
                Text = "Payment Breakdown",
                Font = Theme.FontH3,
                ForeColor = Theme.TextPrimary,
                AutoSize = true,
                Location = new Point(16, 12),
                BackColor = Color.Transparent
            });

            int feeY = 42;
            // Base price
            feeCard.Controls.Add(new Panel
            {
                Location = new Point(16, feeY),
                Size = new Size(contentWidth - 32, 1),
                BackColor = Theme.BorderColor
            });
            feeY += 8;
            feeCard.Controls.Add(new Label
            {
                Text = "Service Price",
                Font = Theme.FontBody,
                ForeColor = Theme.TextSecondary,
                AutoSize = true,
                Location = new Point(16, feeY),
                BackColor = Color.Transparent
            });
            feeCard.Controls.Add(new Label
            {
                Text = $"₱{_basePrice:N2}",
                Font = Theme.FontBold,
                ForeColor = Theme.TextPrimary,
                AutoSize = true,
                Location = new Point(contentWidth - 16 - TextRenderer.MeasureText($"₱{_basePrice:N2}", Theme.FontBold).Width, feeY),
                BackColor = Color.Transparent
            });

            // System fee
            feeY += 28;
            feeCard.Controls.Add(new Label
            {
                Text = "Platform Fee (5%)",
                Font = Theme.FontBody,
                ForeColor = Theme.TextSecondary,
                AutoSize = true,
                Location = new Point(16, feeY),
                BackColor = Color.Transparent
            });
            feeCard.Controls.Add(new Label
            {
                Text = $"₱{_systemFee:N2}",
                Font = Theme.FontBold,
                ForeColor = Theme.TextPrimary,
                AutoSize = true,
                Location = new Point(contentWidth - 16 - TextRenderer.MeasureText($"₱{_systemFee:N2}", Theme.FontBold).Width, feeY),
                BackColor = Color.Transparent
            });

            // Total
            feeY += 32;
            feeCard.Controls.Add(new Panel
            {
                Location = new Point(16, feeY),
                Size = new Size(contentWidth - 32, 2),
                BackColor = Theme.PrimaryBlue
            });
            feeY += 10;
            feeCard.Controls.Add(new Label
            {
                Text = "Total Amount",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Theme.TextPrimary,
                AutoSize = true,
                Location = new Point(16, feeY),
                BackColor = Color.Transparent
            });
            feeCard.Controls.Add(new Label
            {
                Text = $"₱{_totalAmount:N2}",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Theme.GreenText,
                AutoSize = true,
                Location = new Point(contentWidth - 16 - TextRenderer.MeasureText($"₱{_totalAmount:N2}", new Font("Segoe UI", 14f, FontStyle.Bold)).Width, feeY),
                BackColor = Color.Transparent
            });
            Controls.Add(feeCard);
            y += 160;

            // Info note
            var noteCard = UIHelper.MakeCard(leftX, y, contentWidth, 70);
            noteCard.Controls.Add(new Label
            {
                Text = "ℹ️  Keep your reference number safe! You'll need it for:",
                Font = Theme.FontBold,
                ForeColor = Theme.TextPrimary,
                AutoSize = true,
                Location = new Point(16, 10),
                BackColor = Color.Transparent
            });
            noteCard.Controls.Add(new Label
            {
                Text = "• Tracking your booking  • Requesting refunds  • Customer support",
                Font = Theme.FontSmall,
                ForeColor = Theme.TextSecondary,
                AutoSize = true,
                Location = new Point(16, 34),
                BackColor = Color.Transparent
            });
            Controls.Add(noteCard);
            y += 80;

            // Buttons
            var btnCancel = UIHelper.MakeButton("Cancel", Color.White, Theme.RedText, 180, 42);
            btnCancel.Location = new Point(leftX, y);
            btnCancel.Click += (s, e) => { Confirmed = false; DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnCancel);

            var btnConfirm = UIHelper.MakeButton($"Confirm Booking - ₱{_totalAmount:N2}", Theme.GreenText, Color.White, 220, 42);
            btnConfirm.Location = new Point(leftX + 200, y);
            btnConfirm.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            btnConfirm.Click += (s, e) =>
            {
                if (_client.WalletBalance < (double)_totalAmount)
                {
                    MessageBox.Show(
                        $"Insufficient wallet balance.\n\nRequired: ₱{_totalAmount:N2}\nYour balance: ₱{_client.WalletBalance:N2}\n\nPlease top up your wallet from My Dashboard.",
                        "Insufficient Funds",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                Confirmed = true;
                DialogResult = DialogResult.OK;
                Close();
            };
            Controls.Add(btnConfirm);

            // Wallet balance display
            Controls.Add(new Label
            {
                Text = $"Your wallet balance: ₱{_client.WalletBalance:N2}",
                Font = Theme.FontSmall,
                ForeColor = Theme.TextSecondary,
                AutoSize = true,
                Location = new Point(leftX + 10, y + 48),
                BackColor = Color.Transparent
            });
        }
    }
}
