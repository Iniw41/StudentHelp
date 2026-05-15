using TutoringMarketplace.Controls;
using TutoringMarketplace.Database;

namespace TutoringMarketplace.Forms.Pages
{
    partial class ServiceDetailPagePanel
    {
        private void InitializeComponent()
        {
            var service = DatabaseManager.GetService(_serviceId);
            var tutor   = service != null ? DatabaseManager.GetAccount(service.TutorId) : null;

            if (service == null || tutor == null)
            {
                Controls.Add(new Label { Text = "Service not found.", Font = Theme.FontH2, ForeColor = Theme.TextSecondary, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
                return;
            }

            int pw = 1860;   // content width — matches new 1920-wide window

            // ── Back button ───────────────────────────────────────────────────
            var backBtn = UIHelper.MakeButton("← Back", Color.White, Theme.PrimaryBlue, 100, 32);
            backBtn.Location = new Point(16, 12);
            backBtn.FlatAppearance.BorderSize = 1; backBtn.FlatAppearance.BorderColor = Theme.PrimaryBlue;
            backBtn.Click += OnBack;
            Controls.Add(backBtn);
            int y = 54;

            int lx = 16, lw = 1220, rx = 1252, rw = pw - rx - 16;

            // ═══════════════════════ LEFT COLUMN ════════════════════════════

            // Service Info Card
            var infoCard = UIHelper.MakeCard(lx, y, lw, 340);
            var catHdr = new Panel { Location = new Point(0,0), Size = new Size(lw,46), BackColor = Theme.LightBlue };
            catHdr.Controls.Add(new Label { Text = service.Category.ToUpper(), Font = new Font("Segoe UI",9f,FontStyle.Bold), ForeColor = Theme.PrimaryBlue, AutoSize = true, Location = new Point(14,14), BackColor = Color.Transparent });
            infoCard.Controls.Add(catHdr);

            // tags
            int tx = 14;
            foreach (var tag in service.TagList)
            {
                var badge = UIHelper.MakeBadge(tag, Theme.LightBlue, Theme.PrimaryBlue);
                badge.Location = new Point(tx, 56); infoCard.Controls.Add(badge); tx += badge.Width + 6;
            }

            infoCard.Controls.Add(new Label { Text = service.Title, Font = new Font("Segoe UI",14f,FontStyle.Bold), ForeColor = Theme.TextPrimary, Size = new Size(lw-28,52), Location = new Point(14,84), BackColor = Color.Transparent });

            var rp = new Panel { Location = new Point(14,142), Size = new Size(400,24), BackColor = Color.Transparent };
            rp.Controls.Add(new Label { Text = UIHelper.StarString(service.Rating), Font = new Font("Segoe UI",12f), ForeColor = Theme.StarColor, AutoSize = true, Location = new Point(0,0), BackColor = Color.Transparent });
            rp.Controls.Add(new Label { Text = $"  {service.Rating:0.0} ({service.ReviewCount} reviews)   ·   ⏱ {service.Duration}", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(96,3), BackColor = Color.Transparent });
            infoCard.Controls.Add(rp);

            infoCard.Controls.Add(new Panel { Location = new Point(14,174), Size = new Size(lw-28,1), BackColor = Theme.BorderColor });
            infoCard.Controls.Add(new Label { Text = "About this service", Font = Theme.FontH3, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(14,184), BackColor = Color.Transparent });
            infoCard.Controls.Add(new Label { Text = service.Description, Font = Theme.FontBody, ForeColor = Theme.TextSecondary, Size = new Size(lw-28,100), Location = new Point(14,210), BackColor = Color.Transparent });
            Controls.Add(infoCard);
            int leftY = y + 350;

            // What you'll get
            var getCard = UIHelper.MakeCard(lx, leftY, lw, 160);
            getCard.Controls.Add(new Label { Text = "What you'll get", Font = Theme.FontH3, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(14,14), BackColor = Color.Transparent });
            int py = 42;
            foreach (var perk in new[] { "One-on-one personalized help session", "Detailed step-by-step explanations", "Follow-up support via messaging", "Flexible scheduling to fit your timetable" })
            {
                getCard.Controls.Add(new Label { Text = $"✅  {perk}", Font = Theme.FontBody, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(14,py), BackColor = Color.Transparent });
                py += 26;
            }
            Controls.Add(getCard);
            leftY += 168;

            // Reviews section with write review form and scrollable all reviews list
            var allReviews = DatabaseManager.GetServiceReviews(service.Id);
            double avgRating = allReviews.Any() ? allReviews.Average(r => r.rating) : service.Rating;
            int totalReviews = allReviews.Count;

            // Fixed card height with scrollable reviews section
            int revCardHeight = 420;
            var revCard = UIHelper.MakeCard(lx, leftY, lw, revCardHeight);

            // Header with average rating
            revCard.Controls.Add(new Label { Text = "Reviews", Font = Theme.FontH3, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(14,14), BackColor = Color.Transparent });
            revCard.Controls.Add(new Label { Text = avgRating.ToString("0.0"), Font = new Font("Segoe UI",28f,FontStyle.Bold), ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(14,40), BackColor = Color.Transparent });
            revCard.Controls.Add(new Label { Text = UIHelper.StarString((int)Math.Round(avgRating)), Font = new Font("Segoe UI",14f), ForeColor = Theme.StarColor, AutoSize = true, Location = new Point(14,88), BackColor = Color.Transparent });
            revCard.Controls.Add(new Label { Text = $"{totalReviews} reviews", Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(14,110), BackColor = Color.Transparent });

            // Write review form
            int formY = 140;
            revCard.Controls.Add(new Label { Text = "Write a Review", Font = Theme.FontBold, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(14, formY), BackColor = Color.Transparent });

            // Star rating selector
            var starButtons = new Button[5];
            int selectedStars = 5;
            int starX = 14;
            for (int i = 0; i < 5; i++)
            {
                int starIndex = i;
                var btn = new Button
                {
                    Text = "★",
                    Font = new Font("Segoe UI", 22f),
                    ForeColor = Theme.StarColor,
                    Size = new Size(40, 40),
                    Location = new Point(starX + (i * 42), formY + 24),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Transparent,
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, e) =>
                {
                    selectedStars = starIndex + 1;
                    for (int j = 0; j < 5; j++)
                        starButtons[j].ForeColor = j < selectedStars ? Theme.StarColor : Color.FromArgb(209, 213, 219);
                };
                starButtons[i] = btn;
                revCard.Controls.Add(btn);
            }

            // Review textbox
            var txtReview = new TextBox
            {
                Multiline = true,
                Size = new Size(lw - 56, 60),
                Location = new Point(14, formY + 70),
                Font = Theme.FontBody,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Write your review here...",
                ForeColor = Theme.TextSecondary
            };
            txtReview.GotFocus += (s, e) => { if (txtReview.Text == "Write your review here...") { txtReview.Text = ""; txtReview.ForeColor = Theme.TextPrimary; } };
            txtReview.LostFocus += (s, e) => { if (txtReview.Text == "") { txtReview.Text = "Write your review here..."; txtReview.ForeColor = Theme.TextSecondary; } };
            revCard.Controls.Add(txtReview);

            // Submit button
            var submitBtn = UIHelper.MakeButton("Submit Review", Theme.StarColor, Color.White, 140, 30);
            submitBtn.Location = new Point(14, formY + 140);
            submitBtn.Click += (s, e) =>
            {
                if (!Session.IsLoggedIn)
                {
                    MessageBox.Show("Please log in to leave a review.", "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtReview.Text) || txtReview.Text == "Write your review here...")
                {
                    MessageBox.Show("Please write a review before submitting.", "Review Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DatabaseManager.AddServiceReview(service.Id, Session.CurrentUser!.Id, selectedStars, txtReview.Text);
                MessageBox.Show("Review submitted! Thank you.", "Thanks", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Refresh the panel
                var parent = Parent;
                if (parent != null)
                {
                    parent.SuspendLayout();
                    parent.Controls.Remove(this);
                    var fresh = new ServiceDetailPagePanel(service.Id);
                    fresh.BackRequested += (ss, ee) => BackRequested?.Invoke(this, EventArgs.Empty);
                    fresh.ProfileRequested += (ss, pid) => ProfileRequested?.Invoke(this, pid);
                    fresh.BookingMade += (ss, ee) => BookingMade?.Invoke(this, EventArgs.Empty);
                    fresh.Dock = DockStyle.Fill;
                    parent.Controls.Add(fresh);
                    parent.ResumeLayout(true);
                }
            };
            revCard.Controls.Add(submitBtn);

            // Scrollable All Reviews panel
            int reviewsListY = formY + 185;
            int reviewsPanelHeight = revCardHeight - reviewsListY - 10;

            var reviewsLabel = new Label { Text = "All Reviews", Font = Theme.FontH3, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(14, reviewsListY), BackColor = Color.Transparent };
            revCard.Controls.Add(reviewsLabel);

            // Create scrollable panel for reviews
            var reviewsPanel = new Panel
            {
                Location = new Point(14, reviewsListY + 30),
                Size = new Size(lw - 28, reviewsPanelHeight - 20),
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            int reviewY = 0;
            if (!allReviews.Any())
            {
                reviewsPanel.Controls.Add(new Label { Text = "No reviews yet. Be the first to book this service and leave a review!", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(0, 0), BackColor = Color.Transparent });
            }
            else
            {
                foreach (var (clientId, clientName, rating, comment, createdAt) in allReviews)
                {
                    var av = UIHelper.MakeAvatar(clientName, UIHelper.AvatarColor(clientName), 36);
                    av.Location = new Point(0, reviewY);
                    reviewsPanel.Controls.Add(av);
                    reviewsPanel.Controls.Add(new Label { Text = clientName, Font = Theme.FontBold, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(44, reviewY + 2), BackColor = Color.Transparent });
                    reviewsPanel.Controls.Add(new Label { Text = UIHelper.StarString(rating), Font = new Font("Segoe UI", 10f), ForeColor = Theme.StarColor, AutoSize = true, Location = new Point(44, reviewY + 18), BackColor = Color.Transparent });
                    reviewsPanel.Controls.Add(new Label { Text = createdAt.ToString("MMM dd, yyyy"), Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(lw - 180, reviewY + 2), BackColor = Color.Transparent });
                    if (!string.IsNullOrWhiteSpace(comment))
                    {
                        reviewsPanel.Controls.Add(new Label { Text = comment, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, Size = new Size(lw - 180, 40), Location = new Point(44, reviewY + 34), BackColor = Color.Transparent });
                    }
                    reviewY += 90;
                }
                // Set the internal content height for scrolling
                reviewsPanel.AutoScrollMinSize = new Size(0, reviewY);
            }

            revCard.Controls.Add(reviewsPanel);
            Controls.Add(revCard);

            // ═══════════════════════ RIGHT SIDEBAR ══════════════════════════

            // Pricing card
            var priceCard = UIHelper.MakeCard(rx, y, rw, 270);
            priceCard.Controls.Add(new Label { Text = UIHelper.FormatPeso(service.Price), Font = new Font("Segoe UI",28f,FontStyle.Bold), ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(rw/2-50,14), BackColor = Color.Transparent });
            priceCard.Controls.Add(new Label { Text = $"per session ({service.Duration})", Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(rw/2-58,56), BackColor = Color.Transparent });

            var contactBtn = UIHelper.MakeButton("💬  Contact", Theme.PrimaryBlue, Color.White, rw-28, 38);
            contactBtn.Location = new Point(14, 88); contactBtn.Click += OnContact;
            priceCard.Controls.Add(contactBtn);

            var bookBtn = UIHelper.MakeButton("📅  Book This Service", Theme.GreenText, Color.White, rw-28, 38);
            bookBtn.Location = new Point(14, 132); bookBtn.Click += OnBook;
            bookBtn.BackColor = Theme.GreenText;
            priceCard.Controls.Add(bookBtn);

            priceCard.Controls.Add(new Panel { Location = new Point(14,178), Size = new Size(rw-28,1), BackColor = Theme.BorderColor });
            int sy = 188;
            foreach (var (lbl, val) in new[] { ("Response time", tutor.ResponseTime), ("Completed jobs", tutor.CompletedJobs.ToString())}) //, ("Success rate", "98%") 
            {
                priceCard.Controls.Add(new Label { Text = lbl, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(14,sy), BackColor = Color.Transparent });
                priceCard.Controls.Add(new Label { Text = val, Font = Theme.FontBold, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(rw-14-TextRenderer.MeasureText(val,Theme.FontBold).Width,sy), BackColor = Color.Transparent });
                sy += 22;
            }
            Controls.Add(priceCard);

            // Tutor card
            int scy = y + 280;
            var stuCard = UIHelper.MakeCard(rx, scy, rw, 278);
            var av2 = UIHelper.MakeAvatar(tutor.FullName, UIHelper.AvatarColor(tutor.FullName), 52);
            av2.Location = new Point(rw/2-26, 14); stuCard.Controls.Add(av2);
            stuCard.Controls.Add(new Label { Text = tutor.FullName, Font = Theme.FontH3, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(rw/2 - TextRenderer.MeasureText(tutor.FullName,Theme.FontH3).Width/2, 72), BackColor = Color.Transparent });
            stuCard.Controls.Add(new Label { Text = tutor.Major, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(14,94), BackColor = Color.Transparent });
            stuCard.Controls.Add(new Label { Text = $"{tutor.Year} · {tutor.University}", Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(14,112), BackColor = Color.Transparent });
            stuCard.Controls.Add(new Label { Text = $"{UIHelper.StarString(tutor.Rating)} {tutor.Rating:0.0} ({tutor.CompletedJobs} reviews)", Font = Theme.FontSmall, ForeColor = Theme.StarColor, AutoSize = true, Location = new Point(14,132), BackColor = Color.Transparent });
            stuCard.Controls.Add(new Label { Text = tutor.Bio, Font = Theme.FontSmall, ForeColor = Theme.TextSecondary, Size = new Size(rw-28,48), Location = new Point(14,152), BackColor = Color.Transparent });

            var profBtn = UIHelper.MakeButton("View Full Profile", Color.White, Theme.PrimaryBlue, rw-28, 34);
            profBtn.Location = new Point(14, 238);
            profBtn.FlatAppearance.BorderSize = 1; profBtn.FlatAppearance.BorderColor = Theme.PrimaryBlue;
            profBtn.Click += (s, e) => OnProfile(tutor.Id);
            stuCard.Controls.Add(profBtn);
            Controls.Add(stuCard);
        }
    }
}
