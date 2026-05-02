using TutoringMarketplace.Controls;
using TutoringMarketplace.Database;
using TutoringMarketplace.Models;

namespace TutoringMarketplace.Forms
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        // Returns the real typed value, or "" if the field still holds placeholder text.
        private static string Val(TextBox tb)
        {
            string t = tb.Text.Trim();
            // Placeholder text colour is TextSecondary; actual input uses TextPrimary.
            return tb.ForeColor == Theme.TextSecondary ? "" : t;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            string fullName   = Val(txtFullName);
            string username   = Val(txtUsername);
            string email      = Val(txtEmail);
            string password   = Val(txtPassword);
            string confirm    = Val(txtConfirm);
            string university = Val(txtUniversity);
            string major      = Val(txtMajor);
            string year       = cmbYear.SelectedIndex > 0 ? cmbYear.Text.Trim() : "";
            string contact    = Val(txtContact);

            // Required-field check with friendly per-field messages
            if (string.IsNullOrWhiteSpace(fullName))  { ShowError("Please enter your full name.");       return; }
            if (string.IsNullOrWhiteSpace(username))  { ShowError("Please choose a username.");          return; }
            if (string.IsNullOrWhiteSpace(email))     { ShowError("Please enter your email address.");   return; }
            if (string.IsNullOrWhiteSpace(password))  { ShowError("Please enter a password.");           return; }
            if (string.IsNullOrWhiteSpace(confirm))   { ShowError("Please confirm your password.");      return; }

            if (password != confirm)    { ShowError("Passwords do not match.");                return; }
            if (password.Length < 6)    { ShowError("Password must be at least 6 characters."); return; }
            if (!email.Contains('@'))   { ShowError("Please enter a valid email address.");    return; }

            var account = new Account
            {
                FullName    = fullName,
                Username    = username,
                Email       = email,
                University  = university,
                Major       = major,
                Year        = year,
                ContactInfo = contact,
            };

            if (!DatabaseManager.Register(account, password, out var err))
            {
                ShowError(err);
                return;
            }

            var logged = DatabaseManager.Login(username, password);
            if (logged != null) Session.Login(logged);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ShowError(string msg)
        {
            lblError.Text    = msg;
            lblError.Visible = true;
        }

        private void btnCancel_Click(object sender, EventArgs e) => Close();
    }
}
