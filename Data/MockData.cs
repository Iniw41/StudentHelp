using TutoringMarketplace.Models;

namespace TutoringMarketplace.Data
{
    public static class MockData
    {
        public static List<Category> Categories { get; } = new()
        {
            new() { Id="tutoring",  Name="Tutoring",             Icon="🎓" },
            new() { Id="coding",    Name="Coding & Programming",  Icon="💻" },
            new() { Id="writing",   Name="Writing & Editing",     Icon="✍️"  },
            new() { Id="design",    Name="Design & Graphics",     Icon="🎨" },
            new() { Id="math",      Name="Math & Science",        Icon="🔢" },
            new() { Id="languages", Name="Languages",             Icon="🌐" },
            new() { Id="business",  Name="Business & Finance",    Icon="💼" },
            new() { Id="other",     Name="Other",                 Icon="⋯"  },
        };
    }
}
