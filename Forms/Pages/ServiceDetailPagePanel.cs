using TutoringMarketplace.Controls;
using TutoringMarketplace.Database;
using TutoringMarketplace.Forms;

namespace TutoringMarketplace.Forms.Pages
{
    public partial class ServiceDetailPagePanel : Panel
    {
        public event EventHandler?      BackRequested;
        public event EventHandler<int>? ProfileRequested;
        public event EventHandler?      BookingMade;

        private readonly int _serviceId;

        public ServiceDetailPagePanel(int serviceId)
        {
            _serviceId = serviceId;
            BackColor  = Theme.Background;
            AutoScroll = true;
            InitializeComponent();
        }

        private void OnBack(object? s, EventArgs e)    => BackRequested?.Invoke(this, EventArgs.Empty);
        private void OnProfile(int id)                  => ProfileRequested?.Invoke(this, id);

        private void OnContact(object? s, EventArgs e)
        {
            if (!Session.IsLoggedIn)
            {
                MessageBox.Show("Please log in to contact the tutor.", "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var svc   = DatabaseManager.GetService(_serviceId);
            var tutor = svc != null ? DatabaseManager.GetAccount(svc.TutorId) : null;
            if (tutor == null) return;

            // Record that this user has "contacted" — we use BookService flow for actual payment
            // Just show contact info
            string contactDetails = string.IsNullOrWhiteSpace(tutor.ContactInfo)
                ? $"📧  Email: {tutor.Email}\n\n(This tutor has not added extra contact info yet.)"
                : $"📋  Contact Details:\n\n{tutor.ContactInfo}\n\n📧  Email: {tutor.Email}";

            MessageBox.Show(
                $"How to reach {tutor.FullName}\n" +
                $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                contactDetails +
                $"\n\n⏱  Typical response time: {tutor.ResponseTime}",
                "Contact Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnBook(object? s, EventArgs e)
        {
            if (!Session.IsLoggedIn)
            {
                MessageBox.Show("Please log in to book a service.", "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var svc = DatabaseManager.GetService(_serviceId);
            if (svc == null) return;

            Session.Refresh();
            var user = Session.CurrentUser!;
            var tutor = DatabaseManager.GetAccount(svc.TutorId);
            if (tutor == null) return;

            // Generate reference number upfront for the receipt
            string referenceNumber = DatabaseManager.GenerateReferenceNumber();

            using var receiptDialog = new ReceiptDialog(svc, tutor, user, referenceNumber);
            if (receiptDialog.ShowDialog() != DialogResult.OK || !receiptDialog.Confirmed)
            {
                return;
            }

            var (success, error, refNum) = DatabaseManager.BookService(_serviceId, user.Id);
            if (!success)
            {
                MessageBox.Show(error, "Booking Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show(
                $"Booking confirmed!\n\n" +
                $"Reference Number: {refNum}\n\n" +
                $"Check your Dashboard to track progress.",
                "Booking Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            BookingMade?.Invoke(this, EventArgs.Empty);
        }
    }
}
