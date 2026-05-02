using TutoringMarketplace.Controls;
using TutoringMarketplace.Forms.Pages;

namespace TutoringMarketplace.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            UpdateUserArea();
            NavigateTo("home");
        }

        public void NavigateTo(string page, string? id = null)
        {
            UpdateNavHighlight(page);
            _contentArea.SuspendLayout();
            _contentArea.Controls.Clear();

            Control view = page switch
            {
                "browse"         => CreateBrowse(id),
                "post"           => CreatePostService(),
                "service-detail" => CreateServiceDetail(int.Parse(id!)),
                "profile"        => CreateProfile(int.Parse(id!)),
                "dashboard"      => CreateDashboard(),
                _                => CreateHome()
            };

            view.Dock = DockStyle.Fill;
            _contentArea.Controls.Add(view);
            _contentArea.ResumeLayout(true);
        }

        private void UpdateNavHighlight(string page)
        {
            foreach (var (btn, pg) in new[] { (_btnHome, "home"), (_btnBrowse, "browse"), (_btnPost, "post"), (_btnDashboard, "dashboard") })
            {
                bool active = pg == page;
                btn.ForeColor = active ? Theme.PrimaryBlue : Theme.TextSecondary;
                btn.Font = active ? Theme.FontNav : new Font("Segoe UI", 11f);
            }
        }

        public void UpdateUserArea()
        {
            _pnlUserArea.Controls.Clear();
            if (Session.IsLoggedIn)
            {
                var av = UIHelper.MakeAvatar(Session.CurrentUser!.FullName, UIHelper.AvatarColor(Session.CurrentUser.FullName), 34);
                av.Location = new Point(0, 11);
                var nameLabel = new Label
                {
                    Text = Session.CurrentUser.FullName.Split(' ')[0],
                    Font = Theme.FontBold, ForeColor = Theme.TextPrimary,
                    AutoSize = true, Location = new Point(42, 18), BackColor = Color.Transparent
                };

                // Wallet balance display
                var walletLbl = new Label
                {
                    Text = $"₱{Session.CurrentUser.WalletBalance:N2}",
                    Font = Theme.FontSmall, ForeColor = Theme.GreenText,
                    AutoSize = true, Location = new Point(42, 32), BackColor = Color.Transparent
                };

                var logoutBtn = UIHelper.MakeButton("Logout", Theme.RedLight, Theme.RedText, 72, 28);
                logoutBtn.Location = new Point(42 + nameLabel.PreferredWidth + 8, 14);
                logoutBtn.Click += (s, e) => { Session.Logout(); UpdateUserArea(); NavigateTo("home"); };

                _pnlUserArea.Controls.AddRange(new Control[] { av, nameLabel, walletLbl, logoutBtn });
                _btnDashboard.Visible = true;
            }
            else
            {
                var loginBtn = UIHelper.MakeButton("Login / Register", Theme.PrimaryBlue, Color.White, 150, 32);
                loginBtn.Location = new Point(0, 12);
                loginBtn.Click += (s, e) =>
                {
                    using var dlg = new LoginForm();
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        if (Session.IsAdmin)
                        {
                            Session.Logout();
                            using var adminForm = new AdminForm();
                            adminForm.ShowDialog(this);
                            return;
                        }
                        UpdateUserArea();
                        NavigateTo("home");
                    }
                };
                _pnlUserArea.Controls.Add(loginBtn);
                _btnDashboard.Visible = false;
            }
        }

        private HomePagePanel CreateHome()
        {
            var p = new HomePagePanel();
            p.BrowseRequested        += (s, e)   => NavigateTo("browse");
            p.PostServiceRequested   += (s, e)   => NavigateTo("post");
            p.ServiceDetailRequested += (s, id)  => NavigateTo("service-detail", id.ToString());
            p.ProfileRequested       += (s, id)  => NavigateTo("profile", id.ToString());
            p.CategoryRequested      += (s, cat) => NavigateTo("browse", cat);
            return p;
        }

        private BrowsePagePanel CreateBrowse(string? category)
        {
            var p = new BrowsePagePanel(category);
            p.ServiceDetailRequested += (s, id) => NavigateTo("service-detail", id.ToString());
            p.ProfileRequested       += (s, id) => NavigateTo("profile", id.ToString());
            return p;
        }

        private PostServicePagePanel CreatePostService()
        {
            if (!Session.IsLoggedIn)
            {
                using var dlg = new LoginForm();
                if (dlg.ShowDialog(this) != DialogResult.OK) return CreatePostService();
                UpdateUserArea();
            }
            var p = new PostServicePagePanel();
            p.BackRequested  += (s, e) => NavigateTo("browse");
            p.ServicePosted  += (s, e) => NavigateTo("browse");
            return p;
        }

        private ServiceDetailPagePanel CreateServiceDetail(int id)
        {
            var p = new ServiceDetailPagePanel(id);
            p.BackRequested    += (s, e)   => NavigateTo("browse");
            p.ProfileRequested += (s, pid) => NavigateTo("profile", pid.ToString());
            p.BookingMade      += (s, e)   => { Session.Refresh(); UpdateUserArea(); NavigateTo("dashboard"); };
            return p;
        }

        private ProfilePagePanel CreateProfile(int id)
        {
            var p = new ProfilePagePanel(id);
            p.BackRequested          += (s, e)   => NavigateTo("browse");
            p.ServiceDetailRequested += (s, sid) => NavigateTo("service-detail", sid.ToString());
            return p;
        }

        private DashboardPagePanel CreateDashboard()
        {
            var p = new DashboardPagePanel();
            p.ServiceDetailRequested += (s, id) => NavigateTo("service-detail", id.ToString());
            p.WalletUpdated          += (s, e)  => { Session.Refresh(); UpdateUserArea(); };
            return p;
        }
    }
}
