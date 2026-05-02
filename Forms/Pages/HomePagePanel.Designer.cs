using TutoringMarketplace.Controls;
using TutoringMarketplace.Database;
using TutoringMarketplace.Data;

namespace TutoringMarketplace.Forms.Pages
{
    partial class HomePagePanel
    {
        private void InitializeComponent()
        {
            // ── SCROLL CONTAINER ──────────────────────────────────────────────
            // Every child section is parented to this single panel.
            // The outer HomePagePanel (AutoScroll=true, Dock=Fill) scrolls
            // the container as one unit, so the navbar can never overlap
            // anything below it and no two sections can bleed into each other.
            int pw = 1880;  // content width (1920 window minus scrollbar gutter)
            var container = new Panel
            {
                Location  = new Point(0, 0),
                Size      = new Size(pw, 10),   // height is finalised at the bottom
                BackColor = Theme.Background,
            };

            // ── HERO ─────────────────────────────────────────────────────────
            var hero = new Panel { Location = new Point(0, 0), Size = new Size(pw, 280), BackColor = Theme.PrimaryBlue };
            hero.Paint += (s, e) =>
            {
                using var br = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Point(0, 0), new Point(pw, 280), Theme.PrimaryBlue, Theme.DarkBlue);
                e.Graphics.FillRectangle(br, hero.ClientRectangle);
            };
            hero.Controls.Add(new Label { Text = "Student-to-Student Help Made Easy", Font = new Font("Segoe UI",24f,FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Transparent, AutoSize = false, Size = new Size(pw,52), Location = new Point(0,26), TextAlign = ContentAlignment.MiddleCenter });
            hero.Controls.Add(new Label { Text = "Find affordable tutoring and project assistance from fellow students in the Philippines.", Font = new Font("Segoe UI",11f), ForeColor = Color.FromArgb(219,234,254), BackColor = Color.Transparent, AutoSize = false, Size = new Size(pw,28), Location = new Point(0,84), TextAlign = ContentAlignment.MiddleCenter });

            var searchBox = UIHelper.MakeTextBox("What do you need help with?", 500, 38);
            searchBox.Location = new Point((pw - 650) / 2, 130);
            searchBox.ForeColor = Theme.TextPrimary;
            var searchBtn = UIHelper.MakeButton("Search", Theme.DarkBlue, Color.White, 140, 38);
            searchBtn.Location = new Point((pw - 650) / 2 + 510, 130);
            searchBtn.Click += OnSearchClick;
            hero.Controls.Add(searchBox);
            hero.Controls.Add(searchBtn);

            var popularLbl = new Label { Text = "Popular:", Font = Theme.FontSmall, ForeColor = Color.FromArgb(147,197,253), BackColor = Color.Transparent, AutoSize = true, Location = new Point((pw-580)/2, 188) };
            hero.Controls.Add(popularLbl);
            int tx = (pw - 580) / 2 + 58;
            foreach (var term in new[] { "Python", "Calculus", "Essay Writing", "Web Design" })
            {
                var tag = new Label { Text = term, Font = Theme.FontSmall, ForeColor = Color.White, BackColor = Color.FromArgb(60,255,255,255), AutoSize = false, Size = new Size(TextRenderer.MeasureText(term, Theme.FontSmall).Width + 18, 24), Location = new Point(tx, 186), TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand };
                tag.Click += OnSearchClick;
                hero.Controls.Add(tag);
                tx += tag.Width + 8;
            }
            container.Controls.Add(hero);
            int y = 288;    // 8 px gap below hero

            // ── CATEGORIES ───────────────────────────────────────────────────
            y = AddSection(container, y, pw, "Browse by Category", "Find the perfect service for your needs");
            var catFlow = new FlowLayoutPanel { Location = new Point(16, y), Size = new Size(pw - 32, 120), BackColor = Color.Transparent, WrapContents = false };
            foreach (var cat in MockData.Categories)
            {
                var cp = new Panel { Size = new Size(120, 106), Margin = new Padding(3), BackColor = Theme.CardBg, Cursor = Cursors.Hand };
                cp.Paint += (s, e) => { e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; using var pen = new Pen(Theme.BorderColor); using var path = UIHelper.RoundRect(new Rectangle(0,0,cp.Width-1,cp.Height-1),8); e.Graphics.DrawPath(pen,path); };
                cp.Controls.Add(new Label { Text = cat.Icon, Font = new Font("Segoe UI Emoji",20f), AutoSize = false, Size = new Size(120,50), Location = new Point(0,10), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                cp.Controls.Add(new Label { Text = cat.Name, Font = Theme.FontSmall, ForeColor = Theme.TextPrimary, AutoSize = false, Size = new Size(120,38), Location = new Point(0,60), TextAlign = ContentAlignment.TopCenter, BackColor = Color.Transparent });
                var catId = cat.Id;
                foreach (Control c in cp.Controls) c.Click += (s, e) => OnCategoryClick(catId);
                cp.Click += (s, e) => OnCategoryClick(catId);
                catFlow.Controls.Add(cp);
            }
            container.Controls.Add(catFlow);
            y += 128;

            // ── FEATURED SERVICES ────────────────────────────────────────────
            y = AddSection(container, y, pw, "Featured Services", "Top-rated help from talented students");
            var svcFlow = new FlowLayoutPanel { Location = new Point(16, y), Size = new Size(pw - 32, 310), BackColor = Color.Transparent, WrapContents = false };
            var featured = DatabaseManager.GetServices(sortBy: "rating").Take(4).ToList();
            foreach (var svc in featured)
            {
                var card = new ServiceCard(svc);
                card.ServiceClicked += (s, id) => OnServiceClick(id);
                card.ProfileClicked += (s, id) => OnProfileClick(id);
                svcFlow.Controls.Add(card);
            }
            container.Controls.Add(svcFlow);
            y += 316;

            // ── HOW IT WORKS ─────────────────────────────────────────────────
            y = AddSection(container, y, pw, "How It Works", "Get started in three simple steps", Color.FromArgb(243,244,246));
            var howFlow = new FlowLayoutPanel { Location = new Point(16, y), Size = new Size(pw - 32, 160), BackColor = Color.Transparent, WrapContents = false };
            foreach (var (num, title, desc) in new[] { ("1","Browse Services","Search services from verified students"), ("2","Connect & Book","Contact and schedule a session"), ("3","Get Help & Learn","Receive quality help and succeed") })
            {
                var sp = new Panel { Size = new Size(320, 150), Margin = new Padding(4), BackColor = Color.Transparent };
                var circle = new Label { Size = new Size(50,50), Location = new Point(135,0), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent };
                circle.Paint += (s, e) => { e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; e.Graphics.FillEllipse(new SolidBrush(Theme.PrimaryBlue),0,0,49,49); e.Graphics.DrawString(num,new Font("Segoe UI",16f,FontStyle.Bold),Brushes.White,new RectangleF(0,0,50,50),new StringFormat{Alignment=StringAlignment.Center,LineAlignment=StringAlignment.Center}); };
                sp.Controls.Add(circle);
                sp.Controls.Add(new Label { Text = title, Font = Theme.FontH3, ForeColor = Theme.TextPrimary, AutoSize = false, Size = new Size(320,24), Location = new Point(0,58), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                sp.Controls.Add(new Label { Text = desc,  Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = false, Size = new Size(300,40), Location = new Point(10,84), TextAlign = ContentAlignment.TopCenter, BackColor = Color.Transparent });
                howFlow.Controls.Add(sp);
            }
            container.Controls.Add(howFlow);
            y += 168;

            // ── WHY STUDENTHELP ──────────────────────────────────────────────
            y = AddSection(container, y, pw, "Why StudentHelp PH?", "Built for Filipino students");
            var whyFlow = new FlowLayoutPanel { Location = new Point(16, y), Size = new Size(pw - 32, 190), BackColor = Color.Transparent, WrapContents = false };
            foreach (var (icon, ttl, dsc, bg, fg) in new[]
            {
                ("💰","Affordable Rates","Prices in Philippine Peso starting at ₱850/session.", Theme.GreenLight, Theme.GreenText),
                ("🛡","Verified Students","All helpers are verified students from PH universities.", Theme.PurpleLight, Theme.PurpleText),
                ("⚡","Quick Response","Get responses within hours from your fellow students.", Theme.OrangeLight, Theme.OrangeText),
            })
            {
                var wp = UIHelper.MakeCard(0,0,320,178); wp.Margin = new Padding(4);
                wp.Controls.Add(new Label { Text = icon, Font = new Font("Segoe UI Emoji",20f), AutoSize = false, Size = new Size(320,50), Location = new Point(0,14), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                wp.Controls.Add(new Label { Text = ttl, Font = Theme.FontH3, ForeColor = Theme.TextPrimary, AutoSize = false, Size = new Size(300,24), Location = new Point(10,70), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                wp.Controls.Add(new Label { Text = dsc, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = false, Size = new Size(300,52), Location = new Point(10,98), TextAlign = ContentAlignment.TopCenter, BackColor = Color.Transparent });
                whyFlow.Controls.Add(wp);
            }
            container.Controls.Add(whyFlow);
            y += 196;

            // ── CTA ──────────────────────────────────────────────────────────
            var cta = new Panel { Location = new Point(0,y), Size = new Size(pw,150), BackColor = Theme.PrimaryBlue };
            cta.Controls.Add(new Label { Text = "Ready to Get Started?", Font = new Font("Segoe UI",20f,FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Transparent, AutoSize = false, Size = new Size(pw,40), Location = new Point(0,20), TextAlign = ContentAlignment.MiddleCenter });
            cta.Controls.Add(new Label { Text = "Join thousands of Filipino students helping each other succeed", Font = new Font("Segoe UI",11f), ForeColor = Color.FromArgb(219,234,254), BackColor = Color.Transparent, AutoSize = false, Size = new Size(pw,28), Location = new Point(0,62), TextAlign = ContentAlignment.MiddleCenter });
            var findBtn = UIHelper.MakeButton("Find Help Now", Color.White, Theme.PrimaryBlue, 150, 38);
            findBtn.Location = new Point(pw/2-165, 100); findBtn.Click += OnBrowseClick;
            var offerBtn = UIHelper.MakeButton("Offer Services", Color.Transparent, Color.White, 150, 38);
            offerBtn.Location = new Point(pw/2+15, 100);
            offerBtn.FlatAppearance.BorderSize = 1;
            offerBtn.FlatAppearance.BorderColor = Color.White;
            offerBtn.Click += OnPostClick;
            cta.Controls.Add(findBtn);
            cta.Controls.Add(offerBtn);
            container.Controls.Add(cta);
            y += 150;

            // ── FINALISE CONTAINER HEIGHT & ADD TO OUTER PANEL ───────────────
            // Setting the explicit height tells AutoScroll the full scrollable extent.
            container.Size = new Size(pw, y);
            Controls.Add(container);
        }

        // Adds a section header into the given container panel and returns the
        // next Y offset (header block is always 74 px tall).
        private int AddSection(Panel container, int y, int pw, string title, string sub, Color? bg = null)
        {
            var sec = new Panel { Location = new Point(0,y), Size = new Size(pw,68), BackColor = bg ?? Theme.Background };
            sec.Controls.Add(new Label { Text = title, Font = Theme.FontH2, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(16,12), BackColor = Color.Transparent });
            if (!string.IsNullOrEmpty(sub))
                sec.Controls.Add(new Label { Text = sub, Font = Theme.FontBody, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(16,40), BackColor = Color.Transparent });
            container.Controls.Add(sec);
            return y + 74;
        }
    }
}
