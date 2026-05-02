using TutoringMarketplace.Controls;

namespace TutoringMarketplace.Forms.Pages
{
    public partial class BrowsePagePanel : Panel
    {
        public event EventHandler<int>? ServiceDetailRequested;
        public event EventHandler<int>? ProfileRequested;

        public BrowsePagePanel(string? initialCategory = null)
        {
            BackColor  = Theme.Background;
            AutoScroll = true;
            InitializeComponent(initialCategory ?? "");
        }

        private void OnServiceClick(int id) => ServiceDetailRequested?.Invoke(this, id);
        private void OnProfileClick(int id) => ProfileRequested?.Invoke(this, id);
    }
}
