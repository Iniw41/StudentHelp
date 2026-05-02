using TutoringMarketplace.Controls;
using TutoringMarketplace.Database;

namespace TutoringMarketplace.Forms.Pages
{
    public partial class HomePagePanel : Panel
    {
        public event EventHandler?            BrowseRequested;
        public event EventHandler?            PostServiceRequested;
        public event EventHandler<int>?       ServiceDetailRequested;
        public event EventHandler<int>?       ProfileRequested;
        public event EventHandler<string>?    CategoryRequested;

        public HomePagePanel()
        {
            BackColor  = Theme.Background;
            AutoScroll = true;
            InitializeComponent();
        }

        private void OnSearchClick(object? s, EventArgs e)    => BrowseRequested?.Invoke(this, EventArgs.Empty);
        private void OnPostClick(object? s, EventArgs e)      => PostServiceRequested?.Invoke(this, EventArgs.Empty);
        private void OnBrowseClick(object? s, EventArgs e)    => BrowseRequested?.Invoke(this, EventArgs.Empty);
        private void OnCategoryClick(string cat)              => CategoryRequested?.Invoke(this, cat);
        private void OnServiceClick(int id)                   => ServiceDetailRequested?.Invoke(this, id);
        private void OnProfileClick(int id)                   => ProfileRequested?.Invoke(this, id);
    }
}
