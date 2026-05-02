using TutoringMarketplace.Controls;
using TutoringMarketplace.Database;
using TutoringMarketplace.Models;

namespace TutoringMarketplace.Forms.Pages
{
    public partial class PostServicePagePanel : Panel
    {
        public event EventHandler? BackRequested;
        public event EventHandler? ServicePosted;

        private List<string> _tags = new();

        public PostServicePagePanel()
        {
            BackColor  = Theme.Background;
            AutoScroll = true;
            InitializeComponent();
        }

        private void OnBack(object? s, EventArgs e) => BackRequested?.Invoke(this, EventArgs.Empty);

        private void AddTag()
        {
            var t = _txtTagInput.Text.Trim();
            if (t == "" || _tags.Count >= 5 || _tags.Contains(t)) return;
            _tags.Add(t);
            _txtTagInput.Clear();
            RefreshTags();
        }

        private void RefreshTags()
        {
            _tagFlow.Controls.Clear();
            foreach (var tag in _tags)
            {
                var pnl = new Panel
                {
                    Size   = new Size(TextRenderer.MeasureText(tag, Theme.FontSmall).Width + 40, 24),
                    Margin = new Padding(2, 0, 2, 0)
                };
                pnl.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using var br   = new SolidBrush(Theme.LightBlue);
                    using var path = UIHelper.RoundRect(new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1), 10);
                    e.Graphics.FillPath(br, path);
                };
                pnl.Controls.Add(new Label { Text = tag, Font = Theme.FontSmall, ForeColor = Theme.PrimaryBlue, AutoSize = true, Location = new Point(6, 4), BackColor = Color.Transparent });
                var tagCopy = tag;
                var x = new Label { Text = "✕", Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(pnl.Width - 18, 4), BackColor = Color.Transparent, Cursor = Cursors.Hand };
                x.Click += (s, e) => { _tags.Remove(tagCopy); RefreshTags(); };
                pnl.Controls.Add(x);
                _tagFlow.Controls.Add(pnl);
            }
        }

        private void OnPublish(object? s, EventArgs e)
        {
            string title    = _txtTitle.Text.Trim();
            string desc     = _rtfDesc.Text.Trim();
            string priceStr = _txtPrice.Text.Trim();
            string duration = _txtDuration.Text.Trim();

            if (title    == "" || title    == "e.g. Python Programming Help")     { ShowError("Please enter a service title.");       return; }
            if (desc     == "" || desc.Length < 30)                               { ShowError("Description must be at least 30 characters."); return; }
            if (_catBox.SelectedIndex == 0)                                       { ShowError("Please select a category.");            return; }
            if (priceStr == "" || !decimal.TryParse(priceStr, out var price))     { ShowError("Please enter a valid price.");          return; }
            if (price < 100 || price > 50000)                                     { ShowError("Price must be between ₱100 and ₱50,000."); return; }
            if (duration == "" || duration == "e.g. 1 hour, 2-3 days")           { ShowError("Please enter a duration.");             return; }

            _lblError.Visible = false;

            var svc = new Service
            {
                Title       = title,
                Description = desc,
                Category    = Data.MockData.Categories[_catBox.SelectedIndex - 1].Id,
                Price       = price,
                Duration    = duration,
                TutorId     = Session.CurrentUser!.Id,
                Tags        = string.Join(",", _tags)
            };

            DatabaseManager.CreateService(svc);

            // Save contact info if the user typed something new
            string contact = _txtContact.Text.Trim();
            bool isPlaceholder = _txtContact.ForeColor == Theme.TextSecondary;
            if (!isPlaceholder && !string.IsNullOrWhiteSpace(contact))
                DatabaseManager.UpdateContactInfo(Session.CurrentUser!.Id, contact);

            MessageBox.Show($"Service \"{title}\" published successfully! 🎉", "Published", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ServicePosted?.Invoke(this, EventArgs.Empty);
        }

        private void ShowError(string msg) { _lblError.Text = msg; _lblError.Visible = true; }
    }
}
