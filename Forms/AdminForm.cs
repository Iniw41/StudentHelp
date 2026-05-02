using System.Drawing.Drawing2D;
using TutoringMarketplace.Controls;
using TutoringMarketplace.Database;
using TutoringMarketplace.Models;

namespace TutoringMarketplace.Forms
{
    public class AdminForm : Form
    {
        private Panel _sidebar     = null!;
        private Panel _contentArea = null!;
        private Button _btnEarnings= null!;
        private Button _btnServices= null!;
        private Button _btnJobs    = null!;
        private string _currentPage = "earnings";

        public AdminForm()
        {
            InitializeLayout();
            ShowPage("earnings");
        }

        private void InitializeLayout()
        {
            Text            = "StudentHelp — Admin Panel";
            Size            = new Size(1200, 800);
            MinimumSize     = new Size(900, 600);
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = Color.FromArgb(15, 23, 42);
            Font            = Theme.FontBody;

            // Top bar
            var topBar = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Color.FromArgb(15, 23, 42) };
            topBar.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(51, 65, 85), 1);
                e.Graphics.DrawLine(pen, 0, topBar.Height - 1, topBar.Width, topBar.Height - 1);
            };
            topBar.Controls.Add(new Label
            {
                Text = "🔐  StudentHelp Admin Panel",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(248, 250, 252),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(16, 16)
            });
            var logoutBtn = new Button
            {
                Text = "Logout",
                Size = new Size(80, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 38, 38),
                ForeColor = Color.White,
                Font = Theme.FontSmall,
                Cursor = Cursors.Hand
            };
            logoutBtn.FlatAppearance.BorderSize = 0;
            logoutBtn.Click += (s, e) =>
            {
                Session.Logout();
                Close();
                Application.Run(new MainForm());
            };
            topBar.Controls.Add(logoutBtn);
            topBar.Resize += (s, e) => logoutBtn.Location = new Point(topBar.Width - 96, 14);
            logoutBtn.Location = new Point(1100, 14);
            Controls.Add(topBar);

            // Sidebar
            _sidebar = new Panel { Dock = DockStyle.Left, Width = 220, BackColor = Color.FromArgb(15, 23, 42) };
            _sidebar.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(51, 65, 85), 1);
                e.Graphics.DrawLine(pen, _sidebar.Width - 1, 0, _sidebar.Width - 1, _sidebar.Height);
            };

            _btnEarnings = MakeSidebarBtn("💰  Earnings", 0);
            _btnServices = MakeSidebarBtn("🛠  Manage Services", 1);
            _btnJobs     = MakeSidebarBtn("📋  All Bookings", 2);

            _btnEarnings.Click += (s, e) => ShowPage("earnings");
            _btnServices.Click += (s, e) => ShowPage("services");
            _btnJobs.Click     += (s, e) => ShowPage("jobs");

            _sidebar.Controls.AddRange(new Control[] { _btnEarnings, _btnServices, _btnJobs });
            Controls.Add(_sidebar);

            // Content area
            _contentArea = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(248, 250, 252), AutoScroll = true };
            Controls.Add(_contentArea);
        }

        private Button MakeSidebarBtn(string text, int index)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(220, 46),
                Location = new Point(0, 12 + index * 50),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = Theme.FontBody,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => { if (btn.BackColor == Color.Transparent) btn.BackColor = Color.FromArgb(30, 41, 59); };
            btn.MouseLeave += (s, e) => { if (btn.Tag?.ToString() != "active") btn.BackColor = Color.Transparent; };
            return btn;
        }

        private void ShowPage(string page)
        {
            _currentPage = page;
            foreach (var b in new[] { _btnEarnings, _btnServices, _btnJobs })
            { b.BackColor = Color.Transparent; b.ForeColor = Color.FromArgb(148, 163, 184); b.Tag = ""; b.Font = Theme.FontBody; }

            var active = page switch { "earnings" => _btnEarnings, "services" => _btnServices, _ => _btnJobs };
            active.BackColor = Color.FromArgb(30, 41, 59);
            active.ForeColor = Color.FromArgb(248, 250, 252);
            active.Tag = "active";
            active.Font = Theme.FontBold;

            _contentArea.SuspendLayout();
            _contentArea.Controls.Clear();

            Control view = page switch
            {
                "services" => BuildServicesPage(),
                "jobs"     => BuildJobsPage(),
                _          => BuildEarningsPage()
            };
            view.Dock = DockStyle.Fill;
            _contentArea.Controls.Add(view);
            _contentArea.ResumeLayout(true);
        }

        // ── EARNINGS PAGE ─────────────────────────────────────────────────────
        private Panel BuildEarningsPage()
        {
            var panel = new Panel { BackColor = Color.FromArgb(248, 250, 252), AutoScroll = true };
            var earnings = DatabaseManager.GetAdminEarnings();
            var pendingEarnings = DatabaseManager.GetPendingEarnings();
            int y = 24;

            // Page title
            panel.Controls.Add(MakeLabel("💰  Platform Earnings", new Font("Segoe UI", 18f, FontStyle.Bold), Theme.TextPrimary, new Point(274, y)));
            y += 40;
            panel.Controls.Add(MakeLabel("Service fee revenue (5% of each completed booking)", Theme.FontBody, Theme.TextSecondary, new Point(274, y)));
            y += 36;

            // Pending earnings banner
            if (pendingEarnings.TotalCompleted > 0)
            {
                var pendingCard = new Panel
                {
                    Location = new Point(274, y),
                    Size = new Size(920, 70),
                    BackColor = Color.FromArgb(254, 249, 195)
                };
                UIHelper.RoundCorners(pendingCard, 10);
                pendingCard.Controls.Add(new Label
                {
                    Text = $"⏳  Pending Earnings: ₱{pendingEarnings.TotalFees:N2} from {pendingEarnings.TotalCompleted} active booking(s) - Revenue is recognized when bookings are completed",
                    Font = Theme.FontBody,
                    ForeColor = Color.FromArgb(161, 138, 0),
                    AutoSize = true,
                    Location = new Point(16, 24),
                    BackColor = Color.Transparent
                });
                panel.Controls.Add(pendingCard);
                y += 86;
            }

            // Stats cards
            var statDefs = new[]
            {
                ("📅 Weekly",  earnings.WeeklyFees,  "Last 7 days",      Color.FromArgb(219, 234, 254), Color.FromArgb(59, 130, 246)),
                ("🗓 Monthly", earnings.MonthlyFees,  "This month",       Color.FromArgb(220, 252, 231), Color.FromArgb(34, 197, 94)),
                ("📆 Annual",  earnings.AnnualFees,   "This year",        Color.FromArgb(254, 249, 195), Color.FromArgb(234, 179, 8)),
                ("💼 Total",   earnings.TotalFees,    "All time",         Color.FromArgb(243, 232, 255), Color.FromArgb(168, 85, 247)),
            };

            int cardW = 220; int gap = 16; int startX = 274;
            foreach (var (title, amount, sub, bg, fg) in statDefs)
            {
                var card = new Panel
                {
                    Location  = new Point(startX, y),
                    Size      = new Size(cardW, 110),
                    BackColor = Color.White
                };
                UIHelper.RoundCorners(card, 10);
                card.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using var br = new System.Drawing.Drawing2D.GraphicsPath();
                    br.AddRoundedRectangle(new Rectangle(0, 0, card.Width, card.Height), 10);
                    using var fill = new SolidBrush(Color.White);
                    e.Graphics.FillPath(fill, br);
                    using var border = new Pen(Color.FromArgb(229, 231, 235), 1);
                    e.Graphics.DrawPath(border, br);
                };

                // Colored top bar
                var topStrip = new Panel { Location = new Point(0, 0), Size = new Size(cardW, 4), BackColor = fg };
                card.Controls.Add(topStrip);

                card.Controls.Add(new Label { Text = title, Font = Theme.FontBold, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(12, 14), BackColor = Color.Transparent });
                card.Controls.Add(new Label { Text = $"₱{amount:N2}", Font = new Font("Segoe UI", 16f, FontStyle.Bold), ForeColor = fg, AutoSize = true, Location = new Point(12, 40), BackColor = Color.Transparent });
                card.Controls.Add(new Label { Text = sub, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(12, 82), BackColor = Color.Transparent });
                panel.Controls.Add(card);
                startX += cardW + gap;
            }
            y += 128;

            // Pie charts section
            panel.Controls.Add(MakeLabel("📊  Earnings Breakdown by Category", new Font("Segoe UI", 14f, FontStyle.Bold), Theme.TextPrimary, new Point(274, y)));
            y += 30;

            // Show message if no completed earnings yet
            if (earnings.TotalFees <= 0 && earnings.TotalCompleted == 0)
            {
                panel.Controls.Add(MakeLabel("No completed bookings yet. Earnings will appear here once bookings are marked as Completed.", Theme.FontBody, Theme.TextSecondary, new Point(274, y)));
                y += 40;
            }

            // Weekly, Monthly, Annual pie charts
            var chartConfigs = new[]
            {
                ("Weekly", "weekly", earnings.WeeklyFees, Color.FromArgb(59, 130, 246)),
                ("Monthly", "monthly", earnings.MonthlyFees, Color.FromArgb(34, 197, 94)),
                ("Annual", "annual", earnings.AnnualFees, Color.FromArgb(234, 179, 8)),
            };

            int chartStartX = 274;
            foreach (var (chartTitle, period, total, color) in chartConfigs)
            {
                var chartCard = new Panel
                {
                    Location = new Point(chartStartX, y),
                    Size = new Size(280, 280),
                    BackColor = Color.White
                };
                UIHelper.RoundCorners(chartCard, 10);

                // Title
                chartCard.Controls.Add(new Label
                {
                    Text = $"{chartTitle} - by Category",
                    Font = Theme.FontBold,
                    ForeColor = Theme.TextPrimary,
                    AutoSize = true,
                    Location = new Point(12, 12),
                    BackColor = Color.Transparent
                });

                // Get earnings by category
                var categoryEarnings = DatabaseManager.GetEarningsByCategory(period);

                if (categoryEarnings.Count == 0 || total <= 0)
                {
                    chartCard.Controls.Add(new Label
                    {
                        Text = total <= 0 ? "No completed bookings\nin this period" : "No category data\navailable",
                        Font = Theme.FontBody,
                        ForeColor = Theme.TextSecondary,
                        AutoSize = false,
                        Size = new Size(200, 60),
                        Location = new Point(40, 120),
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.Transparent
                    });
                }
                else
                {
                    // Draw pie chart
                    var pieChart = new Panel
                    {
                        Location = new Point(20, 40),
                        Size = new Size(140, 140),
                        BackColor = Color.Transparent
                    };
                    pieChart.Paint += (s, e) =>
                    {
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        float totalAngle = 0;
                        float sweepAngle = 0;
                        var colors = new[] { Color.FromArgb(59, 130, 246), Color.FromArgb(34, 197, 94), Color.FromArgb(234, 179, 8), Color.FromArgb(168, 85, 247), Color.FromArgb(239, 68, 68), Color.FromArgb(249, 115, 22), Color.FromArgb(45, 212, 191), Color.FromArgb(99, 102, 241) };
                        int colorIndex = 0;

                        foreach (var kvp in categoryEarnings)
                        {
                            sweepAngle = (float)(kvp.Value / total) * 360;
                            using var brush = new SolidBrush(colors[colorIndex % colors.Length]);
                            e.Graphics.FillPie(brush, 0, 0, 140, 140, totalAngle, sweepAngle);
                            totalAngle += sweepAngle;
                            colorIndex++;
                        }
                    };
                    chartCard.Controls.Add(pieChart);

                    // Legend
                    int legendY = 40;
                    int legendX = 170;
                    var colors2 = new[] { Color.FromArgb(59, 130, 246), Color.FromArgb(34, 197, 94), Color.FromArgb(234, 179, 8), Color.FromArgb(168, 85, 247), Color.FromArgb(239, 68, 68), Color.FromArgb(249, 115, 22), Color.FromArgb(45, 212, 191), Color.FromArgb(99, 102, 241) };
                    int colorIdx = 0;

                    foreach (var kvp in categoryEarnings)
                    {
                        // Color box
                        var colorBox = new Panel
                        {
                            Location = new Point(legendX, legendY),
                            Size = new Size(12, 12),
                            BackColor = colors2[colorIdx % colors2.Length]
                        };
                        chartCard.Controls.Add(colorBox);

                        // Category name and total earnings
                        chartCard.Controls.Add(new Label
                        {
                            Text = $"{kvp.Key.ToUpper()}: ₱{kvp.Value:N2}",
                            Font = Theme.FontSmall,
                            ForeColor = Theme.TextSecondary,
                            AutoSize = true,
                            Location = new Point(legendX + 18, legendY - 2),
                            BackColor = Color.Transparent
                        });

                        legendY += 24;
                        colorIdx++;
                    }
                }

                panel.Controls.Add(chartCard);
                chartStartX += 296;
            }

            y += 296;

            // Pending bookings section
            if (pendingEarnings.TotalCompleted > 0)
            {
                panel.Controls.Add(MakeLabel("🕒  Pending Bookings (Active)", new Font("Segoe UI", 14f, FontStyle.Bold), Theme.TextPrimary, new Point(274, y)));
                y += 20;

                var pendingCategoryEarnings = DatabaseManager.GetPendingEarningsByCategory();
                var pendingCard = new Panel
                {
                    Location = new Point(274, y),
                    Size = new Size(600, 180),
                    BackColor = Color.White
                };
                UIHelper.RoundCorners(pendingCard, 10);

                if (pendingCategoryEarnings.Count > 0)
                {
                    int pendingY = 12;
                    foreach (var kvp in pendingCategoryEarnings)
                    {
                        pendingCard.Controls.Add(new Label
                        {
                            Text = $"{kvp.Key}: ₱{kvp.Value:N2}",
                            Font = Theme.FontSmall,
                            ForeColor = Theme.TextSecondary,
                            AutoSize = true,
                            Location = new Point(16, pendingY),
                            BackColor = Color.Transparent
                        });
                        pendingY += 24;
                    }
                }
                else
                {
                    pendingCard.Controls.Add(new Label
                    {
                        Text = "No pending bookings",
                        Font = Theme.FontBody,
                        ForeColor = Theme.TextSecondary,
                        AutoSize = true,
                        Location = new Point(16, 12),
                        BackColor = Color.Transparent
                    });
                }
                panel.Controls.Add(pendingCard);
                y += 200;
            }

            // Completed jobs summary
            var summaryCard = new Panel { Location = new Point(274, y), Size = new Size(920, 80), BackColor = Color.White };
            summaryCard.Controls.Add(new Label { Text = $"✅  Total Completed Jobs: {earnings.TotalCompleted}   |   Platform takes 5% of each completed service fee   |   Tutors receive 95%", Font = Theme.FontBody, ForeColor = Theme.TextPrimary, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
            panel.Controls.Add(summaryCard);

            return panel;
        }

        // ── SERVICES PAGE ─────────────────────────────────────────────────────
        private Panel BuildServicesPage()
        {
            var panel = new Panel { BackColor = Color.FromArgb(248, 250, 252), AutoScroll = true };
            var services = DatabaseManager.GetAllServicesAdmin();
            int y = 24;

            panel.Controls.Add(MakeLabel("🛠  All Services", new Font("Segoe UI", 18f, FontStyle.Bold), Theme.TextPrimary, new Point(274, y)));
            y += 40;
            panel.Controls.Add(MakeLabel($"{services.Count} total services (active and removed)", Theme.FontBody, Theme.TextSecondary, new Point(274, y)));
            y += 40;

            // Header row
            var hdr = new Panel { Location = new Point(274, y), Size = new Size(920, 32), BackColor = Color.FromArgb(229, 231, 235) };
            AddCell(hdr, "Service Title", 0, 300);
            AddCell(hdr, "Tutor", 304, 160);
            AddCell(hdr, "Category", 468, 100);
            AddCell(hdr, "Price", 572, 90);
            AddCell(hdr, "Status", 666, 80);
            AddCell(hdr, "Action", 750, 90);
            panel.Controls.Add(hdr);
            y += 36;

            foreach (var svc in services)
            {
                var row = new Panel { Location = new Point(274, y), Size = new Size(920, 44), BackColor = svc.IsActive ? Color.White : Color.FromArgb(254, 242, 242) };
                row.Paint += (s, e) => { using var p = new Pen(Color.FromArgb(229, 231, 235)); e.Graphics.DrawLine(p, 0, row.Height - 1, row.Width, row.Height - 1); };

                AddCell(row, svc.Title.Length > 35 ? svc.Title[..35] + "…" : svc.Title, 0, 300);
                AddCell(row, svc.TutorName, 304, 160);
                AddCell(row, svc.Category.ToUpper(), 468, 100);
                AddCell(row, $"₱{svc.Price:N0}", 572, 90);

                var statusLbl = new Label
                {
                    Text = svc.IsActive ? "Active" : "Removed",
                    Font = Theme.FontSmall,
                    ForeColor = svc.IsActive ? Theme.GreenText : Theme.RedText,
                    BackColor = svc.IsActive ? Theme.GreenLight : Theme.RedLight,
                    AutoSize = false, Size = new Size(70, 22),
                    Location = new Point(666, 11),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                row.Controls.Add(statusLbl);

                if (svc.IsActive)
                {
                    int svcId = svc.Id;
                    var removeBtn = new Button
                    {
                        Text = "Remove",
                        Size = new Size(76, 26),
                        Location = new Point(750, 9),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Theme.RedLight,
                        ForeColor = Theme.RedText,
                        Font = Theme.FontSmall,
                        Cursor = Cursors.Hand
                    };
                    removeBtn.FlatAppearance.BorderSize = 0;
                    removeBtn.Click += (s, e) =>
                    {
                        var confirm = MessageBox.Show($"Remove service \"{svc.Title}\"? This will hide it from Browse.", "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (confirm != DialogResult.Yes) return;
                        DatabaseManager.DeleteService(svcId);
                        MessageBox.Show("Service removed.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ShowPage("services");
                    };
                    row.Controls.Add(removeBtn);
                }

                panel.Controls.Add(row);
                y += 48;
            }

            return panel;
        }

        // ── JOBS PAGE ─────────────────────────────────────────────────────────
        private Panel BuildJobsPage()
        {
            var panel = new Panel { BackColor = Color.FromArgb(248, 250, 252), AutoScroll = true };
            var jobs = DatabaseManager.GetAllJobs();
            int y = 24;

            panel.Controls.Add(MakeLabel("📋  All Bookings", new Font("Segoe UI", 18f, FontStyle.Bold), Theme.TextPrimary, new Point(274, y)));
            y += 40;
            panel.Controls.Add(MakeLabel($"{jobs.Count} total bookings across all users", Theme.FontBody, Theme.TextSecondary, new Point(274, y)));
            y += 40;

            var hdr = new Panel { Location = new Point(274, y), Size = new Size(1050, 32), BackColor = Color.FromArgb(229, 231, 235) };
            AddCell(hdr, "Service", 0, 240);
            AddCell(hdr, "Client", 244, 140);
            AddCell(hdr, "Tutor", 388, 140);
            AddCell(hdr, "Price", 532, 80);
            AddCell(hdr, "Ref #", 616, 100);
            AddCell(hdr, "Status", 720, 80);
            AddCell(hdr, "Date", 804, 120);
            AddCell(hdr, "Action", 928, 120);
            panel.Controls.Add(hdr);
            y += 36;

            foreach (var job in jobs)
            {
                var (bg, fg) = job.Status switch
                {
                    "Completed" => (Theme.GreenLight, Theme.GreenText),
                    "Cancelled" => (Theme.RedLight, Theme.RedText),
                    "Active"    => (Theme.LightBlue, Theme.PrimaryBlue),
                    "Refunded"  => (Color.FromArgb(229, 231, 235), Color.FromArgb(107, 114, 128)),
                    _           => (Color.FromArgb(254,249,195), Color.FromArgb(161,138,0))
                };
                var row = new Panel { Location = new Point(274, y), Size = new Size(1050, 44), BackColor = Color.White };
                row.Paint += (s, e) => { using var p = new Pen(Color.FromArgb(229, 231, 235)); e.Graphics.DrawLine(p, 0, row.Height - 1, row.Width, row.Height - 1); };
                AddCell(row, job.ServiceTitle.Length > 28 ? job.ServiceTitle[..28] + "…" : job.ServiceTitle, 0, 240);
                AddCell(row, job.ClientName, 244, 140);
                AddCell(row, job.TutorName, 388, 140);
                AddCell(row, $"₱{job.Price:N0}", 532, 80);

                // Reference number with copy-friendly format
                string refDisplay = string.IsNullOrEmpty(job.ReferenceNumber) ? "N/A" : job.ReferenceNumber;
                var refLbl = new Label { Text = refDisplay, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Theme.PrimaryBlue, AutoSize = false, Size = new Size(96, 22), Location = new Point(616, 11), TextAlign = ContentAlignment.MiddleLeft, BackColor = Color.Transparent };
                row.Controls.Add(refLbl);

                var statusLbl = new Label { Text = job.Status, Font = Theme.FontSmall, ForeColor = fg, BackColor = bg, AutoSize = false, Size = new Size(78, 22), Location = new Point(720, 11), TextAlign = ContentAlignment.MiddleCenter };
                row.Controls.Add(statusLbl);
                AddCell(row, job.BookedAt.ToString("MMM dd, yyyy"), 804, 120);

                // Refund button for completed jobs
                if (job.Status == "Completed")
                {
                    var jobRef = job.ReferenceNumber;
                    var refundBtn = new Button
                    {
                        Text = "💰 Refund",
                        Size = new Size(100, 26),
                        Location = new Point(928, 9),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Theme.OrangeLight,
                        ForeColor = Theme.OrangeText,
                        Font = Theme.FontSmall,
                        Cursor = Cursors.Hand
                    };
                    refundBtn.FlatAppearance.BorderSize = 0;
                    refundBtn.Click += (s, e) =>
                    {
                        var confirm = MessageBox.Show(
                            $"Process refund for booking {jobRef}?\n\n" +
                            $"Amount: ₱{job.TotalAmount:N2}\n" +
                            $"This will refund the full amount to {job.ClientName} and mark the booking as Refunded.",
                            "Confirm Refund", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (confirm != DialogResult.Yes) return;

                        var result = DatabaseManager.ProcessRefund(jobRef);
                        if (result.success)
                        {
                            MessageBox.Show(result.error, "Refund Processed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ShowPage("jobs");
                        }
                        else
                        {
                            MessageBox.Show(result.error, "Refund Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };
                    row.Controls.Add(refundBtn);
                }
                else if (job.Status == "Refunded")
                {
                    var refundedLbl = new Label
                    {
                        Text = "✓ Refunded",
                        Font = Theme.FontSmall,
                        ForeColor = Color.FromArgb(107, 114, 128),
                        AutoSize = false,
                        Size = new Size(100, 22),
                        Location = new Point(928, 11),
                        TextAlign = ContentAlignment.MiddleLeft,
                        BackColor = Color.Transparent
                    };
                    row.Controls.Add(refundedLbl);
                }

                panel.Controls.Add(row);
                y += 48;
            }

            return panel;
        }

        private static Label MakeLabel(string text, Font font, Color color, Point loc)
            => new() { Text = text, Font = font, ForeColor = color, AutoSize = true, Location = loc, BackColor = Color.Transparent };

        private static void AddCell(Panel row, string text, int x, int w)
        {
            row.Controls.Add(new Label
            {
                Text = text, Font = Theme.FontBody, ForeColor = Theme.TextPrimary,
                AutoSize = false, Size = new Size(w - 4, row.Height),
                Location = new Point(x + 4, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            });
        }
    }
}
