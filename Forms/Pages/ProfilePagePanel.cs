using TutoringMarketplace.Controls;
using TutoringMarketplace.Database;

namespace TutoringMarketplace.Forms.Pages
{
    public partial class ProfilePagePanel : Panel
    {
        public event EventHandler?      BackRequested;
        public event EventHandler<int>? ServiceDetailRequested;

        private readonly int _studentId;

        public ProfilePagePanel(int studentId)
        {
            _studentId = studentId;
            BackColor  = Theme.Background;
            AutoScroll = true;
            InitializeComponent();
        }

        private void OnBack(object? s, EventArgs e)      => BackRequested?.Invoke(this, EventArgs.Empty);
        private void OnServiceClick(int id)               => ServiceDetailRequested?.Invoke(this, id);

        private void OnContact(object? s, EventArgs e)
        {
            var student = DatabaseManager.GetAccount(_studentId);
            if (student == null) return;
            MessageBox.Show($"Contacting {student.FullName}\nResponse time: {student.ResponseTime}\n\nIn a real app this would open a messaging thread.",
                "Contact", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
