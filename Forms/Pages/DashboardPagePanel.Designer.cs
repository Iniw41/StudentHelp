using TutoringMarketplace.Controls;
using TutoringMarketplace.Database;
using TutoringMarketplace.Models;

namespace TutoringMarketplace.Forms.Pages
{
    partial class DashboardPagePanel
    {
        private void InitializeComponent()
        {
            if (!Session.IsLoggedIn)
            {
                Controls.Add(new Label { Text = "Please log in to view your dashboard.", Font = Theme.FontH2, ForeColor = Theme.TextSecondary, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
                return;
            }

            var user = Session.CurrentUser!;
            int pw   = 920;
            int y    = 16;

            // ── Header ────────────────────────────────────────────────────────
            var hdrCard = UIHelper.MakeCard(16, y, pw - 32, 100);
            var avH = UIHelper.MakeAvatar(user.FullName, UIHelper.AvatarColor(user.FullName), 56);
            avH.Location = new Point(18, 20);
            hdrCard.Controls.Add(avH);
            hdrCard.Controls.Add(new Label { Text = $"Welcome back, {user.FullName.Split(' ')[0]}!", Font = new Font("Segoe UI",16f,FontStyle.Bold), ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(86,16), BackColor = Color.Transparent });
            hdrCard.Controls.Add(new Label { Text = $"{user.Major}  ·  {user.University}  ·  {user.Year}", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(86,46), BackColor = Color.Transparent });

            // Wallet section in header
            var walletLbl = new Label { Text = $"Wallet: ₱{user.WalletBalance:N2}", Font = Theme.FontBold, ForeColor = Theme.GreenText, AutoSize = true, Location = new Point(86, 70), BackColor = Color.Transparent };
            hdrCard.Controls.Add(walletLbl);
            var topUpBtn = UIHelper.MakeButton("+ Top Up", Theme.GreenText, Color.White, 90, 26);
            topUpBtn.Location = new Point(86 + walletLbl.PreferredWidth + 10, 66);
            topUpBtn.Click += (s, e) => OnTopUpWallet();
            hdrCard.Controls.Add(topUpBtn);

            Controls.Add(hdrCard);
            y += 112;

            // ── Summary stats ─────────────────────────────────────────────────
            var jobsTaken   = DatabaseManager.GetJobsTakenByClient(user.Id);
            var jobsCreated = DatabaseManager.GetJobsCreatedByTutor(user.Id);
            var myServices  = DatabaseManager.GetServicesByTutor(user.Id);

            decimal totalEarned = jobsCreated.Where(j => j.Status == "Completed").Sum(j => j.TotalAmount) * 0.95m;
            int completedCount  = jobsCreated.Count(j => j.Status == "Completed");

            var statsData = new[]
            {
                ("📚", jobsTaken.Count.ToString(),   "Jobs Booked",     Theme.LightBlue,   Theme.PrimaryBlue),
                ("🛠", myServices.Count.ToString(),  "Services Listed", Theme.PurpleLight, Theme.PurpleText),
                ("✅", completedCount.ToString(),     "Jobs Completed",  Theme.GreenLight,  Theme.GreenText),
                ("💰", UIHelper.FormatPeso(totalEarned), "Total Earned", Theme.OrangeLight, Theme.OrangeText),
            };
            int stX = 16, stW = (pw - 32 - 12) / 4;
            foreach (var (icon, val, lbl, bg, fg) in statsData)
            {
                var sc = UIHelper.MakeCard(stX, y, stW - 4, 100);
                sc.Controls.Add(new Label { Text = icon, Font = new Font("Segoe UI Emoji",18f), AutoSize = false, Size = new Size(stW,42), Location = new Point(0,8), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                sc.Controls.Add(new Label { Text = val, Font = new Font("Segoe UI",14f,FontStyle.Bold), ForeColor = fg, AutoSize = false, Size = new Size(stW,26), Location = new Point(0,52), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                sc.Controls.Add(new Label { Text = lbl, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = false, Size = new Size(stW,20), Location = new Point(0,78), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                Controls.Add(sc); stX += stW;
            }
            y += 112;

            // ═══════════ JOBS TAKEN (as client) ═══════════════════════════════
            var pendingAndActive = jobsTaken.Where(j => j.Status != "Completed" && j.Status != "Cancelled").ToList();
            Controls.Add(SectionHeader(y, pw, "📚  My Bookings — Pending / Active", $"{pendingAndActive.Count} active"));
            y += 44;

            if (!pendingAndActive.Any())
            {
                var emp = UIHelper.MakeCard(16, y, pw - 32, 60);
                emp.Controls.Add(new Label { Text = "No active bookings. Browse services to get started!", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                Controls.Add(emp); y += 68;
            }
            else
            {
                foreach (var job in pendingAndActive)
                {
                    var row = BuildJobRow(job, isTutor: false);
                    row.Location = new Point(16, y);
                    Controls.Add(row);
                    y += row.Height + 8;
                }
            }
            y += 8;

            // Completed bookings (as client)
            var completedBookings = jobsTaken.Where(j => j.Status == "Completed").ToList();
            Controls.Add(SectionHeader(y, pw, "✅  Completed Bookings", $"{completedBookings.Count} completed"));
            y += 44;
            if (!completedBookings.Any())
            {
                var emp = UIHelper.MakeCard(16, y, pw - 32, 60);
                emp.Controls.Add(new Label { Text = "No completed bookings yet.", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                Controls.Add(emp); y += 68;
            }
            else
            {
                foreach (var job in completedBookings)
                {
                    var row = BuildJobRow(job, isTutor: false);
                    row.Location = new Point(16, y);
                    Controls.Add(row);
                    y += row.Height + 8;
                }
            }
            y += 16;

            // ═══════════ MY SERVICES listed ════════════════════════════════════
            Controls.Add(SectionHeader(y, pw, "🛠  My Services", $"{myServices.Count} service{(myServices.Count != 1 ? "s" : "")} listed"));
            y += 44;

            if (!myServices.Any())
            {
                var emp2 = UIHelper.MakeCard(16, y, pw - 32, 60);
                emp2.Controls.Add(new Label { Text = "You haven't listed any services yet. Click 'Post Service' to get started!", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                Controls.Add(emp2); y += 68;
            }
            else
            {
                foreach (var svc in myServices)
                {
                    var svcRow = BuildServiceRow(svc, pw - 32);
                    svcRow.Location = new Point(16, y);
                    Controls.Add(svcRow);
                    y += svcRow.Height + 8;
                }
            }
            y += 16;

            // ═══════════ JOBS RECEIVED (as tutor) ══════════════════════════════
            var incomingActive = jobsCreated.Where(j => j.Status != "Completed" && j.Status != "Cancelled").ToList();
            Controls.Add(SectionHeader(y, pw, "💼  Jobs I've Received — Active", $"{incomingActive.Count} incoming"));
            y += 44;

            if (!incomingActive.Any())
            {
                var emp3 = UIHelper.MakeCard(16, y, pw - 32, 60);
                emp3.Controls.Add(new Label { Text = "No active incoming jobs.", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                Controls.Add(emp3); y += 68;
            }
            else
            {
                foreach (var job in incomingActive)
                {
                    var row = BuildJobRow(job, isTutor: true);
                    row.Location = new Point(16, y);
                    Controls.Add(row);
                    y += row.Height + 8;
                }
            }
            y += 8;

            // Completed jobs received
            var completedReceived = jobsCreated.Where(j => j.Status == "Completed").ToList();
            Controls.Add(SectionHeader(y, pw, "✅  Jobs Completed (as Tutor)", $"{completedReceived.Count} completed"));
            y += 44;
            if (!completedReceived.Any())
            {
                var emp4 = UIHelper.MakeCard(16, y, pw - 32, 60);
                emp4.Controls.Add(new Label { Text = "No completed jobs yet.", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                Controls.Add(emp4); y += 68;
            }
            else
            {
                foreach (var job in completedReceived)
                {
                    var row = BuildJobRow(job, isTutor: true);
                    row.Location = new Point(16, y);
                    Controls.Add(row);
                    y += row.Height + 8;
                }
            }
        }

        // ── Service Row (with Remove button) ─────────────────────────────────
        private Panel BuildServiceRow(Service svc, int width)
        {
            int rowH = 74;
            var row = UIHelper.MakeCard(0, 0, width, rowH);

            // Category badge
            var catBadge = UIHelper.MakeBadge(svc.Category.ToUpper(), Theme.LightBlue, Theme.PrimaryBlue);
            catBadge.Location = new Point(14, 12);
            row.Controls.Add(catBadge);

            row.Controls.Add(new Label { Text = svc.Title, Font = Theme.FontBold, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(14, 36), BackColor = Color.Transparent });
            row.Controls.Add(new Label { Text = $"{UIHelper.FormatPeso(svc.Price)}  ·  ⭐ {svc.Rating:0.0} ({svc.ReviewCount} reviews)  ·  ⏱ {svc.Duration}", Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(14, 56), BackColor = Color.Transparent });

            int bx = width - 14;

            // View button
            int svcId = svc.Id;
            var viewBtn = UIHelper.MakeButton("View", Color.White, Theme.PrimaryBlue, 72, 28);
            bx -= 76; viewBtn.Location = new Point(bx, rowH / 2 - 14);
            viewBtn.FlatAppearance.BorderSize = 1; viewBtn.FlatAppearance.BorderColor = Theme.PrimaryBlue;
            viewBtn.Click += (s, e) => OnServiceClick(svcId);
            row.Controls.Add(viewBtn);

            // Remove button
            string svcTitle = svc.Title;
            var removeBtn = UIHelper.MakeButton("✕ Remove", Theme.RedLight, Theme.RedText, 90, 28);
            bx -= 94; removeBtn.Location = new Point(bx, rowH / 2 - 14);
            removeBtn.FlatAppearance.BorderSize = 1; removeBtn.FlatAppearance.BorderColor = Theme.RedText;
            removeBtn.Click += (s, e) => OnRemoveService(svcId, svcTitle);
            row.Controls.Add(removeBtn);

            return row;
        }

        // ── Job Row builder ───────────────────────────────────────────────────
        private Panel BuildJobRow(JobTaken job, bool isTutor)
        {
            int pw   = 888;
            int rowH = 90;
            bool canReview   = !isTutor && job.Status == "Completed" && !job.Rating.HasValue;
            bool canComplete = isTutor  && (job.Status == "Active" || job.Status == "Pending");
            bool canCancel   = job.Status == "Pending";

            var row = UIHelper.MakeCard(0, 0, pw, rowH);

            var badge = UIHelper.MakeStatusBadge(job.Status);
            badge.Location = new Point(14, 14);
            row.Controls.Add(badge);

            row.Controls.Add(new Label { Text = job.ServiceTitle, Font = Theme.FontBold, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(14, 40), BackColor = Color.Transparent });
            string party = isTutor ? $"Client: {job.ClientName}" : $"Tutor: {job.TutorName}";

            // Show 95% earning for completed tutor rows (based on total amount paid)
            string priceDisplay = (isTutor && job.Status == "Completed")
                ? $"{UIHelper.FormatPeso(job.TotalAmount * 0.95m)} earned (95%)"
                : UIHelper.FormatPeso(job.Price);

            string refDisplay = string.IsNullOrEmpty(job.ReferenceNumber) ? "" : $"  ·  Ref: {job.ReferenceNumber}";
            row.Controls.Add(new Label { Text = $"{party}   ·   {job.BookedAt:MMM dd, yyyy}   ·   {priceDisplay}{refDisplay}", Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(14, 62), BackColor = Color.Transparent });

            if (job.Rating.HasValue)
            {
                row.Controls.Add(new Label { Text = $"Your rating: {UIHelper.StarString(job.Rating.Value)}", Font = Theme.FontSmall, ForeColor = Theme.StarColor, AutoSize = true, Location = new Point(14, 76), BackColor = Color.Transparent });
                rowH = 100; row.Size = new Size(pw, 100);
            }

            int bx = pw - 14;

            if (canReview)
            {
                var jobId = job.Id; var svcTitle = job.ServiceTitle;
                var reviewBtn = UIHelper.MakeButton("⭐  Leave Review", Theme.StarColor, Color.White, 140, 30);
                reviewBtn.BackColor = Color.FromArgb(251, 191, 36);
                bx -= 144; reviewBtn.Location = new Point(bx, rowH / 2 - 15);
                reviewBtn.Click += (s, e) => OnLeaveReview(jobId, svcTitle);
                row.Controls.Add(reviewBtn);
            }

            if (canComplete)
            {
                var jobId = job.Id;
                var doneBtn = UIHelper.MakeButton("✅  Complete", Theme.GreenText, Color.White, 130, 30);
                doneBtn.BackColor = Theme.GreenText;
                bx -= 134; doneBtn.Location = new Point(bx, rowH / 2 - 15);
                doneBtn.Click += (s, e) => OnMarkComplete(jobId);
                row.Controls.Add(doneBtn);
            }

            if (canCancel)
            {
                var jobId = job.Id;
                var cancelBtn = UIHelper.MakeButton("✕  Cancel", Theme.RedLight, Theme.RedText, 90, 30);
                bx -= 94; cancelBtn.Location = new Point(bx, rowH / 2 - 15);
                cancelBtn.FlatAppearance.BorderSize = 1; cancelBtn.FlatAppearance.BorderColor = Theme.RedText;
                cancelBtn.Click += (s, e) => OnCancelJob(jobId);
                row.Controls.Add(cancelBtn);
            }

            return row;
        }

        private static Label SectionHeader(int y, int pw, string title, string sub)
            => new() { Text = $"{title}  —  {sub}", Font = Theme.FontH3, ForeColor = Theme.TextPrimary, AutoSize = false, Size = new Size(pw - 32, 30), Location = new Point(16, y), BackColor = Color.Transparent };
    }
}
