using Microsoft.Data.Sqlite;
using TutoringMarketplace.Models;
using System.Security.Cryptography;
using System.Text;

namespace TutoringMarketplace.Database
{
    public static class DatabaseManager
    {
        private static string _dbPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "database", "marketplace.db");

        public static string DbPath => _dbPath;

        private static SqliteConnection GetConnection()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_dbPath)!);
            var conn = new SqliteConnection($"Data Source={_dbPath}");
            conn.Open();
            using var wal = conn.CreateCommand();
            wal.CommandText = "PRAGMA journal_mode=WAL; PRAGMA foreign_keys=ON;";
            wal.ExecuteNonQuery();
            return conn;
        }

        

        private static void MigrateColumn(SqliteConnection conn, string table, string column, string definition)
        {
            try { using var cmd = conn.CreateCommand(); cmd.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {definition}"; cmd.ExecuteNonQuery(); }
            catch { }
        }

        private static void InitializeDatabase()
        {
            try
            {
                using var conn = GetConnection();
                // Add Category column to JobsTaken if it doesn't exist
                MigrateColumn(conn, "JobsTaken", "Category", "TEXT DEFAULT ''");
            }
            catch { }
        }


        public static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password + "studenthelp_salt"));
            return Convert.ToHexString(bytes);
        }

        public static Account? Login(string username, string password)
        {
            using var conn = GetConnection(); using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Accounts WHERE Username=$u AND PasswordHash=$ph LIMIT 1";
            cmd.Parameters.AddWithValue("$u", username.ToLower().Trim()); cmd.Parameters.AddWithValue("$ph", HashPassword(password));
            using var r = cmd.ExecuteReader(); return r.Read() ? ReadAccount(r) : null;
        }

        public static bool Register(Account a, string plainPassword, out string error)
        {
            error = "";
            using var conn = GetConnection(); using var check = conn.CreateCommand();
            check.CommandText = "SELECT COUNT(*) FROM Accounts WHERE Username=$u OR Email=$e";
            check.Parameters.AddWithValue("$u", a.Username.ToLower().Trim()); check.Parameters.AddWithValue("$e", a.Email.ToLower().Trim());
            if ((long)(check.ExecuteScalar() ?? 0L) > 0) { error = "Username or email already taken."; return false; }
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Accounts (Username,PasswordHash,FullName,Email,Major,Year,University,Bio,Skills,ContactInfo,WalletBalance) VALUES ($u,$ph,$fn,$em,$mj,$yr,$uni,$bio,$sk,$ci,0)";
            cmd.Parameters.AddWithValue("$u",a.Username.ToLower().Trim()); cmd.Parameters.AddWithValue("$ph",HashPassword(plainPassword));
            cmd.Parameters.AddWithValue("$fn",a.FullName); cmd.Parameters.AddWithValue("$em",a.Email.ToLower().Trim());
            cmd.Parameters.AddWithValue("$mj",a.Major); cmd.Parameters.AddWithValue("$yr",a.Year);
            cmd.Parameters.AddWithValue("$uni",a.University); cmd.Parameters.AddWithValue("$bio",a.Bio);
            cmd.Parameters.AddWithValue("$sk",a.Skills); cmd.Parameters.AddWithValue("$ci",a.ContactInfo);
            cmd.ExecuteNonQuery(); return true;
        }

        public static void UpdateContactInfo(int accountId, string contactInfo)
        {
            using var conn = GetConnection(); using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Accounts SET ContactInfo=$ci WHERE Id=$id";
            cmd.Parameters.AddWithValue("$ci",contactInfo); cmd.Parameters.AddWithValue("$id",accountId); cmd.ExecuteNonQuery();
        }

        public static Account? GetAccount(int id)
        {
            using var conn = GetConnection(); using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Accounts WHERE Id=$id"; cmd.Parameters.AddWithValue("$id",id);
            using var r = cmd.ExecuteReader(); return r.Read() ? ReadAccount(r) : null;
        }

        public static List<Account> GetAllTutors()
        {
            var list = new List<Account>(); using var conn = GetConnection(); using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Accounts WHERE IsAdmin=0 ORDER BY Rating DESC";
            using var r = cmd.ExecuteReader(); while (r.Read()) list.Add(ReadAccount(r)); return list;
        }

        public static bool TopUpWallet(int accountId, decimal amount, out string error)
        {
            error = "";
            if (amount <= 0) { error = "Amount must be greater than zero."; return false; }
            if (amount > 50000) { error = "Maximum top-up per transaction is ₱50,000."; return false; }
            using var conn = GetConnection(); using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Accounts SET WalletBalance=WalletBalance+$amt WHERE Id=$id";
            cmd.Parameters.AddWithValue("$amt",(double)amount); cmd.Parameters.AddWithValue("$id",accountId); cmd.ExecuteNonQuery();
            LogWalletTransaction(conn, accountId, amount, "TopUp", $"Wallet top-up of ₱{amount:N2}"); return true;
        }

        private static void LogWalletTransaction(SqliteConnection conn, int accountId, decimal amount, string type, string description)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO WalletTransactions (AccountId,Amount,Type,Description) VALUES ($aid,$amt,$t,$d)";
            cmd.Parameters.AddWithValue("$aid",accountId); cmd.Parameters.AddWithValue("$amt",(double)amount);
            cmd.Parameters.AddWithValue("$t",type); cmd.Parameters.AddWithValue("$d",description); cmd.ExecuteNonQuery();
        }

        public static List<Service> GetServices(string search="",string category="",string priceRange="",string sortBy="popular")
        {
            using var conn=GetConnection(); using var cmd=conn.CreateCommand();
            var where=new List<string>{"s.IsActive=1"};
            if(!string.IsNullOrWhiteSpace(search)){where.Add("(s.Title LIKE $q OR s.Description LIKE $q OR s.Tags LIKE $q)");cmd.Parameters.AddWithValue("$q",$"%{search}%");}
            if(!string.IsNullOrWhiteSpace(category)&&category!="all"){where.Add("s.Category=$cat");cmd.Parameters.AddWithValue("$cat",category);}
            switch(priceRange){case"low":where.Add("s.Price<1000");break;case"medium":where.Add("s.Price BETWEEN 1000 AND 1499");break;case"high":where.Add("s.Price>=1500");break;}
            var order=sortBy switch{"rating"=>"s.Rating DESC","price-low"=>"s.Price ASC","price-high"=>"s.Price DESC",_=>"s.ReviewCount DESC"};
            cmd.CommandText=$@"SELECT s.*,a.FullName as TutorName,a.Major as TutorMajor,a.Year as TutorYear FROM Services s JOIN Accounts a ON s.TutorId=a.Id WHERE {string.Join(" AND ",where)} ORDER BY {order}";
            var list=new List<Service>(); using var r=cmd.ExecuteReader(); while(r.Read())list.Add(ReadService(r)); return list;
        }

        public static List<Service> GetAllServicesAdmin()
        {
            using var conn=GetConnection(); using var cmd=conn.CreateCommand();
            cmd.CommandText=@"SELECT s.*,a.FullName as TutorName,a.Major as TutorMajor,a.Year as TutorYear FROM Services s JOIN Accounts a ON s.TutorId=a.Id ORDER BY s.CreatedAt DESC";
            var list=new List<Service>(); using var r=cmd.ExecuteReader(); while(r.Read())list.Add(ReadService(r)); return list;
        }

        public static Service? GetService(int id)
        {
            using var conn=GetConnection(); using var cmd=conn.CreateCommand();
            cmd.CommandText=@"SELECT s.*,a.FullName as TutorName,a.Major as TutorMajor,a.Year as TutorYear FROM Services s JOIN Accounts a ON s.TutorId=a.Id WHERE s.Id=$id";
            cmd.Parameters.AddWithValue("$id",id); using var r=cmd.ExecuteReader(); return r.Read()?ReadService(r):null;
        }

        public static List<Service> GetServicesByTutor(int tutorId)
        {
            using var conn=GetConnection(); using var cmd=conn.CreateCommand();
            cmd.CommandText=@"SELECT s.*,a.FullName as TutorName,a.Major as TutorMajor,a.Year as TutorYear FROM Services s JOIN Accounts a ON s.TutorId=a.Id WHERE s.TutorId=$tid AND s.IsActive=1 ORDER BY s.CreatedAt DESC";
            cmd.Parameters.AddWithValue("$tid",tutorId); var list=new List<Service>(); using var r=cmd.ExecuteReader(); while(r.Read())list.Add(ReadService(r)); return list;
        }

        public static int CreateService(Service svc)
        {
            using var conn=GetConnection(); using var cmd=conn.CreateCommand();
            cmd.CommandText=@"INSERT INTO Services (Title,Description,Category,Price,Duration,TutorId,Tags) VALUES ($t,$d,$c,$p,$dur,$tid,$tags); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$t",svc.Title); cmd.Parameters.AddWithValue("$d",svc.Description);
            cmd.Parameters.AddWithValue("$c",svc.Category); cmd.Parameters.AddWithValue("$p",(double)svc.Price);
            cmd.Parameters.AddWithValue("$dur",svc.Duration); cmd.Parameters.AddWithValue("$tid",svc.TutorId);
            cmd.Parameters.AddWithValue("$tags",svc.Tags); return (int)(long)(cmd.ExecuteScalar()??0L);
        }

        public static void DeleteService(int serviceId)
        {
            using var conn=GetConnection(); using var cmd=conn.CreateCommand();
            cmd.CommandText="UPDATE Services SET IsActive=0 WHERE Id=$id";
            cmd.Parameters.AddWithValue("$id",serviceId); cmd.ExecuteNonQuery();
        }

        public static string GenerateReferenceNumber()
        {
            var random = new Random();
            return random.Next(10000000, 99999999).ToString();
        }

        public static (bool success, string error, string? referenceNumber) BookService(int serviceId, int clientId)
        {
            var svc=GetService(serviceId); if(svc==null)return(false,"Service not found.", null);
            var client=GetAccount(clientId); if(client==null)return(false,"Account not found.", null);
            if(client.WalletBalance<(double)svc.Price)
                return(false,$"Insufficient wallet balance. You need ₱{svc.Price:N2} but only have ₱{client.WalletBalance:N2}.\nPlease top up your wallet from My Dashboard.", null);

            decimal systemFee = svc.Price * 0.05m;
            decimal totalAmount = svc.Price + systemFee;
            string referenceNumber = GenerateReferenceNumber();

            using var conn=GetConnection();
            using var deduct=conn.CreateCommand();
            deduct.CommandText="UPDATE Accounts SET WalletBalance=WalletBalance-$amt WHERE Id=$id";
            deduct.Parameters.AddWithValue("$amt",(double)totalAmount); deduct.Parameters.AddWithValue("$id",clientId); deduct.ExecuteNonQuery();
            LogWalletTransaction(conn,clientId,-totalAmount,"Booking",$"Booked: {svc.Title} (Ref: {referenceNumber})");
            using var cmd=conn.CreateCommand();
            cmd.CommandText=@"INSERT INTO JobsTaken (ServiceId,ClientId,TutorId,ServiceTitle,Category,Price,SystemFee,TotalAmount,ReferenceNumber) VALUES ($sid,$cid,$tid,$st,$cat,$p,$fee,$total,$ref); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$sid",serviceId); cmd.Parameters.AddWithValue("$cid",clientId);
            cmd.Parameters.AddWithValue("$tid",svc.TutorId); cmd.Parameters.AddWithValue("$st",svc.Title);
            cmd.Parameters.AddWithValue("$cat",svc.Category);
            cmd.Parameters.AddWithValue("$p",(double)svc.Price); cmd.Parameters.AddWithValue("$fee",(double)systemFee);
            cmd.Parameters.AddWithValue("$total",(double)totalAmount); cmd.Parameters.AddWithValue("$ref",referenceNumber);
            cmd.ExecuteScalar(); return(true,"",referenceNumber);
        }

        public static List<JobTaken> GetJobsTakenByClient(int clientId)
        {
            using var conn=GetConnection(); using var cmd=conn.CreateCommand();
            cmd.CommandText=@"SELECT j.*,ca.FullName as ClientName,ta.FullName as TutorName FROM JobsTaken j JOIN Accounts ca ON j.ClientId=ca.Id JOIN Accounts ta ON j.TutorId=ta.Id WHERE j.ClientId=$cid ORDER BY j.BookedAt DESC";
            cmd.Parameters.AddWithValue("$cid",clientId); var list=new List<JobTaken>(); using var r=cmd.ExecuteReader(); while(r.Read())list.Add(ReadJob(r)); return list;
        }

        public static List<JobTaken> GetJobsCreatedByTutor(int tutorId)
        {
            using var conn=GetConnection(); using var cmd=conn.CreateCommand();
            cmd.CommandText=@"SELECT j.*,ca.FullName as ClientName,ta.FullName as TutorName FROM JobsTaken j JOIN Accounts ca ON j.ClientId=ca.Id JOIN Accounts ta ON j.TutorId=ta.Id WHERE j.TutorId=$tid ORDER BY j.BookedAt DESC";
            cmd.Parameters.AddWithValue("$tid",tutorId); var list=new List<JobTaken>(); using var r=cmd.ExecuteReader(); while(r.Read())list.Add(ReadJob(r)); return list;
        }

        public static List<JobTaken> GetAllJobs()
        {
            using var conn=GetConnection(); using var cmd=conn.CreateCommand();
            cmd.CommandText=@"SELECT j.*,ca.FullName as ClientName,ta.FullName as TutorName FROM JobsTaken j JOIN Accounts ca ON j.ClientId=ca.Id JOIN Accounts ta ON j.TutorId=ta.Id ORDER BY j.BookedAt DESC";
            var list=new List<JobTaken>(); using var r=cmd.ExecuteReader(); while(r.Read())list.Add(ReadJob(r)); return list;
        }

        public static void UpdateJobStatus(int jobId, string status)
        {
            using var conn=GetConnection(); using var cmd=conn.CreateCommand();
            cmd.CommandText=status=="Completed"?"UPDATE JobsTaken SET Status=$s,CompletedAt=datetime('now') WHERE Id=$id":"UPDATE JobsTaken SET Status=$s WHERE Id=$id";
            cmd.Parameters.AddWithValue("$s",status); cmd.Parameters.AddWithValue("$id",jobId); cmd.ExecuteNonQuery();
            if(status=="Completed"){
                using var getJob=conn.CreateCommand();
                getJob.CommandText="SELECT TutorId,Price,TotalAmount FROM JobsTaken WHERE Id=$id"; getJob.Parameters.AddWithValue("$id",jobId);
                using var jr=getJob.ExecuteReader();
                if(jr.Read()){
                    int tutorId=jr.GetInt32(0); double totalAmount=jr.GetDouble(2); double payout=totalAmount*0.95;
                    jr.Close();
                    using var credit=conn.CreateCommand();
                    credit.CommandText="UPDATE Accounts SET WalletBalance=WalletBalance+$amt,CompletedJobs=CompletedJobs+1 WHERE Id=$tid";
                    credit.Parameters.AddWithValue("$amt",payout); credit.Parameters.AddWithValue("$tid",tutorId); credit.ExecuteNonQuery();
                    LogWalletTransaction(conn,tutorId,(decimal)payout,"Earning",$"Completed job payout (95% of ₱{totalAmount:N2})");
                }
            }
        }

        public static void LeaveReview(int jobId, int stars, string review)
        {
            using var conn=GetConnection(); using var cmd=conn.CreateCommand();
            cmd.CommandText="UPDATE JobsTaken SET Rating=$r,Review=$rev WHERE Id=$id";
            cmd.Parameters.AddWithValue("$r",stars); cmd.Parameters.AddWithValue("$rev",review); cmd.Parameters.AddWithValue("$id",jobId); cmd.ExecuteNonQuery();
            using var recalc=conn.CreateCommand();
            recalc.CommandText=@"UPDATE Services SET Rating=(SELECT AVG(CAST(Rating AS REAL)) FROM JobsTaken WHERE ServiceId=Services.Id AND Rating IS NOT NULL),ReviewCount=(SELECT COUNT(*) FROM JobsTaken WHERE ServiceId=Services.Id AND Rating IS NOT NULL) WHERE Id=(SELECT ServiceId FROM JobsTaken WHERE Id=$id)";
            recalc.Parameters.AddWithValue("$id",jobId); recalc.ExecuteNonQuery();
        }

        public static void AddServiceReview(int serviceId, int clientId, int rating, string comment)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Reviews (ServiceId, ClientId, Rating, Comment) VALUES ($sid, $cid, $r, $c)";
            cmd.Parameters.AddWithValue("$sid", serviceId);
            cmd.Parameters.AddWithValue("$cid", clientId);
            cmd.Parameters.AddWithValue("$r", rating);
            cmd.Parameters.AddWithValue("$c", comment);
            cmd.ExecuteNonQuery();

            // Recalculate service rating and review count from all reviews
            using var recalc = conn.CreateCommand();
            recalc.CommandText = @"UPDATE Services SET
                Rating = (SELECT AVG(CAST(Rating AS REAL)) FROM Reviews WHERE ServiceId = Services.Id),
                ReviewCount = (SELECT COUNT(*) FROM Reviews WHERE ServiceId = Services.Id)
                WHERE Id = $sid";
            recalc.Parameters.AddWithValue("$sid", serviceId);
            recalc.ExecuteNonQuery();
        }

        public static List<(int clientId, string clientName, int rating, string comment, DateTime createdAt)> GetServiceReviews(int serviceId)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT r.ClientId, a.FullName, r.Rating, r.Comment, r.CreatedAt
                FROM Reviews r JOIN Accounts a ON r.ClientId = a.Id
                WHERE r.ServiceId = $sid ORDER BY r.CreatedAt DESC";
            cmd.Parameters.AddWithValue("$sid", serviceId);
            var reviews = new List<(int, string, int, string, DateTime)>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                reviews.Add((
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetInt32(2),
                    reader.IsDBNull(3) ? "" : reader.GetString(3),
                    DateTime.Parse(reader.GetString(4))
                ));
            }
            return reviews;
        }

        public static bool HasUserReviewedService(int clientId, int serviceId)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Reviews WHERE ClientId=$cid AND ServiceId=$sid";
            cmd.Parameters.AddWithValue("$cid", clientId);
            cmd.Parameters.AddWithValue("$sid", serviceId);
            return (long)(cmd.ExecuteScalar() ?? 0L) > 0;
        }

        public static AdminEarnings GetAdminEarnings()
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT
                COALESCE(SUM(SystemFee), 0) as TotalFees,
                COALESCE(SUM(CASE WHEN CompletedAt >= date('now','-7 days') THEN SystemFee ELSE 0 END), 0) as WeeklyFees,
                COALESCE(SUM(CASE WHEN CompletedAt >= date('now','start of month') THEN SystemFee ELSE 0 END), 0) as MonthlyFees,
                COALESCE(SUM(CASE WHEN CompletedAt >= date('now','start of year') THEN SystemFee ELSE 0 END), 0) as AnnualFees,
                COUNT(*) as TotalCompleted
                FROM JobsTaken WHERE Status='Completed'";
            using var r = cmd.ExecuteReader();
            if (r.Read())
                return new AdminEarnings
                {
                    TotalFees = r.IsDBNull(0) ? 0m : (decimal)r.GetDouble(0),
                    WeeklyFees = r.IsDBNull(1) ? 0m : (decimal)r.GetDouble(1),
                    MonthlyFees = r.IsDBNull(2) ? 0m : (decimal)r.GetDouble(2),
                    AnnualFees = r.IsDBNull(3) ? 0m : (decimal)r.GetDouble(3),
                    TotalCompleted = r.IsDBNull(4) ? 0 : r.GetInt32(4)
                };
            return new AdminEarnings();
        }

        public static AdminEarnings GetPendingEarnings()
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT
                COALESCE(SUM(SystemFee), 0) as PendingFees,
                COUNT(*) as PendingCount
                FROM JobsTaken WHERE Status IN ('Active', 'Pending')";
            using var r = cmd.ExecuteReader();
            if (r.Read())
                return new AdminEarnings
                {
                    TotalFees = r.IsDBNull(0) ? 0m : (decimal)r.GetDouble(0),
                    TotalCompleted = r.IsDBNull(1) ? 0 : r.GetInt32(1)
                };
            return new AdminEarnings();
        }

        public static Dictionary<string, decimal> GetEarningsByCategory(string timePeriod)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();

            string dateCondition = timePeriod switch
            {
                "weekly"  => "j.CompletedAt >= date('now', '-7 days')",
                "monthly" => "j.CompletedAt >= date('now', 'start of month')",
                "annual"  => "j.CompletedAt >= date('now', 'start of year')",
                _         => "1=1"
            };

            cmd.CommandText = $@"
                SELECT s.Category, COALESCE(SUM(j.SystemFee), 0) as Earnings
                FROM JobsTaken j
                JOIN Services s ON j.ServiceId = s.Id
                WHERE j.Status = 'Completed' AND {dateCondition}
                GROUP BY s.Category
                ORDER BY Earnings DESC";

            var result = new Dictionary<string, decimal>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string category = reader.GetString(0);
                decimal earnings = reader.IsDBNull(1) ? 0m : (decimal)reader.GetDouble(1);
                result[category] = earnings;
            }
            return result;
        }

        public static Dictionary<string, decimal> GetPendingEarningsByCategory()
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = $@"
                SELECT s.Category, COALESCE(SUM(j.SystemFee), 0) as Earnings
                FROM JobsTaken j
                JOIN Services s ON j.ServiceId = s.Id
                WHERE j.Status IN ('Active', 'Pending')
                GROUP BY s.Category
                ORDER BY Earnings DESC";

            var result = new Dictionary<string, decimal>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string category = reader.GetString(0);
                decimal earnings = reader.IsDBNull(1) ? 0m : (decimal)reader.GetDouble(1);
                result[category] = earnings;
            }
            return result;
        }

        public static bool HasBookedService(int clientId, int serviceId)
        {
            using var conn=GetConnection(); using var cmd=conn.CreateCommand();
            cmd.CommandText="SELECT COUNT(*) FROM JobsTaken WHERE ClientId=$cid AND ServiceId=$sid AND Status='Completed'";
            cmd.Parameters.AddWithValue("$cid",clientId); cmd.Parameters.AddWithValue("$sid",serviceId);
            return (long)(cmd.ExecuteScalar()??0L)>0;
        }

        public static JobTaken? GetJobByReferenceNumber(string referenceNumber)
        {
            using var conn=GetConnection(); using var cmd=conn.CreateCommand();
            cmd.CommandText="SELECT * FROM JobsTaken WHERE ReferenceNumber=$ref";
            cmd.Parameters.AddWithValue("$ref",referenceNumber);
            using var r=cmd.ExecuteReader();
            return r.Read() ? ReadJob(r) : null;
        }

        public static (bool success, string error) ProcessRefund(string referenceNumber)
        {
            var job = GetJobByReferenceNumber(referenceNumber);
            if (job == null) return (false, "No booking found with reference number: " + referenceNumber);
            if (job.Status == "Refunded") return (false, "This booking has already been refunded.");
            if (job.Status == "Pending") return (false, "Cannot refund a pending booking. Please cancel it first.");

            using var conn = GetConnection();
            // Refund the total amount to the client
            using var refund = conn.CreateCommand();
            refund.CommandText = "UPDATE Accounts SET WalletBalance=WalletBalance+$amt WHERE Id=$id";
            refund.Parameters.AddWithValue("$amt", (double)job.TotalAmount);
            refund.Parameters.AddWithValue("$id", job.ClientId);
            refund.ExecuteNonQuery();

            // Update job status to Refunded
            using var updateJob = conn.CreateCommand();
            updateJob.CommandText = "UPDATE JobsTaken SET Status='Refunded' WHERE Id=$id";
            updateJob.Parameters.AddWithValue("$id", job.Id);
            updateJob.ExecuteNonQuery();

            // Log the refund transaction
            LogWalletTransaction(conn, job.ClientId, job.TotalAmount, "Refund", $"Refund for booking {referenceNumber} - {job.ServiceTitle}");

            return (true, $"Refund of ₱{job.TotalAmount:N2} processed successfully for booking {referenceNumber}");
        }

        public static void Initialize()
        {
            using var conn = GetConnection();
            // Ensure the column exists
            MigrateColumn(conn, "JobsTaken", "Category", "TEXT DEFAULT ''");
            // Optionally call SeedData(conn) if you want initial demo data when DB is empty
        }

        private static Account ReadAccount(SqliteDataReader r)=>new(){
            Id=r.GetInt32(r.GetOrdinal("Id")),Username=r.GetString(r.GetOrdinal("Username")),PasswordHash=r.GetString(r.GetOrdinal("PasswordHash")),
            FullName=r.GetString(r.GetOrdinal("FullName")),Email=r.GetString(r.GetOrdinal("Email")),
            Major=r.IsDBNull(r.GetOrdinal("Major"))?"":r.GetString(r.GetOrdinal("Major")),Year=r.IsDBNull(r.GetOrdinal("Year"))?"":r.GetString(r.GetOrdinal("Year")),
            University=r.IsDBNull(r.GetOrdinal("University"))?"":r.GetString(r.GetOrdinal("University")),Bio=r.IsDBNull(r.GetOrdinal("Bio"))?"":r.GetString(r.GetOrdinal("Bio")),
            Skills=r.IsDBNull(r.GetOrdinal("Skills"))?"":r.GetString(r.GetOrdinal("Skills")),Rating=r.GetDouble(r.GetOrdinal("Rating")),
            CompletedJobs=r.GetInt32(r.GetOrdinal("CompletedJobs")),ResponseTime=r.IsDBNull(r.GetOrdinal("ResponseTime"))?"1 hour":r.GetString(r.GetOrdinal("ResponseTime")),
            ContactInfo=r.IsDBNull(r.GetOrdinal("ContactInfo"))?"":r.GetString(r.GetOrdinal("ContactInfo")),
            WalletBalance=r.IsDBNull(r.GetOrdinal("WalletBalance"))?0.0:r.GetDouble(r.GetOrdinal("WalletBalance")),
            IsAdmin=!r.IsDBNull(r.GetOrdinal("IsAdmin"))&&r.GetInt32(r.GetOrdinal("IsAdmin"))==1,
        };

        private static Service ReadService(SqliteDataReader r)=>new(){
            Id=r.GetInt32(r.GetOrdinal("Id")),Title=r.GetString(r.GetOrdinal("Title")),Description=r.GetString(r.GetOrdinal("Description")),
            Category=r.GetString(r.GetOrdinal("Category")),Price=(decimal)r.GetDouble(r.GetOrdinal("Price")),Duration=r.GetString(r.GetOrdinal("Duration")),
            Rating=r.GetDouble(r.GetOrdinal("Rating")),ReviewCount=r.GetInt32(r.GetOrdinal("ReviewCount")),TutorId=r.GetInt32(r.GetOrdinal("TutorId")),
            TutorName=r.IsDBNull(r.GetOrdinal("TutorName"))?"":r.GetString(r.GetOrdinal("TutorName")),TutorMajor=r.IsDBNull(r.GetOrdinal("TutorMajor"))?"":r.GetString(r.GetOrdinal("TutorMajor")),
            TutorYear=r.IsDBNull(r.GetOrdinal("TutorYear"))?"":r.GetString(r.GetOrdinal("TutorYear")),Tags=r.IsDBNull(r.GetOrdinal("Tags"))?"":r.GetString(r.GetOrdinal("Tags")),
            IsActive=r.GetInt32(r.GetOrdinal("IsActive"))==1,
        };

        private static bool HasColumn(SqliteDataReader r, string name)
        {
            for (int i = 0; i < r.FieldCount; i++)
                if (string.Equals(r.GetName(i), name, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private static JobTaken ReadJob(SqliteDataReader r)
        {
            int idxCategory = -1;
            if (HasColumn(r, "Category")) idxCategory = r.GetOrdinal("Category");

            return new JobTaken {
                Id = r.GetInt32(r.GetOrdinal("Id")),
                ServiceId = r.GetInt32(r.GetOrdinal("ServiceId")),
                ClientId = r.GetInt32(r.GetOrdinal("ClientId")),
                TutorId = r.GetInt32(r.GetOrdinal("TutorId")),
                ServiceTitle = r.GetString(r.GetOrdinal("ServiceTitle")),
                ClientName = r.IsDBNull(r.GetOrdinal("ClientName")) ? "" : r.GetString(r.GetOrdinal("ClientName")),
                TutorName = r.IsDBNull(r.GetOrdinal("TutorName")) ? "" : r.GetString(r.GetOrdinal("TutorName")),
                Category = (idxCategory >= 0 && !r.IsDBNull(idxCategory)) ? r.GetString(idxCategory) : "",
                Price = (decimal)r.GetDouble(r.GetOrdinal("Price")),
                SystemFee = r.IsDBNull(r.GetOrdinal("SystemFee")) ? 0m : (decimal)r.GetDouble(r.GetOrdinal("SystemFee")),
                TotalAmount = r.IsDBNull(r.GetOrdinal("TotalAmount")) ? 0m : (decimal)r.GetDouble(r.GetOrdinal("TotalAmount")),
                ReferenceNumber = r.IsDBNull(r.GetOrdinal("ReferenceNumber")) ? "" : r.GetString(r.GetOrdinal("ReferenceNumber")),
                Status = r.GetString(r.GetOrdinal("Status")),
                Rating = r.IsDBNull(r.GetOrdinal("Rating")) ? null : r.GetInt32(r.GetOrdinal("Rating")),
                Review = r.IsDBNull(r.GetOrdinal("Review")) ? "" : r.GetString(r.GetOrdinal("Review")),
                BookedAt = DateTime.Parse(r.GetString(r.GetOrdinal("BookedAt"))),
                CompletedAt = r.IsDBNull(r.GetOrdinal("CompletedAt")) ? null : DateTime.Parse(r.GetString(r.GetOrdinal("CompletedAt"))),
            };
        }
    }
}
