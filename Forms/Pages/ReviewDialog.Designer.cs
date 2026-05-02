using TutoringMarketplace.Controls;

namespace TutoringMarketplace.Forms.Pages
{
    partial class ReviewDialog
    {
        private System.ComponentModel.IContainer? components = null;
        private Button[]  _starButtons = null!;
        private TextBox   _txtReview   = null!;
        private Button    _btnSubmit   = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            Text            = "Leave a Review";
            Size            = new Size(420, 340);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = Theme.Background;
            Font            = Theme.FontBody;

            var card = UIHelper.MakeCard(16, 16, 372, 274);
            Controls.Add(card);

            card.Controls.Add(new Label { Text = "Rate Your Experience", Font = Theme.FontH3, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(16, 14), BackColor = Color.Transparent });
            card.Controls.Add(new Label { Text = _serviceTitle, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(16, 38), BackColor = Color.Transparent });

            // star buttons
            _starButtons = new Button[5];
            for (int i = 0; i < 5; i++)
            {
                int stars = i + 1;
                var btn   = new Button
                {
                    Text      = "★",
                    Font      = new Font("Segoe UI", 22f),
                    ForeColor = Theme.StarColor,
                    Size      = new Size(44, 44),
                    Location  = new Point(16 + i * 46, 66),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White,
                    Cursor    = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, e) => SetStars(stars);
                _starButtons[i] = btn;
                card.Controls.Add(btn);
            }

            card.Controls.Add(new Label { Text = "Write a Review (optional)", Font = Theme.FontBold, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(16, 122), BackColor = Color.Transparent });

            _txtReview = new TextBox
            {
                Multiline   = true,
                Size        = new Size(340, 80),
                Location    = new Point(16, 144),
                Font        = Theme.FontBody,
                BorderStyle = BorderStyle.FixedSingle,
                Text        = "Write your review here...",
                ForeColor   = Theme.TextSecondary
            };
            _txtReview.GotFocus  += (s, e) => { if (_txtReview.Text == "Write your review here...") { _txtReview.Text = ""; _txtReview.ForeColor = Theme.TextPrimary; } };
            _txtReview.LostFocus += (s, e) => { if (_txtReview.Text == "") { _txtReview.Text = "Write your review here..."; _txtReview.ForeColor = Theme.TextSecondary; } };
            card.Controls.Add(_txtReview);

            _btnSubmit = UIHelper.MakeButton("Submit Review", Theme.PrimaryBlue, Color.White, 340, 38);
            _btnSubmit.Location = new Point(16, 232);
            _btnSubmit.Click   += OnSubmit;
            card.Controls.Add(_btnSubmit);
        }
    }
}
