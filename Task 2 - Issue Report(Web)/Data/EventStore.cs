using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Task_2___Issue_Report_Web_.Models;

namespace Task_2___Issue_Report_Web_.Data
{
    public class EventStore
    {
        private readonly string _dataFilePath;
        private readonly List<EventItem> _allEvents = new List<EventItem>();
        private readonly SortedDictionary<DateTime, List<EventItem>> _eventsByDate = new SortedDictionary<DateTime, List<EventItem>>();
        private readonly Dictionary<string, List<EventItem>> _eventsByCategory = new Dictionary<string, List<EventItem>>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly Queue<string> _recentSearches = new Queue<string>();
        private const int MaxRecentSearches = 10;

        public EventStore(string contentRootPath)
        {
            _dataFilePath = Path.Combine(contentRootPath, "App_Data", "events.json");
            Load();
        }

        public IReadOnlyCollection<EventItem> AllEvents => _allEvents.AsReadOnly();
        public IReadOnlyCollection<string> Categories => _categories;

        public IEnumerable<EventItem> GetUpcoming(DateTime? from = null)
        {
            DateTime start = (from ?? DateTime.Today).Date;
            foreach (var kvp in _eventsByDate)
            {
                if (kvp.Key.Date >= start)
                {
                    foreach (var ev in kvp.Value.OrderBy(e => e.StartDate))
                        yield return ev;
                }
            }
        }

        public IEnumerable<EventItem> Search(string? category, DateTime? date)
        {
            IEnumerable<EventItem> query = GetUpcoming();
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(e => string.Equals(e.Category, category, StringComparison.OrdinalIgnoreCase));
                TrackSearch(category);
            }
            if (date.HasValue)
            {
                DateTime target = date.Value.Date;
                query = query.Where(e => e.StartDate.Date == target || (e.EndDate.HasValue && target >= e.StartDate.Date && target <= e.EndDate.Value.Date));
                TrackSearch(target.ToString("yyyy-MM-dd"));
            }
            return query.Take(200);
        }

        public IEnumerable<EventItem> GetRecommendations(int max = 6)
        {
            // Simple recommendation: prioritize categories most recently searched, then popular upcoming categories
            var recent = _recentSearches.Reverse().ToList();
            var scored = new Dictionary<Guid, int>();
            int weight = recent.Count;
            foreach (var term in recent)
            {
                foreach (var e in GetUpcoming().Where(e => e.Category.Equals(term, StringComparison.OrdinalIgnoreCase)))
                {
                    if (!scored.ContainsKey(e.Id)) scored[e.Id] = 0;
                    scored[e.Id] += weight;
                }
                weight--;
            }

            // Fallback: add top upcoming events by soonest date
            foreach (var e in GetUpcoming().Take(20))
            {
                if (!scored.ContainsKey(e.Id)) scored[e.Id] = 1;
            }

            return GetUpcoming()
                .Where(e => scored.ContainsKey(e.Id))
                .OrderByDescending(e => scored[e.Id])
                .ThenBy(e => e.StartDate)
                .Take(max)
                .ToList();
        }

        private void TrackSearch(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return;
            _recentSearches.Enqueue(term);
            while (_recentSearches.Count > MaxRecentSearches)
            {
                _recentSearches.Dequeue();
            }
        }

        private void Load()
        {
            _allEvents.Clear();
            _eventsByDate.Clear();
            _eventsByCategory.Clear();
            _categories.Clear();

            if (!File.Exists(_dataFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_dataFilePath)!);
                File.WriteAllText(_dataFilePath, "[]");
            }

            var json = File.ReadAllText(_dataFilePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var items = JsonSerializer.Deserialize<List<EventItem>>(json, options) ?? new List<EventItem>();
            foreach (var item in items)
            {
                Index(item);
            }
        }

        private void Index(EventItem item)
        {
            _allEvents.Add(item);

            var dateKey = item.StartDate.Date;
            if (!_eventsByDate.TryGetValue(dateKey, out var listByDate))
            {
                listByDate = new List<EventItem>();
                _eventsByDate[dateKey] = listByDate;
            }
            listByDate.Add(item);

            if (!_eventsByCategory.TryGetValue(item.Category, out var listByCat))
            {
                listByCat = new List<EventItem>();
                _eventsByCategory[item.Category] = listByCat;
            }
            listByCat.Add(item);

            _categories.Add(item.Category);
        }
    }
}


