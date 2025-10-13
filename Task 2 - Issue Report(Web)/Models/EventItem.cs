using System;
using System.Collections.Generic;

namespace Task_2___Issue_Report_Web_.Models
{
    public class EventItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsAnnouncement { get; set; }

        public IEnumerable<string> GetKeywords()
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            void Add(string? s)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    foreach (var part in s.Split(new[] { ' ', ',', ';', '/', '\\', '-', '_' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        set.Add(part.Trim());
                    }
                }
            }

            Add(Title);
            Add(Description);
            Add(Category);
            Add(Location);
            return set;
        }
    }
}


