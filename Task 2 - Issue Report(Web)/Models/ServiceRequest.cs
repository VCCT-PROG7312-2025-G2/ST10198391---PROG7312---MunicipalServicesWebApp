namespace Task_2___Issue_Report_Web_.Models
{
    public enum RequestStatus
    {
        Submitted,
        UnderReview,
        InProgress,
        OnHold,
        Resolved,
        Closed
    }

    public class ServiceRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RequestId { get; set; } = string.Empty; // Unique identifier for tracking
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RequestStatus Status { get; set; } = RequestStatus.Submitted;
        public string? StatusNotes { get; set; }
        public int Priority { get; set; } = 5;
        public List<string> Attachments { get; set; } = new();
        public string? AssignedTo { get; set; }
        public DateTime? EstimatedCompletionDate { get; set; }
    }
}

