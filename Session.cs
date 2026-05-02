using TutoringMarketplace.Models;

namespace TutoringMarketplace
{
    public static class Session
    {
        public static Account? CurrentUser { get; private set; }
        public static bool IsLoggedIn  => CurrentUser != null;
        public static bool IsAdmin     => CurrentUser?.IsAdmin == true;

        public static void Login(Account user)  => CurrentUser = user;
        public static void Logout()             => CurrentUser = null;
        public static void Refresh()
        {
            if (CurrentUser != null)
                CurrentUser = Database.DatabaseManager.GetAccount(CurrentUser.Id) ?? CurrentUser;
        }
    }
}
