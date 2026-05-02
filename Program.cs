using TutoringMarketplace;
using TutoringMarketplace.Database;
using TutoringMarketplace.Forms;

Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
Application.SetHighDpiMode(HighDpiMode.SystemAware);

DatabaseManager.Initialize();

using var login = new LoginForm();
var result = login.ShowDialog();

if (result == DialogResult.Cancel)
{
    Application.Exit();
    return;
}

// If admin logged in, show admin panel
if (Session.IsAdmin)
{
    Application.Run(new AdminForm());
    return;
}

// result == OK  → logged in as user
// result == Ignore → guest mode
Application.Run(new MainForm());
