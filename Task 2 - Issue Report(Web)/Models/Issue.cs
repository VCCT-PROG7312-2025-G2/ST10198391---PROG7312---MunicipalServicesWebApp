namespace Task_2___Issue_Report_Web_.Models
{
    public class Issue
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public string Location { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public List<string> Attachments { get; set; } = new();
    }
}
