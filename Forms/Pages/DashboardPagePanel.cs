using TutoringMarketplace.Controls;
using TutoringMarketplace.Database;
using TutoringMarketplace.Models;

namespace TutoringMarketplace.Forms.Pages
{
    public partial class DashboardPagePanel : Panel
    {
        public event EventHandler<int>? ServiceDetailRequested;
        public event EventHandler?      WalletUpdated;

        public DashboardPagePanel()
        {
            BackColor  = Theme.Background;
            AutoScroll = true;
            InitializeComponent();
        }

        private void OnServiceClick(int id) => ServiceDetailRequested?.Invoke(this, id);

        private void Refresh()
        {
            Session.Refresh();
            var parent = Parent;
            if (parent == null) return;
            parent.SuspendLayout();
            parent.Controls.Remove(this);
            var fresh = new DashboardPagePanel();
            fresh.ServiceDetailRequested += (s, id) => ServiceDetailRequested?.Invoke(this, id);
            fresh.WalletUpdated += (s, e) => WalletUpdated?.Invoke(this, EventArgs.Empty);
            fresh.Dock = DockStyle.Fill;
            parent.Controls.Add(fresh);
            parent.ResumeLayout(true);
        }

        private void OnMarkComplete(int jobId)
        {
            var confirm = MessageBox.Show("Mark this job as completed?\n\nThe tutor will receive 95% of the service fee.", "Confirm Complete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;
            DatabaseManager.UpdateJobStatus(jobId, "Completed");
            WalletUpdated?.Invoke(this, EventArgs.Empty);
            MessageBox.Show("Job marked as completed! The tutor has been paid.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Refresh();
        }

        private void OnLeaveReview(int jobId, string serviceTitle)
        {
            using var dlg = new ReviewDialog(jobId, serviceTitle);
            dlg.ShowDialog();
            Refresh();
        }

        private void OnCancelJob(int jobId)
        {
            var confirm = MessageBox.Show("Cancel this booking?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;
            DatabaseManager.UpdateJobStatus(jobId, "Cancelled");
            Refresh();
        }

        private void OnRemoveService(int serviceId, string title)
        {
            var confirm = MessageBox.Show($"Remove your service \"{title}\"?\n\nIt will no longer appear in Browse.", "Remove Service", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;
            DatabaseManager.DeleteService(serviceId);
            MessageBox.Show("Service removed.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Refresh();
        }

        private void OnTopUpWallet()
        {
            using var dlg = new WalletTopUpDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                decimal amount = dlg.Amount;
                var result = DatabaseManager.TopUpWallet(Session.CurrentUser!.Id, amount, out string error);
                if (!result)
                { MessageBox.Show(error, "Top-Up Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                Session.Refresh();
                WalletUpdated?.Invoke(this, EventArgs.Empty);
                MessageBox.Show($"₱{amount:N2} added to your wallet!", "Top-Up Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Refresh();
            }
        }
    }
}
