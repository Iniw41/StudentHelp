using TutoringMarketplace.Controls;
using TutoringMarketplace.Database;

namespace TutoringMarketplace.Forms.Pages
{
    partial class ProfilePagePanel
    {
        private void InitializeComponent()
        {
            var student = DatabaseManager.GetAccount(_studentId);
            if (student == null)
            {
                Controls.Add(new Label { Text = "Student not found.", Font = Theme.FontH2, ForeColor = Theme.TextSecondary, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
                return;
            }

            int pw = 1860;

            // Back
            var backBtn = UIHelper.MakeButton("← Back", Color.White, Theme.PrimaryBlue, 100, 32);
            backBtn.Location = new Point(16, 12); backBtn.FlatAppearance.BorderSize = 1; backBtn.FlatAppearance.BorderColor = Theme.PrimaryBlue;
            backBtn.Click += OnBack;
            Controls.Add(backBtn);

            // Banner
            var banner = new Panel { Location = new Point(0, 50), Size = new Size(pw, 110), BackColor = Theme.PrimaryBlue };
            banner.Paint += (s, e) => { using var br = new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0,0), new Point(pw,0), Theme.PrimaryBlue, Theme.DarkBlue); e.Graphics.FillRectangle(br, banner.ClientRectangle); };
            Controls.Add(banner);

            // Profile card
            var profCard = UIHelper.MakeCard(16, 120, pw - 32, 202);
            var av = UIHelper.MakeAvatar(student.FullName, UIHelper.AvatarColor(student.FullName), 80);
            av.Location = new Point(24, 16);
            profCard.Controls.Add(av);
            profCard.Controls.Add(new Label { Text = student.FullName, Font = new Font("Segoe UI",18f,FontStyle.Bold), ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(122,14), BackColor = Color.Transparent });
            profCard.Controls.Add(new Label { Text = $"{student.Year}  •  {student.Major}", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(122,44), BackColor = Color.Transparent });
            profCard.Controls.Add(new Label { Text = $"🏛  {student.University}", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(122,64), BackColor = Color.Transparent });
            profCard.Controls.Add(new Label { Text = $"{UIHelper.StarString(student.Rating)}  {student.Rating:0.0}  ({student.CompletedJobs} reviews)", Font = Theme.FontBody, ForeColor = Theme.StarColor, AutoSize = true, Location = new Point(122,86), BackColor = Color.Transparent });

            var contactBtn = UIHelper.MakeButton("💬  Contact", Theme.PrimaryBlue, Color.White, 130, 34);
            contactBtn.Location = new Point(pw - 330, 30); contactBtn.Click += OnContact;
            profCard.Controls.Add(contactBtn);
            profCard.Controls.Add(new Label { Text = student.Bio, Font = Theme.FontBody, ForeColor = Theme.TextSecondary, Size = new Size(pw - 160, 34), Location = new Point(24, 138), BackColor = Color.Transparent });

            // Skills
            int sx = 24;
            foreach (var skill in student.Skills.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()))
            {
                var badge = UIHelper.MakeBadge(skill, Theme.LightBlue, Theme.PrimaryBlue);
                badge.Location = new Point(sx, 178); profCard.Controls.Add(badge); sx += badge.Width + 6;
            }
            Controls.Add(profCard);
            int y = 334;

            // Stats
            var statsData = new[] { ("🏅", student.CompletedJobs.ToString(), "Completed Jobs", Theme.LightBlue, Theme.PrimaryBlue), ("📈","98%","Success Rate",Theme.GreenLight,Theme.GreenText), ("⏱",student.ResponseTime,"Response Time",Theme.PurpleLight,Theme.PurpleText), ("📅","2+ years","On Platform",Theme.OrangeLight,Theme.OrangeText) };
            int stX = 16, stW = (pw - 32 - 12) / 4;
            foreach (var (icon, val, lbl, bg, fg) in statsData)
            {
                var sc = UIHelper.MakeCard(stX, y, stW - 4, 108);
                sc.Controls.Add(new Label { Text = icon, Font = new Font("Segoe UI Emoji",18f), AutoSize = false, Size = new Size(stW,44), Location = new Point(0,8), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                sc.Controls.Add(new Label { Text = val, Font = new Font("Segoe UI",15f,FontStyle.Bold), ForeColor = Theme.TextPrimary, AutoSize = false, Size = new Size(stW,28), Location = new Point(0,54), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                sc.Controls.Add(new Label { Text = lbl, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = false, Size = new Size(stW,22), Location = new Point(0,82), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                Controls.Add(sc); stX += stW;
            }
            y += 120;

            // Tabs
            var tabBar = new Panel { Location = new Point(16, y), Size = new Size(pw - 32, 38), BackColor = Color.White };
            tabBar.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Theme.BorderColor), 0, 37, tabBar.Width, 37);
            Controls.Add(tabBar);
            y += 40;

            var studentServices = DatabaseManager.GetServicesByTutor(_studentId);
            var studentJobs     = DatabaseManager.GetJobsCreatedByTutor(_studentId);

            var tabServices = new Panel { Location = new Point(16,y), Size = new Size(pw-32,1200), BackColor = Color.Transparent, Visible = true };
            var tabReviews  = new Panel { Location = new Point(16,y), Size = new Size(pw-32,800),  BackColor = Color.Transparent, Visible = false };
            var tabAbout    = new Panel { Location = new Point(16,y), Size = new Size(pw-32,600),  BackColor = Color.Transparent, Visible = false };
            Controls.AddRange(new Control[] { tabServices, tabReviews, tabAbout });

            Button? activeBtn = null;
            void SetTab(Panel tab, Button btn)
            {
                tabServices.Visible = tabReviews.Visible = tabAbout.Visible = false;
                tab.Visible = true;
                if (activeBtn != null) { activeBtn.ForeColor = Theme.TextSecondary; activeBtn.Font = new Font("Segoe UI",11f); }
                btn.ForeColor = Theme.PrimaryBlue; btn.Font = Theme.FontNav; activeBtn = btn;
            }

            int tbx = 0;
            foreach (var (tabLbl, tab) in new[] { ($"Services ({studentServices.Count})", tabServices), ($"Reviews ({studentJobs.Count(j=>j.Rating.HasValue)})", tabReviews), ("About", tabAbout) })
            {
                int w = TextRenderer.MeasureText(tabLbl, Theme.FontNav).Width + 24;
                var btn = new Button { Text = tabLbl, Location = new Point(tbx,0), Size = new Size(w,38), Font = new Font("Segoe UI",11f), ForeColor = Theme.TextSecondary, BackColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
                btn.FlatAppearance.BorderSize = 0;
                var tabCopy = tab; var btnCopy = btn;
                btn.Click += (s, e) => SetTab(tabCopy, btnCopy);
                tabBar.Controls.Add(btn); tbx += w;
                if (tab == tabServices) SetTab(tab, btn);
            }

            // ── Services tab ──────────────────────────────────────────────────
            var svcFlow = new FlowLayoutPanel { Location = new Point(0,8), Size = new Size(pw-40,1100), BackColor = Color.Transparent, WrapContents = true, AutoSize = true };
            if (studentServices.Count == 0)
                svcFlow.Controls.Add(new Label { Text = "No services offered yet.", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, AutoSize = true });
            else
                foreach (var svc in studentServices) { var card = new ServiceCard(svc); card.ServiceClicked += (s,id) => OnServiceClick(id); svcFlow.Controls.Add(card); }
            tabServices.Controls.Add(svcFlow);

            // ── Reviews tab ───────────────────────────────────────────────────
            var revCard = UIHelper.MakeCard(0, 8, pw-36, 50);
            int rvy = 14;
            var reviewedJobs = studentJobs.Where(j => j.Rating.HasValue).ToList();
            if (!reviewedJobs.Any())
                revCard.Controls.Add(new Label { Text = "No reviews yet.", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(14, 14), BackColor = Color.Transparent });
            else
                foreach (var job in reviewedJobs)
                {
                    var avR = UIHelper.MakeAvatar(job.ClientName, UIHelper.AvatarColor(job.ClientName), 36);
                    avR.Location = new Point(14, rvy); revCard.Controls.Add(avR);
                    revCard.Controls.Add(new Label { Text = job.ClientName, Font = Theme.FontBold, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(58,rvy+2), BackColor = Color.Transparent });
                    revCard.Controls.Add(new Label { Text = job.ServiceTitle, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(58,rvy+20), BackColor = Color.Transparent });
                    revCard.Controls.Add(new Label { Text = UIHelper.StarString(job.Rating!.Value), Font = new Font("Segoe UI",11f), ForeColor = Theme.StarColor, AutoSize = true, Location = new Point(58,rvy+36), BackColor = Color.Transparent });

                    // Review text with dynamic height based on content
                    var reviewLabel = new Label { Text = job.Review, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, Location = new Point(58, rvy+54), BackColor = Color.Transparent, AutoSize = false };
                    int reviewWidth = pw - 130;
                    int reviewHeight = TextRenderer.MeasureText(job.Review, Theme.FontSmall, new Size(reviewWidth, int.MaxValue), TextFormatFlags.WordBreak).Height;
                    reviewLabel.Size = new Size(reviewWidth, Math.Max(reviewHeight, 36));
                    revCard.Controls.Add(reviewLabel);

                    revCard.Controls.Add(new Panel { Location = new Point(14,rvy+54+reviewLabel.Height+10), Size = new Size(pw-80,1), BackColor = Theme.BorderColor });
                    rvy += 54 + reviewLabel.Height + 10 + 20;
                }
            revCard.Size = new Size(pw-36, Math.Max(rvy + 14, 60));
            revCard.AutoScroll = true;
            tabReviews.Controls.Add(revCard);

            // ── About tab ─────────────────────────────────────────────────────
            var aboutCard = UIHelper.MakeCard(0, 8, pw-36, 460);
            int aby = 14;
            void Section(string ttl, Action<int> body) { aboutCard.Controls.Add(new Label { Text = ttl, Font = Theme.FontH3, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(14,aby), BackColor = Color.Transparent }); aby += 28; body(aby); aby += 10; }

            Section($"About {student.FullName.Split(' ')[0]}", start =>
            {
                aboutCard.Controls.Add(new Label { Text = student.Bio, Font = Theme.FontBody, ForeColor = Theme.TextSecondary, Size = new Size(pw-80,38), Location = new Point(14,start), BackColor = Color.Transparent }); aby = start + 44;
                aboutCard.Controls.Add(new Label { Text = "Helping fellow students for over 2 years. My goal is to help you truly understand the material.", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, Size = new Size(pw-80,38), Location = new Point(14,aby), BackColor = Color.Transparent }); aby += 44;
            });
            Section("Education", start =>
            {
                aboutCard.Controls.Add(new Label { Text = $"🎓  {student.Major}", Font = Theme.FontBold, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(14,start), BackColor = Color.Transparent }); aby = start + 22;
                aboutCard.Controls.Add(new Label { Text = $"    {student.University}  —  {student.Year}", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(14,aby), BackColor = Color.Transparent }); aby += 28;
            });
            Section("Skills", start =>
            {
                var sf = new FlowLayoutPanel { Location = new Point(14,start), Size = new Size(pw-80,36), BackColor = Color.Transparent };
                foreach (var sk in student.Skills.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim())) sf.Controls.Add(UIHelper.MakeBadge(sk, Theme.LightBlue, Theme.PrimaryBlue));
                aboutCard.Controls.Add(sf); aby = start + 42;
            });
            Section("Achievements", start =>
            {
                int ai = start;
                foreach (var ach in new[] { "⭐  Top Rated Tutor — 2025", "⭐  Dean's List — Last 3 Semesters", "⭐  98% Student Satisfaction Rate" })
                { aboutCard.Controls.Add(new Label { Text = ach, Font = Theme.FontBody, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(14,ai), BackColor = Color.Transparent }); ai += 22; }
                aby = ai;
            });
            aboutCard.Size = new Size(pw-36, aby + 14);
            tabAbout.Controls.Add(aboutCard);
        }
    }
}
