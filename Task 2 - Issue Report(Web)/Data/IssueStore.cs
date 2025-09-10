using System.Text.Json;
using Task_2___Issue_Report_Web_.Models;

namespace Task_2___Issue_Report_Web_.Data
{
    public static class IssueStore
    {
        private static readonly string DataFile = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "issues.json");
        private static readonly object _lock = new();

        public static List<Issue> Issues { get; } = Load();

        private static List<Issue> Load()
        {
            try
            {
                var dir = Path.GetDirectoryName(DataFile);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                if (!File.Exists(DataFile))
                {
                    File.WriteAllText(DataFile, "[]");
                    return new List<Issue>();
                }
                var json = File.ReadAllText(DataFile);
                var list = JsonSerializer.Deserialize<List<Issue>>(json) ?? new List<Issue>();
                return list;
            }
            catch
            {
                return new List<Issue>();
            }
        }

        public static void Save()
        {
            lock (_lock)
            {
                var json = JsonSerializer.Serialize(Issues, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(DataFile, json);
            }
        }

        public static void Add(Issue issue)
        {
            Issues.Add(issue);
            Save();
        }
    }
}
