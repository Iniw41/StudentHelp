using TutoringMarketplace.Controls;
using TutoringMarketplace.Database;

namespace TutoringMarketplace.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (user == "" || user == "Username" || pass == "" || pass == "Password")
            {
                lblError.Text = "Please enter your username and password.";
                lblError.Visible = true;
                return;
            }

            var account = DatabaseManager.Login(user, pass);
            if (account == null)
            {
                lblError.Text = "Invalid username or password.";
                lblError.Visible = true;
                return;
            }

            if (account.IsAdmin)
            {
                lblError.Text = "Use the Admin Login button for admin accounts.";
                lblError.Visible = true;
                return;
            }

            Session.Login(account);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnAdminLogin_Click(object sender, EventArgs e)
        {
            using var adminDlg = new AdminLoginForm();
            if (adminDlg.ShowDialog() == DialogResult.OK)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using var reg = new RegisterForm();
            if (reg.ShowDialog() == DialogResult.OK)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnGuest_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore; // guest mode
            Close();
        }
    }
}
