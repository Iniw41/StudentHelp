using TutoringMarketplace.Controls;
using TutoringMarketplace.Data;
using TutoringMarketplace.Database;

namespace TutoringMarketplace.Forms.Pages
{
    partial class BrowsePagePanel
    {
        private TextBox          _searchBox = null!;
        private ComboBox         _catBox    = null!;
        private ComboBox         _priceBox  = null!;
        private ComboBox         _sortBox   = null!;
        private Label            _countLbl  = null!;
        private FlowLayoutPanel  _grid      = null!;

        private void InitializeComponent(string initialCategory)
        {
            int pw = 1860;

            Controls.Add(new Label { Text = "Browse Services", Font = Theme.FontH2, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(16, 12), BackColor = Color.Transparent });
            _countLbl = new Label { Text = "", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, AutoSize = true, Location = new Point(16, 42), BackColor = Color.Transparent };
            Controls.Add(_countLbl);

            // Filter card
            var fc = UIHelper.MakeCard(12, 66, pw - 24, 118);
            Controls.Add(fc);

            _searchBox = UIHelper.MakeTextBox("🔍  Search services…", pw - 52, 34);
            _searchBox.Location = new Point(14, 12);
            _searchBox.TextChanged += (s, e) => ApplyFilters();
            fc.Controls.Add(_searchBox);

            fc.Controls.Add(MkLbl("Category", 14, 58));
            _catBox = MkCombo(14, 76, 200);
            _catBox.Items.Add("All Categories");
            foreach (var c in MockData.Categories) _catBox.Items.Add(c.Name);
            _catBox.SelectedIndex = 0;
            if (!string.IsNullOrEmpty(initialCategory))
            {
                int idx = MockData.Categories.FindIndex(c => c.Id == initialCategory);
                if (idx >= 0) _catBox.SelectedIndex = idx + 1;
            }
            _catBox.SelectedIndexChanged += (s, e) => ApplyFilters();
            fc.Controls.Add(_catBox);

            fc.Controls.Add(MkLbl("Price (₱)", 224, 58));
            _priceBox = MkCombo(224, 76, 176);
            _priceBox.Items.AddRange(new object[] { "All Prices", "Under ₱1,000", "₱1,000 – ₱1,499", "₱1,500+" });
            _priceBox.SelectedIndex = 0;
            _priceBox.SelectedIndexChanged += (s, e) => ApplyFilters();
            fc.Controls.Add(_priceBox);

            fc.Controls.Add(MkLbl("Sort By", 410, 58));
            _sortBox = MkCombo(410, 76, 176);
            _sortBox.Items.AddRange(new object[] { "Most Popular", "Highest Rated", "Price: Low→High", "Price: High→Low" });
            _sortBox.SelectedIndex = 0;
            _sortBox.SelectedIndexChanged += (s, e) => ApplyFilters();
            fc.Controls.Add(_sortBox);

            var clearBtn = UIHelper.MakeButton("✕  Clear", Color.White, Theme.TextPrimary, 100, 34);
            clearBtn.Location = new Point(596, 76);
            clearBtn.FlatAppearance.BorderSize = 1; clearBtn.FlatAppearance.BorderColor = Theme.BorderColor;
            clearBtn.Click += (s, e) => ClearFilters();
            fc.Controls.Add(clearBtn);

            // Grid — wraps cards vertically, no horizontal scroll
            // Card width ~220, so we set FlowLayoutPanel wide enough to wrap at ~7 cols
            _grid = new FlowLayoutPanel
            {
                Location     = new Point(12, 196),
                Size         = new Size(pw - 24, 3000),
                BackColor    = Color.Transparent,
                WrapContents = true,
                AutoSize     = true,
                FlowDirection = FlowDirection.LeftToRight
            };
            Controls.Add(_grid);

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string q = _searchBox.Text.Trim();
            if (q.StartsWith("🔍")) q = "";
            int ci = _catBox.SelectedIndex;
            string selCat = ci == 0 ? "" : MockData.Categories[ci - 1].Id;
            string price  = _priceBox.SelectedIndex switch { 1 => "low", 2 => "medium", 3 => "high", _ => "" };
            string sort   = _sortBox.SelectedIndex switch  { 1 => "rating", 2 => "price-low", 3 => "price-high", _ => "popular" };

            var results = DatabaseManager.GetServices(q, selCat, price, sort);
            _countLbl.Text = $"Showing {results.Count} service{(results.Count != 1 ? "s" : "")}";

            _grid.SuspendLayout();
            _grid.Controls.Clear();
            if (results.Count == 0)
            {
                var emp = UIHelper.MakeCard(0, 0, 880, 90);
                emp.Controls.Add(new Label { Text = "No services match your filters. Try adjusting your search.", Font = Theme.FontBody, ForeColor = Theme.TextSecondary, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });
                _grid.Controls.Add(emp);
            }
            else
            {
                foreach (var svc in results)
                {
                    var card = new ServiceCard(svc);
                    card.ServiceClicked += (s, id) => OnServiceClick(id);
                    card.ProfileClicked += (s, id) => OnProfileClick(id);
                    _grid.Controls.Add(card);
                }
            }
            _grid.ResumeLayout(true);
        }

        private void ClearFilters()
        {
            _searchBox.Text = "🔍  Search services…"; _searchBox.ForeColor = Theme.TextSecondary;
            _catBox.SelectedIndex = 0; _priceBox.SelectedIndex = 0; _sortBox.SelectedIndex = 0;
            ApplyFilters();
        }

        private static Label MkLbl(string t, int x, int y) => new() { Text = t, Font = Theme.FontBold, ForeColor = Theme.TextPrimary, AutoSize = true, Location = new Point(x, y), BackColor = Color.Transparent };
        private static ComboBox MkCombo(int x, int y, int w) => new() { Location = new Point(x, y), Size = new Size(w, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontBody, FlatStyle = FlatStyle.Flat };
    }
}
