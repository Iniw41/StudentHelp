using TutoringMarketplace.Models;
using TutoringMarketplace.Controls;

namespace TutoringMarketplace.Controls
{
    public class ServiceCard : Panel
    {
        public event EventHandler<int>? ServiceClicked;
        public event EventHandler<int>? ProfileClicked;

        private readonly Service _service;

        private static readonly Dictionary<string, (Color bg, Color fg)> CatColors = new()
        {
            ["coding"]    = (Color.FromArgb(219,234,254), Color.FromArgb(29,78,216)),
            ["math"]      = (Color.FromArgb(220,252,231), Color.FromArgb(21,128,61)),
            ["writing"]   = (Color.FromArgb(255,237,213), Color.FromArgb(194,65,12)),
            ["design"]    = (Color.FromArgb(237,233,254), Color.FromArgb(109,40,217)),
            ["tutoring"]  = (Color.FromArgb(254,243,199), Color.FromArgb(146,64,14)),
            ["languages"] = (Color.FromArgb(204,251,241), Color.FromArgb(15,118,110)),
            ["business"]  = (Color.FromArgb(254,226,226), Color.FromArgb(185,28,28)),
            ["other"]     = (Color.FromArgb(243,244,246), Color.FromArgb(75,85,99)),
        };

        public ServiceCard(Service service)
        {
            _service = service;
            Size     = new Size(240, 295);
            BackColor = Theme.CardBg;
            Cursor   = Cursors.Hand;
            Margin   = new Padding(6);
            Build();
        }

        private void Build()
        {
            var (catBg, catFg) = CatColors.TryGetValue(_service.Category, out var cc)
                ? cc : (Color.FromArgb(243,244,246), Theme.TextSecondary);

            // colour header
            var hdr = new Panel { Location = new Point(0, 0), Size = new Size(Width, 68), BackColor = catBg };
            hdr.Controls.Add(new Label { Text = _service.Category.ToUpper(), Font = new Font("Segoe UI",8f,FontStyle.Bold), ForeColor = catFg, AutoSize = true, Location = new Point(12,8), BackColor = Color.Transparent });
            hdr.Controls.Add(new Label { Text = UIHelper.FormatPeso(_service.Price) + "/session", Font = new Font("Segoe UI",14f,FontStyle.Bold), ForeColor = catFg, AutoSize = true, Location = new Point(12,24), BackColor = Color.Transparent });
            Controls.Add(hdr);

            int y = 76;
            Controls.Add(new Label { Text = _service.Title, Font = Theme.FontBold, ForeColor = Theme.TextPrimary, Location = new Point(12,y), Size = new Size(Width-24,50), BackColor = Color.Transparent });
            y += 54;

            // rating
            var rp = new Panel { Location = new Point(12,y), Size = new Size(Width-24,22), BackColor = Color.Transparent };
            rp.Controls.Add(new Label { Text = UIHelper.StarString(_service.Rating), Font = new Font("Segoe UI",11f), ForeColor = Theme.StarColor, AutoSize = true, Location = new Point(0,0), BackColor = Color.Transparent });
            rp.Controls.Add(new Label { Text = $"{_service.Rating:0.0} ({_service.ReviewCount})", Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(92,3), BackColor = Color.Transparent });
            Controls.Add(rp);
            y += 26;

            Controls.Add(new Label { Text = $"⏱  {_service.Duration}", Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(12,y), BackColor = Color.Transparent });
            y += 24;

            Controls.Add(new Panel { Location = new Point(12,y), Size = new Size(Width-24,1), BackColor = Theme.BorderColor });
            y += 8;

            // tutor
            var av = UIHelper.MakeAvatar(_service.TutorName, UIHelper.AvatarColor(_service.TutorName), 34);
            av.Location = new Point(12, y);
            Controls.Add(av);

            var nameLbl = new Label { Text = _service.TutorName, Font = Theme.FontBold, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(54,y+2), BackColor = Color.Transparent, Cursor = Cursors.Hand };
            nameLbl.Click += (s, e) => ProfileClicked?.Invoke(this, _service.TutorId);
            Controls.Add(nameLbl);
            Controls.Add(new Label { Text = $"{_service.TutorMajor} · {_service.TutorYear}", Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(54,y+20), BackColor = Color.Transparent });
            y += 44;

            // view button
            var btn = UIHelper.MakeButton("View Details", Theme.PrimaryBlue, Color.White, Width-24, 34);
            btn.Location = new Point(12, y);
            btn.Click += (s, e) => ServiceClicked?.Invoke(this, _service.Id);
            Controls.Add(btn);

            foreach (Control c in Controls)
                if (c != btn && c != nameLbl) c.Click += (s, e) => ServiceClicked?.Invoke(this, _service.Id);
            Click += (s, e) => ServiceClicked?.Invoke(this, _service.Id);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var pen  = new Pen(Theme.BorderColor, 1);
            using var path = UIHelper.RoundRect(new Rectangle(0, 0, Width-1, Height-1), 10);
            e.Graphics.DrawPath(pen, path);
        }
    }
}
