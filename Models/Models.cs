namespace TutoringMarketplace.Models
{
    public class Account
    {
        public int    Id            { get; set; }
        public string Username      { get; set; } = "";
        public string PasswordHash  { get; set; } = "";
        public string FullName      { get; set; } = "";
        public string Email         { get; set; } = "";
        public string Major         { get; set; } = "";
        public string Year          { get; set; } = "";
        public string University    { get; set; } = "";
        public string Bio           { get; set; } = "";
        public string Skills        { get; set; } = "";
        public double Rating        { get; set; } = 5.0;
        public int    CompletedJobs { get; set; } = 0;
        public string ResponseTime  { get; set; } = "1 hour";
        public string ContactInfo   { get; set; } = "";
        public double WalletBalance { get; set; } = 0.0;
        public bool   IsAdmin       { get; set; } = false;
        public DateTime CreatedAt   { get; set; } = DateTime.Now;
    }

    public class Service
    {
        public int      Id           { get; set; }
        public string   Title        { get; set; } = "";
        public string   Description  { get; set; } = "";
        public string   Category     { get; set; } = "";
        public decimal  Price        { get; set; }
        public string   Duration     { get; set; } = "";
        public double   Rating       { get; set; } = 5.0;
        public int      ReviewCount  { get; set; } = 0;
        public int      TutorId      { get; set; }
        public string   TutorName    { get; set; } = "";
        public string   TutorMajor   { get; set; } = "";
        public string   TutorYear    { get; set; } = "";
        public string   Tags         { get; set; } = "";
        public bool     IsActive     { get; set; } = true;
        public DateTime CreatedAt    { get; set; } = DateTime.Now;

        public List<string> TagList =>
            Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim()).ToList();
    }

    public class JobTaken
    {
        public int      Id            { get; set; }
        public int      ServiceId     { get; set; }
        public int      ClientId      { get; set; }
        public int      TutorId       { get; set; }
        public string   ServiceTitle  { get; set; } = "";
        public string   ClientName    { get; set; } = "";
        public string   TutorName     { get; set; } = "";
        public string   Category      { get; set; } = "";
        public decimal  Price         { get; set; }
        public decimal  SystemFee     { get; set; }
        public decimal  TotalAmount   { get; set; }
        public string   ReferenceNumber { get; set; } = "";
        public string   Status        { get; set; } = "Pending";
        public int?     Rating        { get; set; }
        public string   Review        { get; set; } = "";
        public DateTime BookedAt      { get; set; } = DateTime.Now;
        public DateTime? CompletedAt  { get; set; }
    }

    public class Category
    {
        public string Id   { get; set; } = "";
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "";
    }

    public class Review
    {
        public string ReviewerName { get; set; } = "";
        public int    Stars        { get; set; }
        public string Date         { get; set; } = "";
        public string Comment      { get; set; } = "";
        public string ServiceName  { get; set; } = "";
    }

    public class AdminEarnings
    {
        public decimal TotalFees      { get; set; }
        public decimal WeeklyFees     { get; set; }
        public decimal MonthlyFees    { get; set; }
        public decimal AnnualFees     { get; set; }
        public int     TotalCompleted { get; set; }
    }
}
