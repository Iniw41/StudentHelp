using TutoringMarketplace.Controls;
using TutoringMarketplace.Database;

namespace TutoringMarketplace.Forms.Pages
{
    public partial class ReviewDialog : Form
    {
        private readonly int    _jobId;
        private readonly string _serviceTitle;
        private int             _selectedStars = 5;

        public ReviewDialog(int jobId, string serviceTitle)
        {
            _jobId        = jobId;
            _serviceTitle = serviceTitle;
            InitializeComponent();
        }

        private void SetStars(int stars)
        {
            _selectedStars = stars;
            for (int i = 0; i < _starButtons.Length; i++)
                _starButtons[i].ForeColor = i < stars ? Theme.StarColor : Color.FromArgb(209, 213, 219);
        }

        private void OnSubmit(object? s, EventArgs e)
        {
            string review = _txtReview.Text.Trim();
            if (review == "" || review == "Write your review here...") review = "";
            DatabaseManager.LeaveReview(_jobId, _selectedStars, review);
            MessageBox.Show("Review submitted! Thank you.", "Thanks", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
